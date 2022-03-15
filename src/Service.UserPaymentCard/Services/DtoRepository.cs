using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Core.Client.Services;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserPaymentCard.Models;

namespace Service.UserPaymentCard.Services
{
	public class DtoRepository : IDtoRepository
	{
		private static Func<string> Key => Program.ReloadedSettings(model => model.KeyUserPaymentCard);

		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;
		private readonly ILogger<DtoRepository> _logger;
		private readonly IEncoderDecoder _encoderDecoder;

		public DtoRepository(IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService, ILogger<DtoRepository> logger, IEncoderDecoder encoderDecoder)
		{
			_serverKeyValueService = serverKeyValueService;
			_logger = logger;
			_encoderDecoder = encoderDecoder;
		}

		public async ValueTask<CardDto[]> GetAllAsync(Guid? userId)
		{
			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				UserId = userId,
				Key = Key.Invoke()
			});

			if (response?.Value == null)
				return Array.Empty<CardDto>();

			CardDto[] dtos = JsonSerializer.Deserialize<CardDto[]>(response.Value) ?? Array.Empty<CardDto>();

			foreach (CardDto cardDto in dtos)
				DecodeCardDto(cardDto);

			return dtos;
		}

		public async ValueTask<CardDto> GetAsync(Guid? userId, Guid? cardId)
		{
			CardDto[] cards = await GetAllAsync(userId);

			return cards.FirstOrDefault(dto => dto.CardId == cardId);
		}

		public async ValueTask<Guid?> SaveAsync(Guid? userId, CardDto card)
		{
			List<CardDto> cardDtos = (await GetAllAsync(userId)).ToList();

			ClearDefault(cardDtos);

			Guid? cardId;

			CardDto existingCard = cardDtos
				.WhereIf(card.CardId != null, dto => dto.CardId == card.CardId)
				.WhereIf(card.CardId == null, dto => dto.Number == card.Number)
				.FirstOrDefault();

			if (existingCard != null)
			{
				cardId = existingCard.CardId;

				UpdateCardDto(existingCard, card);
			}
			else
			{
				cardId = card.CardId ?? Guid.NewGuid();

				var newCard = new CardDto {CardId = cardId};

				UpdateCardDto(newCard, card);

				cardDtos.Add(newCard);
			}

			CommonGrpcResponse response = await SaveDtosAsync(userId, cardDtos.ToArray());
			return response.IsSuccess
				? cardId
				: await ValueTask.FromResult<Guid?>(null);
		}

		public async ValueTask<CommonGrpcResponse> SetDefaulAsync(Guid? userId, Guid? cardId)
		{
			CardDto[] cardDtos = await GetAllAsync(userId);

			ClearDefault(cardDtos);

			CardDto cardDto = cardDtos.FirstOrDefault(dto => dto.CardId == cardId);

			if (cardDto == null)
			{
				_logger.LogError("Can't find card with id: {id} for set as default for userId: {user}", cardId, userId);

				return CommonGrpcResponse.Fail;
			}

			cardDto.IsDefault = true;

			return await SaveDtosAsync(userId, cardDtos);
		}

		private async ValueTask<CommonGrpcResponse> SaveDtosAsync(Guid? userId, CardDto[] cardDtos)
		{
			foreach (CardDto cardDto in cardDtos)
				EncodeCardDto(cardDto);

			CommonGrpcResponse response = await _serverKeyValueService.TryCall(service => service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = Key.Invoke(),
						Value = JsonSerializer.Serialize(cardDtos)
					}
				}
			}));

			if (response?.IsSuccess != true)
				_logger.LogError("Can't set server key values for user {user} with values: {@cardDtos}", userId, cardDtos);

			return response;
		}

		private static void ClearDefault(IEnumerable<CardDto> cardDtos)
		{
			foreach (CardDto cardDto in cardDtos)
				cardDto.IsDefault = false;
		}

		private static void UpdateCardDto(CardDto cardDto, CardDto paymentCard)
		{
			cardDto.Name = GetName(paymentCard.Number);
			cardDto.IsDefault = true;

			cardDto.Number = paymentCard.Number;
			cardDto.Holder = paymentCard.Holder;
			cardDto.Month = paymentCard.Month;
			cardDto.Year = paymentCard.Year;
			cardDto.Cvv = paymentCard.Cvv;
		}

		private void EncodeCardDto(CardDto cardDto)
		{
			cardDto.Number = Encode(cardDto.Number);
			cardDto.Holder = Encode(cardDto.Holder);
			cardDto.Month = Encode(cardDto.Month);
			cardDto.Year = Encode(cardDto.Year);
			cardDto.Cvv = Encode(cardDto.Cvv);
		}

		private void DecodeCardDto(CardDto cardDto)
		{
			cardDto.Number = Decode(cardDto.Number);
			cardDto.Holder = Decode(cardDto.Holder);
			cardDto.Month = Decode(cardDto.Month);
			cardDto.Year = Decode(cardDto.Year);
			cardDto.Cvv = Decode(cardDto.Cvv);
		}

		private static string GetName(string number)
		{
			int len = number.Length;

			return len <= 4 ? number : $"{string.Concat(Enumerable.Repeat("*", len - 4))}{number.Substring(len - 4, 4)}";
		}

		private string Encode(string value) => _encoderDecoder.Encode(value);
		private string Decode(string value) => _encoderDecoder.Decode(value);
	}
}