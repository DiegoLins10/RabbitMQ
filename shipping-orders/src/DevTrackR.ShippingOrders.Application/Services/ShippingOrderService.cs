using DevTrackR.ShippingOrders.Application.InputModels;
using DevTrackR.ShippingOrders.Application.ViewModels;
using DevTrackR.ShippingOrders.Core.Repositories;
using System.Text.Json;

namespace DevTrackR.ShippingOrders.Application.Services
{
    public class ShippingOrderService : IShippingOrderService
    {
        private readonly IShippingOrderRepository _shippingOrderRepository;

        public ShippingOrderService(IShippingOrderRepository repository)
        {
            _shippingOrderRepository = repository;
        }

        public async Task<string> Add(AddShippingOrderInputModel model)
        {
            var shippingOrder = model.ToEntity();
            var shippingServices = model
                .Services
                .Select(s => s.ToEntity())
                .ToList();

            shippingOrder.SetupServices(shippingServices);

            Console.WriteLine(JsonSerializer.Serialize(shippingOrder));

            await _shippingOrderRepository.AddAsync(shippingOrder);

            return shippingOrder.TrackingCode;
        }

        public async Task<ShippingOrderViewModel> GetByCode(string trackingCode)
        {
            var shippingOrder = await _shippingOrderRepository.GetByCodeAsync(trackingCode);

            return ShippingOrderViewModel.FromEntity(shippingOrder);
        }
    }
}