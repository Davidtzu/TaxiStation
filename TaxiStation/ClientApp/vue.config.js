module.exports = {
	productionSourceMap: false,
	publicPath: './',
	lintOnSave: false,
	runtimeCompiler: true,
	configureWebpack: {
		"devtool": 'source-map',
	},
	"transpileDependencies": [
		"vuetify"
	],
}