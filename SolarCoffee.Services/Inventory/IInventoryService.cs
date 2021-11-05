using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolarCoffee.Services.Inventory
{
    public interface IInventoryService
    {
        public IEnumerable<ProductInventory> GetCurrentInventory();
        public ServiceResponse<ProductInventory> UpdateUnitsAvailable(int id, int adjustment);
        public ProductInventory GetProductById(int productId);
        public List<ProductInventorySnapshot> GetSnapshotHistory();
    }
}
