using System;
using System.Runtime.Serialization;

namespace Service.UserPaymentCard.Grpc.Models
{
	[DataContract]
	public class CardsInfoGrpcModel
	{
		[DataMember(Order = 1)]
		public Guid? CardId { get; set; }

		[DataMember(Order = 2)]
		public string CardName { get; set; }

		[DataMember(Order = 3)]
		public bool IsDefault { get; set; }
	}
}