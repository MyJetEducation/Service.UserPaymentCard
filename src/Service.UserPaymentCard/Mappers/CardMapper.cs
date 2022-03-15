using Service.ServiceBus.Models;
using Service.UserPaymentCard.Grpc.Models;
using Service.UserPaymentCard.Models;

namespace Service.UserPaymentCard.Mappers
{
	public static class CardMapper
	{
		public static CardDto ToDto(this NewPaymentServiceBusModel message) => new CardDto
		{
			CardId = message.CardId,
			Number = message.Number,
			Holder = message.Holder,
			Month = message.Month,
			Year = message.Year,
			Cvv = message.Cvv
		};

		public static CardDto ToDto(this SaveCardGrpcRequest request) => new CardDto
		{
			CardId = request.CardId,
			Number = request.Number,
			Holder = request.Holder,
			Month = request.Month,
			Year = request.Year,
			Cvv = request.Cvv
		};

		public static CardGrpcResponse ToCardGrpcModel(this CardDto card) => new CardGrpcResponse
		{
			CardId = card?.CardId,
			Number = card?.Number,
			Holder = card?.Holder,
			Month = card?.Month,
			Year = card?.Year,
			Cvv = card?.Cvv
		};

		public static CardsInfoGrpcModel ToCardInfoGrpcModel(this CardDto dto) => new CardsInfoGrpcModel
		{
			CardId = dto.CardId,
			CardName = dto.Name,
			IsDefault = dto.IsDefault
		};
	}
}