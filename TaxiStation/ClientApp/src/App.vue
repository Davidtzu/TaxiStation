<template>
	<v-app>
		<AppBar />
		<Menu />
		<!--<Overlay />-->
		<MainContent />
		<!--<Footer />-->
	</v-app>
</template>
<script>
	import Overlay from "./layout/Overlay";
	import AppBar from "./layout/AppBar";
	import MainContent from "./layout/MainContent";
	import Menu from "./layout/Menu";
	import Footer from "./layout/Footer";

	export default {
		name: "App",
		components: { AppBar, MainContent, Menu, Footer, Overlay }
	};
</script>

