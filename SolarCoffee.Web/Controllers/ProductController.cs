﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Product;
using SolarCoffee.Web.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        public readonly ILogger<ProductController> _logger;
        public readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService) {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("/api/product")]
        public ActionResult GetProduct() {
            _logger.LogInformation("Getting all products");

            var products = _productService.GetAllProducts();

            //var productViewModels = products.Select(product => ProductMapper.SerializeProductViewModel(product));
            var productViewModels = products.Select(ProductMapper.SerializeProductViewModel);

            return Ok(productViewModels);
        }
    }
}