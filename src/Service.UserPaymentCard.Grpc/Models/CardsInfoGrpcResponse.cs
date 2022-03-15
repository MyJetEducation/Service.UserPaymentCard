using System.Runtime.Serialization;

namespace Service.UserPaymentCard.Grpc.Models
{
	[DataContract]
	public class CardsInfoGrpcResponse
	{
		[DataMember(Order = 1)]
		public CardsInfoGrpcModel[] Items { get; set; }
	}
}