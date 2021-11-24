<template>
    <div class="inventory-container">
        <div id="inventory-title">
            <h1>Inventory Dashboard</h1>
        </div>
        
        <hr />
        
        <div class="inventory-actions">
            <solar-button @click.native="showNewProductModal" id="add-new-btn">Add New Item</solar-button>
            <solar-button @click.native="showShipmentModal" id="receive-shipment-btn">Receive Shipment</solar-button>
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
                <td>{{ item.quantityOnHand }}</td>
                <td>{{ item.product.price | price }}</td>
                <td>{{ item.product.isTaxable }}</td>
                <td>
                    <div>x</div>
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

@Component({
    name: "Inventory",
    components: { SolarButton, NewProductModal, ShipmentModal }
})

export default class Inventory extends Vue { 
    isNewProductVisible: boolean = false;
    isShipmentVisible: boolean = false;
    
    inventory: IProductInventory[] = [
        {
            id: 1,
            product: {
                id: 1, 
                name: "Some Product", 
                description: "Some product", 
                price: 100, 
                createdOn: new Date(), 
                updatedOn: new Date(),
                isTaxable: true,
                isArchived: false
            },
            quantityOnHand: 100,
            idealQuantity: 100
        },
        {
            id: 2,
            product: {
                id: 2, 
                name: "Some Great Product", 
                description: "Some great product", 
                price: 100, 
                createdOn: new Date(), 
                updatedOn: new Date(),
                isTaxable: true,
                isArchived: false
            },
            quantityOnHand: 40,
            idealQuantity: 20
        }
    ];

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

    saveNewProduct(product: IProduct) {
        console.log(`saveNewProduct:`, product);
    };

    saveNewShipment(shipment: IShipment) {
        console.log(`saveNewShipment:`, shipment);
    };
}
</script>

<style lang="scss" scoped>
    @import "@/scss/global.scss";
</style>
