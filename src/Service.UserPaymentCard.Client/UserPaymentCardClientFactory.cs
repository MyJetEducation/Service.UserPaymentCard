using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Service.UserPaymentCard.Grpc;
using Service.Grpc;

namespace Service.UserPaymentCard.Client
{
    [UsedImplicitly]
    public class UserPaymentCardClientFactory : GrpcClientFactory
    {
        public UserPaymentCardClientFactory(string grpcServiceUrl, ILogger logger) : base(grpcServiceUrl, logger)
        {
        }

        public IGrpcServiceProxy<IUserPaymentCardService> GetUserPaymentCardService() => CreateGrpcService<IUserPaymentCardService>();
    }
}
