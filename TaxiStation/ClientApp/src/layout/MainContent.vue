<template>
	<v-main style="display: flex; flex-direction: column;background-color: #c3c3c3">
		<v-container class="containerMainContent" fluid>
			<router-view background-color="#efefef" style="height: 100%; display:flex"></router-view>
		</v-container>
	</v-main>
</template>

<script>

	export default {
		name: "MainContent"
	};
</script>

<style scoped>
	.containerMainContent {
		height: 100%;
		background-color: #c3c3c3;
	}
</style>