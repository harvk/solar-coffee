using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Serialization
{
    public static class ProductMapper
    {
        public static ProductViewModel SerializeProductViewModel(Data.Models.Product product) 
        {
            return new ProductViewModel { 
                Id = product.Id,
                CreatedOn = product.CreatedOn,
                UpdatedOn = product.UpdatedOn,
                Price = product.Price,
                Name = product.Name,
                Description = product.Description,
                IsTaxable = product.IsTaxable,
                IsArchived = product.IsArchived
            };
        }

        public static Data.Models.Product SerializeProductViewModel(ProductViewModel productVieModel)
        {
            return new Data.Models.Product
            {
                Id = productVieModel.Id,
                CreatedOn = productVieModel.CreatedOn,
                UpdatedOn = productVieModel.UpdatedOn,
                Price = productVieModel.Price,
                Name = productVieModel.Name,
                Description = productVieModel.Description,
                IsTaxable = productVieModel.IsTaxable,
                IsArchived = productVieModel.IsArchived
            };
        }
    }
}
