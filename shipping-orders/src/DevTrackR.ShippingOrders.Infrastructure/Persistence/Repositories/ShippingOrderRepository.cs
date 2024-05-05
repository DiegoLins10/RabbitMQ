﻿using DevTrackR.ShippingOrders.Core.Entities;
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

        public async Task AddAsync(ShippingOrder shippingOrder)
        {
            await _collection.InsertOneAsync(shippingOrder);
        }

        public async Task<ShippingOrder> GetByCodeAsync(string code)
        {
            return await _collection.Find(d => d.TrackingCode == code).SingleOrDefaultAsync();
        }
    }
}