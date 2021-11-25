<template>
    <div class="create-invoice-container">
        <div id="create-invoice-title">
            <h1>Create Invoice</h1>
        </div>

        <hr />

        <div class="invoice-step" v-if="invoiceStep === 1">
            <h2>Step 1: Select Customer</h2>

            <div v-if="customers.length" class="invoice-step-detail">
                <label for="customer">Customer:</label>
                <select v-model="selectedCustomerId" id="customer" class="invoice-customers">
                    <option disabled value="">Please select a Customer</option>
                    <option v-for="c in customers" :key="c.id" :value="c.id">{{ c.firstName + " " + c.lastName }}</option>
                </select>
            </div>
        </div>

        <div class="invoice-step" v-if="invoiceStep === 2">
            <h2>Step 2: Create Order</h2>

            <div v-if="inventory.length" class="invoice-step-detail">
                <label for="product">Product:</label>
                <select v-model="newItem.product" id="product" class="invoice-line-item">
                    <option disabled value="">Please select a Product</option>
                    <option v-for="i in inventory" :key="i.product.id" :value="i.product">
                        {{ i.product.name }}
                    </option>
                </select>

                <label for="quantity">Quantity:</label>
                <input v-model="newItem.quantity" type="number" id="quantity" min="0" />
            </div>

            <div class="invoice-item-actions">
                <solar-button @button:click="addLineItem" :disabled="!newItem.product || !newItem.quantity">
                    Add Line Item
                </solar-button>
                <solar-button @button:click="finalizeOrder" :disabled="!lineItems.length">
                    Finalize Order
                </solar-button>
            </div>

            <div v-if="lineItems.length" class="invoice-order-list">
                <div class="running-total">
                    <h3>Running Total:</h3>
                    {{ runningTotal | price }}
                </div>

                <hr />

                <table class="table">
                    <thead>
                        <tr>
                            <th>Product</th>
                            <th>Description</th>
                            <th>Qty</th>
                            <th>Price</th>
                            <th>Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="lineItem in lineItems" :key="`index_${lineItem.product.id}`">
                            <td>{{ lineItem.product.name }}</td>
                            <td>{{ lineItem.product.description }}</td>
                            <td>{{ lineItem.quantity }}</td>
                            <td>{{ lineItem.product.price }}</td>
                            <td>{{ lineItem.product.price * lineItem.quantity | price }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="invoice-step" v-if="invoiceStep === 3">
            <h2>Step 3: Review and Submit</h2>
            <solar-button @button:click="submitInvoice">Submit Invoice</solar-button>
            
            <hr />
            
            <div id="invoice" class="invoice-step-detail" ref="invoice">
                <div class="invoice-logo">
                    <img src="../assets/images/solar-coffee-logo.jpg" alt="Solar Coffee Logo" id="img-logo">
                    <h3>1337 Solar Lane</h3>
                    <h3>St Somewhere, FL 30000</h3>
                    <h3>USA</h3>

                    <div v-if="lineItems.length" class="invoice-order-list">
                        <div class="invoice-header">
                            <h3>Invoice: {{ new Date() | humanizeDate }}</h3>
                            <h3>Customer: {{ selectedCustomer.firstName + " " + selectedCustomer.lastName }}</h3>
                            <h3>Address: {{ selectedCustomer.primaryAddress.addressLine1 }}</h3>
                            <h3 v-if="selectedCustomer.primaryAddress.addressLine2">
                                {{ selectedCustomer.primaryAddress.addressLine2 }}
                            </h3>
                            <h3>
                                {{ selectedCustomer.primaryAddress.city }},
                                {{ selectedCustomer.primaryAddress.state }}
                                {{ selectedCustomer.primaryAddress.postalCode }}
                            </h3>
                            <h3>{{ selectedCustomer.primaryAddress.country }}</h3>
                        </div>

                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Description</th>
                                    <th>Qty</th>
                                    <th>Price</th>
                                    <th>Subtotal</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="lineItem in lineItems" :key="`index_${lineItem.product.id}`">
                                    <td>{{ lineItem.product.name }}</td>
                                    <td>{{ lineItem.product.description }}</td>
                                    <td>{{ lineItem.quantity }}</td>
                                    <td>{{ lineItem.product.price }}</td>
                                    <td>{{ lineItem.product.price * lineItem.quantity | price }}</td>
                                </tr>
                                <tr>
                                    <th colspan="4"></th>
                                    <th>Grand Total</th>
                                </tr>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td colspan="4" class="due">Balance due:</td>
                                    <td class="price-final">{{ runningTotal | price }}</td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <hr />
        
        <div class="invoice-step-actions">
            <solar-button @button:click="prev" :disabled="!canGoPrev">Previous</solar-button>
            <solar-button @button:click="next" :disabled="!canGoNext">Next</solar-button>
            <solar-button @button:click="startOver">Start Over</solar-button>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from "@/components/SolarButton.vue";
import { IInvoice, ILineItem } from "@/types/Invoice";
import { ICustomer } from "@/types/Customer";
import { IProductInventory } from "@/types/Product";
import { CustomerService } from "@/services/customer-service";
import { InventoryService } from "@/services/inventory-service";
import { InvoiceService } from "@/services/invoice-service";
import jsPDF from "jspdf";
import html2canvas from "html2canvas";

const customerService = new CustomerService();
const inventoryService = new InventoryService();
const invoiceService = new InvoiceService();

@Component({
    name: "CreateInvoice",
    components: { SolarButton }
})

export default class CreateInvoice extends Vue {
    invoiceStep = 1;
    
    invoice: IInvoice = {
        createdOn: new Date(),
        updatedOn: new Date(),
        lineItems: []
    };

    customers: ICustomer[] = [];
    selectedCustomerId: number = 0;
    inventory: IProductInventory[] = [];
    lineItems: ILineItem[] = [];
    newItem: ILineItem = { product: undefined, quantity: 0 };

    get canGoPrev() {
        return this.invoiceStep !== 1;
    }
    
    get canGoNext() {
        if (this.invoiceStep === 1) {
            return this.selectedCustomerId !== 0;
        }

        if (this.invoiceStep === 2) {
            return this.lineItems.length;
        }

        if (this.invoiceStep === 3) {
            return false;
        }

        return false;
    }

    get runningTotal() {
        return this.lineItems.reduce(
            (total, currentItem) => total + (currentItem['product']['price'] * currentItem['quantity']), 0
        );
    }

    get selectedCustomer() {
        return this.customers.find(c => c.id === this.selectedCustomerId);
    }

    prev(): void {
        if (this.invoiceStep === 1) {
            return;
        }

        this.invoiceStep -= 1;
    }

    next(): void {
        if (this.invoiceStep === 3 || this.selectedCustomerId === 0) {
            return;
        }

        this.invoiceStep += 1;
    }

    startOver(): void {
        this.invoice = { lineItems: [] };
        this.invoiceStep = 1;
    }

    addLineItem() {
        let newItem: ILineItem = {
            product: this.newItem.product,
            // quantity: parseInt(this.newItem.quantity),
            quantity: Number(this.newItem.quantity)
        }

        let existingItems = this.lineItems.map(item => item.product.id);

        //line item already exists, increase quantity
        if (existingItems.includes(newItem.product.id)) {
            let lineItem = this.lineItems.find(
                item => item.product.id === newItem.product.id
            );
            lineItem.quantity = Number(lineItem.quantity) + newItem.quantity;
        } 
        // add new item to list
        else {
            this.lineItems.push(this.newItem);
        }

        this.newItem = { product: undefined, quantity: 0 };
    }

    finalizeOrder() {
        this.invoiceStep = 3;
    }

    downloadPdf() {
        let pdf = new jsPDF("p", "pt", "a4", true);
        let invoice = document.getElementById("invoice");
        let width = this.$refs.invoice.clientWidth;
        let height = this.$refs.invoice.clientHeight;

        html2canvas(invoice).then(canvas => {
            let image = canvas.toDataURL("image/jpg");
            pdf.addImage(image, "JPG", 0, 0, width * 0.55, height * 0.55);
            pdf.save("invoice");
        });
    }

    async initialize(): Promise<void> {
        // customerService.getCustomers().then(res => this.customers = res).catch(err => console.log(err));
        this.customers = await customerService.getCustomers();
        this.inventory = await inventoryService.getInventory();
    }

    async submitInvoice(): Promise<void> {
        this.invoice = {
            customer: this.selectedCustomerId,
            lineItems: this.lineItems
        };

        await invoiceService.MakeNewInvoice(this.invoice);
        this.downloadPdf();
        await this.$router.push("/orders");
    }

    async created() {
        await this.initialize();
    }
}
</script>

<style lang="scss" scoped>
    @import "@/scss/global.scss";

    .invoice-step {

    }

    .invoice-step-detail {
        margin: 1.2rem;
    }

    .invoice-step-actions {
        display: flex;
        width: 100%;
    }

    .invoice-order-list {
        margin-top: 1.2rem;
        padding: 0.8rem;

        .line-item {
            display: flex;
            border-bottom: 1px dashed #ccc;
            padding: 0.8rem;
        }

        .item-col {
            flex-grow: 1;
        }
    }

    .invoice-item-actions {
        display: flex;
    }

    .price-pre-tax {
        font-weight: bold;
    }

    .price-final {
        font-weight: bold;
        color: $solar-green;
    }

    .due {
        font-weight: bold;
    }

    .invoice-header {
        width: 100%;
        margin-bottom: 1.2rem;
    }

    .invoice-logo {
        padding-top: 1.4rem;
        text-align: center;

        img {
            width: 280px;
        }
    }
</style>
