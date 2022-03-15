using System.ServiceModel;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserPaymentCard.Grpc.Models;

namespace Service.UserPaymentCard.Grpc
{
	[ServiceContract]
	public interface IUserPaymentCardService
	{
		[OperationContract]
		ValueTask<CardsInfoGrpcResponse> GetCardNamesAsync(GetCardsInfoGrpcRequest request);

		[OperationContract]
		ValueTask<CardGrpcResponse> GetCardAsync(GetCardGrpcRequest request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> SaveCardAsync(SaveCardGrpcRequest request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> SetDefaultCardAsync(SetDefaultCardGrpcRequest request);
	}
}