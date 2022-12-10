<template>
	<span>
		<v-navigation-drawer dark
							 :width="200"
							 app
							 clipped
							 color="#163a58"
							 right
							 style="box-shadow: rgba(0, 0, 0, 0.5) 0px 1px 5px, rgba(0, 0, 0, 0.5) 0px -1px 5px;">
			<template v-slot:prepend>
				<v-list-item two-line style="justify-content: center">
					<v-list-item-content style="text-align: right">
						<v-list-item-title>שלום  </v-list-item-title>
					</v-list-item-content>
				</v-list-item>
			</template>
			<v-divider></v-divider>
			<v-list dense>
				<v-list-item v-for="item in items"
							 :key="item.title"
							 link
							 :to="item.route">
					<v-list-item-icon>
						<v-icon>{{ item.icon }}</v-icon>
					</v-list-item-icon>
					<v-list-item-content style="text-align: right">
						<v-list-item-title>{{item.title}}</v-list-item-title>
					</v-list-item-content>
				</v-list-item>
			</v-list>
		</v-navigation-drawer>
	</span>
</template>
<script>
	export default {
		name: "Menu",
		data: () => ({
			items: [
				{ title: "מסך ראשי", icon: "mdi-account-group-outline", route: "/" },
                { title: "מסך לקוח", icon: "mdi-account-cash-outline", route: "/CustomerScreen" },
                { title: "מסך מונית", icon: "mdi-folder-account-outline", route: "/TaxiScreen" }
				
			],
		})
	};

</script>
