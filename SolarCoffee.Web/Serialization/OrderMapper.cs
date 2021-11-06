﻿using SolarCoffee.Data.Models;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Serialization
{
    public static class OrderMapper
    {
        public static SalesOrder SerializeInvoiceToOrder(InvoiceViewModel invoice)
        {
            var salesOrderItems = invoice.LineItems
                .Select(item => new SalesOrderItem
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    Product = ProductMapper.SerializeProductViewModel(item.Product)
                }).ToList();

            return new SalesOrder
            {
                SalesOrderItems = salesOrderItems,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
        }

        public static List<OrderViewModel> SerializeOrders(IEnumerable<SalesOrder> orders)
        {
            return orders.Select(order => new OrderViewModel { 
                Id = order.Id,
                CreatedOn = order.CreatedOn,
                UpdatedOn = order.UpdatedOn,
                SalesOrderItems = SerializeSalesOrderItems(order.SalesOrderItems),
                Customer = CustomerMapper.SerializeCustomer(order.Customer),
                IsPaid = order.IsPaid
            }).ToList();
        }

        private static List<SalesOrderItemViewModel> SerializeSalesOrderItems(IEnumerable<SalesOrderItem> orderItems) 
        {
            return orderItems.Select(item => new SalesOrderItemViewModel { 
                Id = item.Id,
                Quantity = item.Quantity,
                Product = ProductMapper.SerializeProductViewModel(item.Product)
            }).ToList();
        }
    }
}
