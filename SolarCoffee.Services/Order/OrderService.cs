using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Services.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarCoffee.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly SolarDbContext _db;
        private readonly ILogger<OrderService> _logger;
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;

        public OrderService(SolarDbContext dbContext, ILogger<OrderService> logger, IProductService productService, 
            IInventoryService inventoryService) 
        {
            _db = dbContext;
            _logger = logger;
            _productService = productService;
            _inventoryService = inventoryService;
        }

        public ServiceResponse<bool> GenerateOpenOrder(SalesOrder order)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation("Generating new order");

            foreach (var item in order.SalesOrderItems) 
            {
                item.Product = _productService.GetProductById(item.Product.Id);

                var inventoryId = _inventoryService.GetProductById(item.Product.Id).Id;

                _inventoryService.UpdateUnitsAvailable(inventoryId, -item.Quantity);
            }

            try 
            {
                _db.SalesOrders.Add(order);
                _db.SaveChanges();

                return new ServiceResponse<bool> { 
                    Data = true,
                    Time = now,
                    Message = "Open order created",
                    IsSuccess = true
                };
            } 
            catch (Exception e) 
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Time = now,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
        }

        public List<SalesOrder> GetAllSalesOrders()
        {
            return _db.SalesOrders
                .Include(so => so.Customer)
                    .ThenInclude(cust => cust.PrimaryAddress)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(item => item.Product)
                .ToList();
        }

        public ServiceResponse<bool> MarkFulfilled(int id)
        {
            var now = DateTime.Now;
            var order = _db.SalesOrders.Find(id);

            order.UpdatedOn = now;
            order.IsPaid = true;

            try 
            {
                _db.SalesOrders.Update(order);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Time = now,
                    Message = $"Order {order.Id} closed: Invoice paid in full.",
                    IsSuccess = true
                };
            } 
            catch (Exception e)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Time = now,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
        }
    }
}
