using System;
using System.Runtime.Serialization;

namespace Service.UserPaymentCard.Grpc.Models
{
    [DataContract]
    public class GetCardsInfoGrpcRequest
    {
        [DataMember(Order = 1)]
        public Guid? UserId { get; set; }
    }
}
