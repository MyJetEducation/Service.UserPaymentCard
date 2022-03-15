using System;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserPaymentCard.Models;

namespace Service.UserPaymentCard.Services
{
	public interface IDtoRepository
	{
		ValueTask<CardDto[]> GetAllAsync(Guid? userId);

		ValueTask<CardDto> GetAsync(Guid? userId, Guid? cardId);

		ValueTask<Guid?> SaveAsync(Guid? userId, CardDto card);

		ValueTask<CommonGrpcResponse> SetDefaulAsync(Guid? userId, Guid? cardId);
	}
}