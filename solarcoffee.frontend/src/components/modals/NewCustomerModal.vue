<template>
    <solar-modal>
        <template v-slot:header>Add New Customer</template>

        <template v-slot:body>
            <ul class="new-customer">
                <li>
                    <label for="first-name">First Name</label>
                    <input type="text" id="first-name" v-model="customer.firstName" />
                </li>
                <li>
                    <label for="last-name">Last Name</label>
                    <input type="text" id="last-name" v-model="customer.lastName" />
                </li>
                <li>
                    <label for="address-1">Address 1</label>
                    <input type="text" id="address-1" v-model="customer.primaryAddress.addressLine1" />
                </li>
                <li>
                    <label for="address-2">Address 2</label>
                    <input type="text" id="address-2" v-model="customer.primaryAddress.addressLine2" />
                </li>
                <li>
                    <label for="city">City</label>
                    <input type="text" id="city" v-model="customer.primaryAddress.city" />
                </li>
                <li>
                    <label for="state">State</label>
                    <input type="text" id="state" v-model="customer.primaryAddress.state" />
                </li>
                <li>
                    <label for="postal-code">Postal Code</label>
                    <input type="text" id="postal-code" v-model="customer.primaryAddress.postalCode" />
                </li>
                <li>
                    <label for="country">Country</label>
                    <input type="text" id="country" v-model="customer.primaryAddress.country" />
                </li>
            </ul>
        </template>

        <template v-slot:footer>
            <solar-button type="button" @button:click="save" aria-label="Save new customer">Save</solar-button>
            <solar-button type="button" @button:click="close" aria-label="Close modal">Close</solar-button>
        </template>
    </solar-modal>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from  "@/components/SolarButton.vue";
import SolarModal from "@/components/modals/SolarModal.vue";
import { ICustomer } from "@/types/Customer";

@Component({
    name: "NewCustomerModal",
    components: { SolarButton, SolarModal }
})

export default class NewCustomerModal extends Vue {
    customer: ICustomer = {
      createdOn: new Date(),
      updatedOn: new Date(),
      firstName: "",
      lastName: "",
      // @ts-ignore
      primaryAddress: { }
    };

    save() {
        this.$emit("save:customer", this.customer);
    }

    close() {
        this.$emit("close");
    }
}
</script>

<style lang="scss" scoped>
    .new-customer {
        display: flex;
        flex-wrap: wrap;
        list-style: none;
        padding: 0;
        margin: 0;

        input {
            width: 80%;
            height: 1.8rem;
            margin: 0.8rem;
            margin-right: 2rem;
            font-size: 1.1rem;
            line-height: 1.3rem;
            padding: 0.2rem;
            color: #555;
        }

        label {
            font-weight: bold;
            margin: 0.8rem;
            display: block;
        }
    }
</style>
