using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IInventoryService _inventoryService;

        public InventoryController(ILogger<InventoryController> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet("/api/inventory")]
        public ActionResult GetCurrentInventory() 
        {
            _logger.LogInformation("Getting all inventory");

            var inventory = _inventoryService.GetCurrentInventory().Select(pi => new ProductInventoryViewModel { 
                Id = pi.Id,
                QuantityOnHand = pi.QuantityOnHand,
                IdealQuantity = pi.IdealQuantity,
                Product = ProductMapper.SerializeProductViewModel(pi.Product)
            })
            .OrderBy(inv => inv.Product.Name)
            .ToList();

            return Ok(inventory);
        }

        [HttpPatch("/api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentViewModel shipment) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Updating inventory for {shipment.ProductId} - Adjustment: {shipment.Adjustment}");

            var id = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvailable(id, adjustment);

            return Ok(inventory);
        }

        [HttpGet("/api/inventory/snapshot")]
        public ActionResult GetSnapshotHistory()
        {
            _logger.LogInformation("Getting snapshot history");

            try
            {
                var snapshotHistory = _inventoryService.GetSnapshotHistory();

                // get distinct points in time a snapshot was collected
                var timelineMarkers = snapshotHistory.Select(t => t.SnapshotTime).Distinct().ToList();

                // get quantities grouped by id
                var snapshots = snapshotHistory
                    .GroupBy(hist => hist.Product, hist => hist.QuantityOnHand, (key, g) => new ProductInventorySnapshotViewModel
                    {
                        ProductId = key.Id,
                        QuantityOnHand = g.ToList()
                    })
                    .OrderBy(hist => hist.ProductId)
                    .ToList();

                var viewModel = new SnapshotResponse
                {
                    Timeline = timelineMarkers,
                    ProductInventorySnapshots = snapshots
                };

                return Ok(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error getting snapshot history");
                _logger.LogInformation(e.StackTrace);
                return BadRequest("Error retrieving history");
            }
        }
    }
}
