using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.Core.Client.Services;
using Service.ServerKeyValue.Client;
using Service.ServiceBus.Models;
using Service.UserPaymentCard.Jobs;
using Service.UserPaymentCard.Services;

namespace Service.UserPaymentCard.Modules
{
	public class ServiceModule : Module
	{
		private const string QueueName = "MyJetEducation-UserPaymentCard";

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<DtoRepository>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof (ServerKeyValueClientFactory)));

			builder.Register(context => new EncoderDecoder(Program.EncodingKey))
				.As<IEncoderDecoder>()
				.SingleInstance();

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterMyServiceBusSubscriberBatch<NewPaymentServiceBusModel>(serviceBusClient, NewPaymentServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);

			builder.RegisterType<NewPaymentNotificator>().AutoActivate().SingleInstance();
		}
	}
}