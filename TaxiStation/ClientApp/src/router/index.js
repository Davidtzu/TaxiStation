import Vue from 'vue'
import VueRouter from 'vue-router'
import MainAppScrean from '../components/MainAppScrean'
import CustomerScreen from '../components/CustomerScreen'
import TaxiScreen from '../components/TaxiScreen'


Vue.use(VueRouter)

const routes = [
	{
		path: '/',
		name: 'MainAppScreen',
		component: MainAppScrean,
	},
	{
		path: '/CustomerScreen',
		name: 'CustomerScreen',
		component: CustomerScreen,
	},
	{
		path: '/TaxiScreen',
		name: 'TaxiScreen',
		component: TaxiScreen,
	}

]

const router = new VueRouter({
	routes
})


export default router
