import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify';
import Vuetify from 'vuetify/lib'
import router from './router'
import he from 'vuetify/es5/locale/he'
import axios from 'axios'
import '@mdi/font/css/materialdesignicons.css'
import VueSweetalert2 from 'vue-sweetalert2'
import Vuex from 'vuex'
import VueMoment from 'vue-moment'
import Pusher from 'pusher-js';


import "ag-grid-enterprise";
/*import { LicenseManager } from "ag-grid-enterprise";*/
/*LicenseManager.setLicenseKey(window.ITGlobalConfig.agGridLicenseKey);*/
import '../node_modules/ag-grid-community/dist/styles/ag-grid.css';
import '../node_modules/ag-grid-community/dist/styles/ag-theme-alpine.css';
import '../node_modules/ag-grid-community/dist/styles/ag-theme-balham.css';
import '../node_modules/ag-grid-community/dist/styles/ag-theme-bootstrap.css';


Vue.prototype.$http = axios;
Vue.use(Vuex);
Vue.use(axios);
Vue.use(VueSweetalert2);
Vue.use(Pusher);
Vue.use(VueMoment);




const store = new Vuex.Store({
	state: {
		SearchUrl: null,
		FilesUrl: null,
		rowData: null
	},
	mutations: {
		SetSearchUrl(state, SearchUrl) {
			state.SearchUrl = SearchUrl;
		},
		SetFilesUrl(state, FilesUrl) {
			state.FilesUrl = FilesUrl;
		},
		SetRowData(state, RowData) {
			state.rowData = RowData;
		}
	},


})

Vue.config.productionTip = false

export default new Vuetify({
	rtl: true,
})
new Vue({
	vuetify,
	store: store,
	icons: {
		iconfont: 'mdi'
	},
	router,
	lang: {
		locales: { he },
		current: 'he',
	},
	render: h => h(App)
}).$mount('#app')



