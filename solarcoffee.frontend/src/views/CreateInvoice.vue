<template>
    <div class="create-invoice-container">
        <div id="create-invoice-title">
            <h1>Create Invoice Dashboard</h1>
        </div>

        <hr />
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from "@/components/SolarButton.vue";

@Component({
    name: "CreateInvoice",
    components: { SolarButton }
})

export default class CreateInvoice extends Vue {
    
}
</script>

<style scoped>

</style>
