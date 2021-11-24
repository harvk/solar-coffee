<template>
    <div class="inventory-container">
        <div id="inventory-title">
            <h1>Inventory Dashboard</h1>
        </div>
        
        <hr />
        
        <div class="inventory-actions">
            <solar-button @button:click="showNewProductModal" id="add-new-btn">Add New Item</solar-button>
            <solar-button @button:click="showShipmentModal" id="receive-shipment-btn">Receive Shipment</solar-button>
        </div>

        <table id="inventory-table" class="table">
            <tr>
                <th>Item</th>
                <th>Quantity On-hand</th>
                <th>Unit Price</th>
                <th>Taxable</th>
                <th>Delete</th>
            </tr>
            <tr v-for="item in inventory" :key="item.id">
                <td>{{ item.product.name }}</td>
                <td v-bind:class="`${applyColor(item.quantityOnHand, item.idealQuantity)}`">
                    {{ item.quantityOnHand }}
                </td>
                <td>{{ item.product.price | price }}</td>
                <td>{{ item.product.isTaxable }}</td>
                <td>
                    <div class="lni lni-cross-circle product-archive" @click="archiveProduct(item.product.id)"></div>
                </td>
            </tr>
        </table>

        <new-product-modal v-if="isNewProductVisible" @save:product="saveNewProduct" @close="closeModal"></new-product-modal>
        <shipment-modal v-if="isShipmentVisible" :inventory="inventory" 
                        @save:shipment="saveNewShipment" @close="closeModal"></shipment-modal>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { IProduct, IProductInventory } from "@/types/Product";
import SolarButton from "@/components/SolarButton.vue";
import NewProductModal from "@/components/modals/NewProductModal.vue";
import ShipmentModal from "@/components/modals/ShipmentModal.vue";
import { IShipment } from "@/types/Shipment";
import { InventoryService } from "@/services/inventory-service";
import { ProductService } from "@/services/product-service";

const inventoryService = new InventoryService();
const productService = new ProductService();

@Component({
    name: "Inventory",
    components: { SolarButton, NewProductModal, ShipmentModal }
})

export default class Inventory extends Vue { 
    isNewProductVisible: boolean = false;
    isShipmentVisible: boolean = false;
    
    inventory: IProductInventory[] = [];

    applyColor(current: number, target: number) {
        if (current <= 0) {
            return "red";
        }
        
        if (Math.abs(target - current) > 8) {
            return "yellow";
        }

        return "green";
    };

    showNewProductModal() {
        this.isNewProductVisible = true;
    };

    showShipmentModal() {
        this.isShipmentVisible = true;
    };

    closeModal() {
        this.isShipmentVisible = false;
        this.isNewProductVisible = false;
    };

    async archiveProduct(productId: number) {
        await productService.archive(productId);
        await this.initialize();
    }

    async saveNewProduct(product: IProduct) {
        await productService.save(product);
        this.isNewProductVisible = false;
        await this.initialize();
    };

    async saveNewShipment(shipment: IShipment) {
        await inventoryService.updateInventoryQuantity(shipment);
        this.isShipmentVisible = false;
        await this.initialize();
    };

    async initialize() {
        this.inventory = await inventoryService.getInventory();
    }

    async created() {
        await this.initialize();
    }
}
</script>

<style lang="scss" scoped>
    @import "@/scss/global.scss";

    .green {
        font-weight: bold;
        color: $solar-green;
    }

    .yellow {
        font-weight: bold;
        color: $solar-yellow;
    }

    .red {
        font-weight: bold;
        color: $solar-red;
    }

    .inventory-actions {
        display: flex;
        margin-bottom: 0.8rem;
    }

    .product-archive {
        cursor: pointer;
        font-weight: bold;
        font-size: 1.2rem;
        color: $solar-red;
    }
</style>
