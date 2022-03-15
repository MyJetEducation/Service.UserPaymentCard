using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Models;
using Service.ServiceBus.Models;
using Service.UserPaymentCard.Mappers;
using Service.UserPaymentCard.Services;

namespace Service.UserPaymentCard.Jobs
{
	public class NewPaymentNotificator
	{
		private readonly IDtoRepository _dtoRepository;
		private readonly ILogger<NewPaymentNotificator> _logger;

		public NewPaymentNotificator(ILogger<NewPaymentNotificator> logger,
			ISubscriber<IReadOnlyList<NewPaymentServiceBusModel>> subscriber, IDtoRepository dtoRepository)
		{
			_logger = logger;
			_dtoRepository = dtoRepository;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<NewPaymentServiceBusModel> events)
		{
			foreach (NewPaymentServiceBusModel message in events)
			{
				Guid? userId = message.UserId;
				Guid? cardId = message.CardId;

				_logger.LogInformation("Handling NewPaymentServiceBusModel for user: {userId}, card id: {card}", userId, cardId);

				CommonGrpcResponse result = cardId != null
					? await _dtoRepository.SetDefaulAsync(userId, cardId)
					: await _dtoRepository.SaveAsync(userId, message.ToDto());

				if (!result.IsSuccess)
					_logger.LogError("Failed to process result of NewPaymentServiceBusModel for user: {userId}, card id: {card}", userId, cardId);
			}
		}
	}
}