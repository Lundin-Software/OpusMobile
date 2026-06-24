using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Orders;
using System.Text.Json;

namespace Opus.Mobile.API.Services.Orders;

public class OrderService(OpusDBContext ctx) : IOrderService
{
    public async Task<IEnumerable<OrderItem>> GetOrders()
    {
        var orders = await ctx.Orders
            .AsNoTracking()
            .Where(order => order.Closed == false)
            .Select(order => new OrderItem
            {
                Id = order.Id,
                OrderNr = order.OrderNr,
                Description = order.Description,
                Supplier = order.Suppl == null ? null : order.Suppl.Name,
                Department = order.DepartId,
                Received = order.OrdersLocationsHistory.Any(),
                Location = order.OrdersLocationsHistory
                    .OrderByDescending(location => location.Id)
                    .Select(location =>
                        location.Shelve == null
                            ? string.Empty
                            : location.Shelve.Name + " - " +
                              location.Shelve.Rack.Name + " - " +
                              location.Shelve.Rack.Stock.ShortName)
                    .FirstOrDefault() ?? string.Empty
            })
            .ToListAsync();

        return orders;
    }

    public async Task<string> SetOrderLocation(int userId, int orderId, SetOrderLocationRequest request)
    {
        try
        {
            await ctx.OrdersLocationsHistory.AddAsync(new OrdersLocationsHistory
            {
                OrderId = orderId,
                ShelveId = request.ShelfId,
                Nr = request.Nr,
                Date = DateTime.Now,
                UserId = userId
            });

            await ctx.SaveChangesAsync();

            await RequestLabelPrint(new LabelPrintRequest
            {
                ReportName = "ReportOrderLocationLabel",
                ParameterName = "p_orderID",
                ParameterValue = orderId.ToString()
            });

            return "OK";
        }
        catch (Exception ex)
        {
            return ex.InnerException?.Message ?? ex.Message;
        }
    }

    private static async Task RequestLabelPrint(LabelPrintRequest report)
    {
        await using var connection = new HubConnectionBuilder()
            .WithUrl("http://vardin.gannet.biz:9877/LabelPrintHub")
            .Build();

        await connection.StartAsync();

        var requestString = JsonSerializer.Serialize(report);

        await connection.SendAsync("PrintLabel", "JB", requestString);

        await Task.Delay(200);

        await connection.StopAsync();
    }

    private class LabelPrintRequest
    {
        public string ReportName { get; set; } = string.Empty;

        public string ParameterName { get; set; } = string.Empty;

        public string ParameterValue { get; set; } = string.Empty;
    }
}
