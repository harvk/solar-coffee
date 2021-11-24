<template>
    <div class="orders-container">
        <div id="orders-title">
            <h1>Orders Dashboard</h1>
        </div>

        <hr />
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from "@/components/SolarButton.vue";

@Component({
    name: "Orders",
    components: { SolarButton }
})

export default class Orders extends Vue {
    
}
</script>

<style scoped>

</style>
