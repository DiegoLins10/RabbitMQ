using DevTrackR.ShippingOrders.Application.ViewModels;
using DevTrackR.ShippingOrders.Core.Repositories;

namespace DevTrackR.ShippingOrders.Application.Services
{
    public class ShippingServiceService : IShippingServiceService
    {
        private readonly IShippingServiceRepository _shippingServiceRepository;

        public ShippingServiceService(IShippingServiceRepository shippingServiceRepository)
        {
            _shippingServiceRepository = shippingServiceRepository;
        }

        public async Task<List<ShippingServiceViewModel>> GetAll()
        {
            var shippingServices = await _shippingServiceRepository.GetAllAsync();

            return shippingServices
                    .Select(s => new ShippingServiceViewModel(s.Id, s.Title, s.PricePerKg, s.FixedPrice))
                    .ToList();
        }
    }
}