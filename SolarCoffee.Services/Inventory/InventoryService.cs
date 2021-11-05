using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarCoffee.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly SolarDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(SolarDbContext dbContext, ILogger<InventoryService> logger) 
        {
            _db = dbContext;
            _logger = logger;
        }

        public IEnumerable<ProductInventory> GetCurrentInventory()
        {
            return _db.ProductInventories
                .Include(pi => pi.Product)
                .Where(pi => !pi.Product.IsArchived)
                .ToList();
        }

        public ProductInventory GetProductById(int productId)
        {
            return _db.ProductInventories
                .Include(pi => pi.Product)
                .FirstOrDefault(pi => pi.Product.Id == productId);
        }

        public List<ProductInventorySnapshot> GetSnapshotHistory()
        {
            var earliest = DateTime.UtcNow - TimeSpan.FromHours(6);

            return _db.ProductInventorySnapshots
                .Include(snap => snap.Product)
                .Where(snap => snap.SnapshotTime > earliest && !snap.Product.IsArchived)
                .ToList();
        }

        public ServiceResponse<ProductInventory> UpdateUnitsAvailable(int id, int adjustment)
        {
            try 
            {
                var inventory = _db.ProductInventories
                    .Include(inventory => inventory.Product)
                    .First(inventory => inventory.Product.Id == id);

                inventory.QuantityOnHand += adjustment;

                try 
                {
                    CreateSnapshot(inventory);
                } 
                catch (Exception e) 
                {
                    _logger.LogError("Error creating inventory snapshot");
                    _logger.LogError(e.StackTrace);
                }

                _db.SaveChanges();

                return new ServiceResponse<ProductInventory> { 
                    Data = inventory,
                    Time = DateTime.UtcNow,
                    Message = $"Product {id} inventory adjusted",
                    IsSuccess = true
                };
            } 
            catch (Exception e) 
            {
                return new ServiceResponse<ProductInventory>
                {
                    Data = null,
                    Time = DateTime.UtcNow,
                    Message = $"Error updating product inventory quantity",
                    IsSuccess = false
                };
            }
        }

        private void CreateSnapshot(ProductInventory inventory)
        {
            var now = DateTime.UtcNow;

            var snapshot = new ProductInventorySnapshot
            {
                SnapshotTime = now,
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand
            };

            _db.Add(snapshot);
        }
    }
}
