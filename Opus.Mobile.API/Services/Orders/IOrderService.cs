using Opus.Mobile.Shared.Orders;

namespace Opus.Mobile.API.Services.Orders;

public interface IOrderService
{
    Task<IEnumerable<OrderItem>> GetOrders();

    Task<string> SetOrderLocation(int userId, int orderId, SetOrderLocationRequest request);

    Task RequestOrderLocationLabelPrint(int orderId);
}
