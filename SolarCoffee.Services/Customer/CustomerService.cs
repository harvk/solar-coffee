using Microsoft.EntityFrameworkCore;
using SolarCoffee.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarCoffee.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly SolarDbContext _db;

        public CustomerService(SolarDbContext dbContext) 
        {
            _db = dbContext;
        }
        public ServiceResponse<Data.Models.Customer> CreateCustomer(Data.Models.Customer customer)
        {
            try 
            {
                _db.Customers.Add(customer);
                _db.SaveChanges();

                return new ServiceResponse<Data.Models.Customer> { 
                    Data = customer,
                    Time = DateTime.UtcNow,
                    Message = "Saved new customer",
                    IsSuccess = true
                };
            } 
            catch (Exception e) 
            {
                return new ServiceResponse<Data.Models.Customer>
                {
                    Data = customer,
                    Time = DateTime.UtcNow,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
        }

        public ServiceResponse<bool> DeleteCustomer(int id)
        {
            var customer = _db.Customers.Find(id);
            var now = DateTime.UtcNow;

            if (customer == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Time = now,
                    Message = "Customer to delete not found!",
                    IsSuccess = false
                };
            }

            try 
            {
                _db.Customers.Remove(customer);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Time = now,
                    Message = "Deleted customer",
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

        public List<Data.Models.Customer> GetAllCustomers()
        {
            return _db.Customers
                .Include(customer => customer.PrimaryAddress) // loads additional data from referenced classes in model defintion
                .OrderBy(customer => customer.LastName)
                .ToList();
        }

        public Data.Models.Customer GetCustomerById(int id)
        {
            //return _db.Customers.FirstOrDefault(customer => customer.Id == id);
            return _db.Customers.Find(id);
        }
    }
}
