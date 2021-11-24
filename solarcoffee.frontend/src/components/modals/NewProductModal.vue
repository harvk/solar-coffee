<template>
    <solar-modal>
        <template v-slot:header>Add New Product</template>

        <template v-slot:body>
            <ul class="new-product">
                <li>
                    <label for="is-taxable">Is this product taxable?</label>
                    <input type="checkbox" id="is-taxable" v-model="newProduct.isTaxable">
                </li>
                <li>
                    <label for="product-name">Name</label>
                    <input type="text" id="product-name" v-model="newProduct.name">
                </li>
                <li>
                    <label for="product-desc">Description</label>
                    <input type="text" id="product-desc" v-model="newProduct.description">
                </li>
                <li>
                    <label for="product-price">Price (USD)</label>
                    <input type="number" id="product-price" v-model="newProduct.price">
                </li>
            </ul>
        </template>

        <template v-slot:footer>
            <solar-button type="button" @click.native="save" aria-label="Save new item">Save Product</solar-button>
            <solar-button type="button" @click.native="close" aria-label="Close modal">Close</solar-button>
        </template>
    </solar-modal>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from "@/components/SolarButton.vue";
import SolarModal from "./SolarModal.vue";
import { IProduct } from "@/types/Product";

@Component({
    name: "NewProductModal",
    components: { SolarButton, SolarModal }
})

export default class NewProductModal extends Vue {
    newProduct: IProduct = {
        createdOn: new Date(),
        updatedOn: new Date(),
        id: 0,
        description: "",
        isTaxable: false,
        isArchived: false,
        name: "",
        price: 0
    };

    close() {
        this.$emit("close");
    };

    save() {
        this.$emit("save:product", this.newProduct);
    };
}
</script>

<style lang="scss" scoped>
    .new-product {
        list-style: none;
        padding: 0;
        margin: 0;

        input {
            width: 100%;
            height: 1.8rem;
            margin-bottom: 1.2rem;
            font-size: 1.1rem;
            line-height: 1.3rem;
            padding: 0.2rem;
            color: #555;
        }

        input[type=checkbox] {
            width: 5%;
            height: 1.8rem;
            margin-bottom: 1.2rem;
            font-size: 1.1rem;
            line-height: 1.3rem;
            padding: 0.2rem;
            color: #555;
        }

        label {
            font-weight: bold;
            display: block;
            margin-bottom: 0.3rem;
        }
    }
</style>
