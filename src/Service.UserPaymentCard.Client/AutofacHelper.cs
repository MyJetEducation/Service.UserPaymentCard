using Autofac;
using Microsoft.Extensions.Logging;
using Service.UserPaymentCard.Grpc;
using Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.UserPaymentCard.Client
{
    public static class AutofacHelper
    {
        public static void RegisterUserPaymentCardClient(this ContainerBuilder builder, string grpcServiceUrl, ILogger logger)
        {
            var factory = new UserPaymentCardClientFactory(grpcServiceUrl, logger);

            builder.RegisterInstance(factory.GetUserPaymentCardService()).As<IGrpcServiceProxy<IUserPaymentCardService>>().SingleInstance();
        }
    }
}
