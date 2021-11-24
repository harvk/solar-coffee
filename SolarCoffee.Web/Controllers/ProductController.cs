using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Product;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService) {
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost("/api/product")]
        public ActionResult AddProduct([FromBody] ProductViewModel product) {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding product");

            var newProduct = ProductMapper.SerializeProductViewModel(product);

            var newProductResponse = _productService.CreateProduct(newProduct);

            return Ok(newProductResponse);
        }

        /// <summary>
        /// Returns all products
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/product")]
        public ActionResult GetProduct() {
            _logger.LogInformation("Getting all products");

            var products = _productService.GetAllProducts();

            //var productViewModels = products.Select(product => ProductMapper.SerializeProductViewModel(product));
            var productViewModels = products.Select(ProductMapper.SerializeProductViewModel);

            return Ok(productViewModels);
        }

        /// <summary>
        /// Archives an existing product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("/api/product/{id}")]
        public ActionResult ArchiveProduct(int id)
        {
            _logger.LogInformation("Archiving product");

            var archiveResult = _productService.ArchiveProduct(id);

            return Ok(archiveResult);
        }
    }
}
