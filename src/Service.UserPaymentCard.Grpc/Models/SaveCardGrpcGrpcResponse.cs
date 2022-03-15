using System;
using System.Runtime.Serialization;

namespace Service.UserPaymentCard.Grpc.Models
{
	[DataContract]
	public class SaveCardGrpcGrpcResponse
	{
		[DataMember(Order = 2)]
		public Guid? CardId { get; set; }
	}
}