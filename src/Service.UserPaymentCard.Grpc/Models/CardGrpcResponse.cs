using System;
using System.Runtime.Serialization;

namespace Service.UserPaymentCard.Grpc.Models
{
	[DataContract]
	public class CardGrpcResponse
	{
		[DataMember(Order = 1)]
		public Guid? CardId { get; set; }

		[DataMember(Order = 2)]
		public string Number { get; set; }

		[DataMember(Order = 3)]
		public string Holder { get; set; }

		[DataMember(Order = 4)]
		public string Month { get; set; }

		[DataMember(Order = 5)]
		public string Year { get; set; }

		[DataMember(Order = 6)]
		public string Cvv { get; set; }
	}
}