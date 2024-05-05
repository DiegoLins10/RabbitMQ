using DevTrackR.ShippingOrders.Core.Entities;
using DevTrackR.ShippingOrders.Core.Repositories;
using MongoDB.Driver;

namespace DevTrackR.ShippingOrders.Infrastructure.Persistence.Repositories
{
    public class ShippingOrderRepository : IShippingOrderRepository
    {

        private readonly IMongoCollection<ShippingOrder> _collection;

        public ShippingOrderRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<ShippingOrder>("shipping-orders");
        }

        public Task AddAsync(ShippingOrder shippingOrder)
        {
            throw new NotImplementedException();
        }

        public Task<ShippingOrder> GetByCodeAsync(string code)
        {
            throw new NotImplementedException();
        }
    }
}