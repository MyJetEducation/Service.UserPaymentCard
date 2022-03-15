using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.UserPaymentCard.Client;
using Service.UserPaymentCard.Grpc;
using Service.UserPaymentCard.Grpc.Models;
using GrpcClientFactory = ProtoBuf.Grpc.Client.GrpcClientFactory;

namespace TestApp
{
	public class Program
	{
		private static async Task Main()
		{
			GrpcClientFactory.AllowUnencryptedHttp2 = true;
			ILogger<Program> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

			Console.Write("Press enter to start");
			Console.ReadLine();

			var factory = new UserPaymentCardClientFactory("http://localhost:5001", logger);
			IGrpcServiceProxy<IUserPaymentCardService> serviceProxy = factory.GetUserPaymentCardService();
			IUserPaymentCardService client = serviceProxy.Service;

			var userId = new Guid("000005dc-0000-0000-dc05-000000000000");
			var card1Id = Guid.NewGuid();
			var card2Id = Guid.NewGuid();

			Console.WriteLine($"Save Card #1 for {userId} !");
			CommonGrpcResponse save1Response = await client.SaveCardAsync(new SaveCardGrpcRequest
			{
				CardId = card1Id,
				Number = "1111111111111111",
				Holder = "Holder 1",
				Month = "01",
				Year = "02",
				UserId = userId,
				Cvv = "345"
			});

			if (!save1Response.IsSuccess)
				throw new Exception("Error! Can't save Card #1");

			CardsInfoGrpcResponse cards1Response = await client.GetCardNamesAsync(new GetCardsInfoGrpcRequest {UserId = userId});
			Console.WriteLine($"Cards for {userId}:");
			LogData(cards1Response);

			if (!cards1Response.Items.First(model => model.CardId == card1Id).IsDefault)
				throw new Exception("Error! Card #1 not default");

			Console.WriteLine($"Save Card #2 for {userId} !");
			CommonGrpcResponse save2Response = await client.SaveCardAsync(new SaveCardGrpcRequest
			{
				CardId = card2Id,
				Number = "2222222222222222",
				Holder = "Holder 2",
				Month = "03",
				Year = "04",
				UserId = userId,
				Cvv = "678"
			});

			if (!save2Response.IsSuccess)
				throw new Exception("Error! Can't save Card #2");

			CardsInfoGrpcResponse cards2Response = await client.GetCardNamesAsync(new GetCardsInfoGrpcRequest {UserId = userId});
			Console.WriteLine($"Cards for {userId}:");
			LogData(cards2Response);

			if (!cards2Response.Items.First(model => model.CardId == card2Id).IsDefault)
				throw new Exception("Error! Card #2 not default");

			CommonGrpcResponse setCard2DefResponse = await client.SetDefaultCardAsync(new SetDefaultCardGrpcRequest {UserId = userId, CardId = card1Id});
			if (!setCard2DefResponse.IsSuccess)
				throw new Exception("Error! Can't set Card #1 as default");

			CardsInfoGrpcResponse cards3Response = await client.GetCardNamesAsync(new GetCardsInfoGrpcRequest {UserId = userId});
			Console.WriteLine($"Cards for {userId}:");
			LogData(cards3Response);

			if (!cards3Response.Items.First(model => model.CardId == card1Id).IsDefault)
				throw new Exception("Error! Card #1 not default");

			CardGrpcResponse card1Response = await client.GetCardAsync(new GetCardGrpcRequest {UserId = userId, CardId = card1Id});
			if (card1Response.CardId == null)
				throw new Exception("Error! Can't get card #1");

			Console.WriteLine("Card #1 data:");
			LogData(card1Response);

			Console.WriteLine("End");
			Console.ReadLine();
		}

		private static void LogData(object data) => Console.WriteLine(JsonSerializer.Serialize(data));
	}
}