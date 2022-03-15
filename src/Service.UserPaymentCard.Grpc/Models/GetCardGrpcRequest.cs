using System;
using System.Runtime.Serialization;

namespace Service.UserPaymentCard.Grpc.Models
{
	[DataContract]
	public class GetCardGrpcRequest
	{
		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }

		[DataMember(Order = 2)]
		public Guid? CardId { get; set; }
	}
}