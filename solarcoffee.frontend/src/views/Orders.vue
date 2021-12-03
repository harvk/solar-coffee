<template>
    <div class="orders-container">
        <div id="orders-title">
            <h1>Sales Orders</h1>
        </div>

        <hr />
        
        <table v-if="orders.length" id="sales-orders" class="table">
            <thead>
                <tr>
                    <th>Customer Id</th>
                    <th>Order Id</th>
                    <th>Order Total</th>
                    <th>Order Status</th>
                    <th>Mark Complete</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="order in orders" :key="order.id">
                    <td>{{ order.customer.id }}</td>
                    <td>{{ order.id }}</td>
                    <td>{{ getTotal(order) | price }}</td>
                    <td :class="{ 'green': order.isPaid }">{{ getStatus(order.isPaid) }}</td>
                    <td>
                        <div v-if="!order.isPaid" class="lni lni-checkmark-circle order-complete green" 
                            @click="markComplete(order.id)"></div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { OrderService } from "@/services/order-service";
import { ISalesOrder } from "@/types/SalesOrder";

const orderService = new OrderService();

@Component({
    name: "Orders"
})

export default class Orders extends Vue {
    orders: ISalesOrder[] = [];

    getTotal(order: ISalesOrder) {
        return order.salesOrderItems.reduce(
            (total, currentItem) => total + (currentItem['product']['price'] * currentItem['quantity']), 0
        );
    }

    getStatus(isPaid: boolean) {
        return isPaid ? "Paid in full" : "Unpaid";
    }

    async markComplete(orderId: number) {
        await orderService.markOrderComplete(orderId);
        this.initialize();
    }
    
    async initialize() {
        this.orders = await orderService.getOrders();
    }

    async created() {
        await this.initialize();
    }
}
</script>

<style lang="scss" scoped>
    @import "@/scss/global.scss";
    
    #sales-orders {
        text-align: center;
    }

    .green {
        font-weight: bold;
        color: $solar-green;
    }

    .order-complete {
        cursor: pointer;
        text-align: center;
    }
</style>
