using Autofac;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Services;
using Service.ServerKeyValue.Client;
using Service.UserPaymentCard.Services;

namespace Service.UserPaymentCard.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<DtoRepository>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof (ServerKeyValueClientFactory)));

			builder.Register(context => new EncoderDecoder(Program.EncodingKey))
				.As<IEncoderDecoder>()
				.SingleInstance();
		}
	}
}