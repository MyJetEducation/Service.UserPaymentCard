using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Models;
using Service.UserPaymentCard.Grpc;
using Service.UserPaymentCard.Grpc.Models;
using Service.UserPaymentCard.Mappers;
using Service.UserPaymentCard.Models;

namespace Service.UserPaymentCard.Services
{
	public class UserPaymentCardService : IUserPaymentCardService
	{
		private readonly IDtoRepository _dtoRepository;
		private readonly ILogger<UserPaymentCardService> _logger;

		public UserPaymentCardService(ILogger<UserPaymentCardService> logger, IDtoRepository dtoRepository)
		{
			_logger = logger;
			_dtoRepository = dtoRepository;
		}

		public async ValueTask<CardsInfoGrpcResponse> GetCardNamesAsync(GetCardsInfoGrpcRequest request)
		{
			CardDto[] cards = await _dtoRepository.GetAllAsync(request.UserId);

			return new CardsInfoGrpcResponse
			{
				Items = cards
					.Select(dto => dto.ToCardInfoGrpcModel())
					.OrderBy(model => model.IsDefault)
					.ToArray()
			};
		}

		public async ValueTask<CardGrpcResponse> GetCardAsync(GetCardGrpcRequest request)
		{
			CardDto card = await _dtoRepository.GetAsync(request.UserId, request.CardId);

			if (card == null)
				_logger.LogError("No user {userId} payment card found by id: {id}", request.UserId, request.CardId);

			return card.ToCardGrpcModel();
		}

		public async ValueTask<CommonGrpcResponse> SaveCardAsync(SaveCardGrpcRequest request) => await _dtoRepository.SaveAsync(request.UserId, request.ToDto());

		public async ValueTask<CommonGrpcResponse> SetDefaultCardAsync(SetDefaultCardGrpcRequest request) => await _dtoRepository.SetDefaulAsync(request.UserId, request.CardId);
	}
}