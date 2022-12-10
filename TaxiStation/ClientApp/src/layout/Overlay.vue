<template>
	<v-overlay :value="overlay">
		<v-progress-circular indeterminate size="64"></v-progress-circular>
		שלום
	</v-overlay>
</template>
<script>
	export default {
		name: "Overlay",
		created: function () {
			setTimeout(() => {
				this.overlay = false;
			}, 2000);
		},
		data: () => ({
			overlay: true,
		}),
	};
</script>

