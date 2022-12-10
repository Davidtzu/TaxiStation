<template>
    <div class="component-root">
        <v-card elevation="8" outlined class="card-settings" height="calc(75vh - 500px)">
            <v-card-title class="box-header">
                פעולות כלליות
                <label class="mx-1">|</label>
            </v-card-title>
        </v-card>
        <v-card elevation="8" outlined class="card-settings" height="calc(100vh - 170px)">
            <v-card-title class="box-header">
                היסטוריית נסיעות שלי
                <label class="mx-1">|</label>

            </v-card-title>

            <ag-grid-vue class="ag-theme-balham ag-grid-size"
                         :columnDefs="ColumnDefs"
                         :rowData="RowData"
                         :enableRtl="true"
                         @grid-ready="onGridReady">
            </ag-grid-vue>
        </v-card>
    </div>
</template>

<script>
    import { AgGridVue } from 'ag-grid-vue';

    export default {
        components: { AgGridVue },
        name: "MainTable",
        data: () => ({
            gridApi: null,
            ColumnDefs: null,
            search: "",
            text: { userName: "david", message: "hi" }
            
        }),


        created() {
            this.$http.get("/api/main/GetDriveHistory").then((response) => {
                console.log(response.data);
                this.$store.commit('SetRowData', response.data.DriveHistory);
            });

        },
        watch: {
            search(newSearch, OldSearch) {
                this.gridApi.setQuickFilter(newSearch);
            }
        },
        computed: {
            RowData() {
                return this.$store.state.rowData;
            }
        },
        methods: {
            onGridReady(params) {
                this.gridApi = params.api;
                this.gridApi.sizeColumnsToFit();
            },


            message(){
                this.$http.post("/api/main/Messages", this.text).then((response) => {
                console.log(response.data);
            });
            }

        },
        onMounted() {
            Pusher.logToConsole = true;
            const pusher = new Pusher('52a43643bf829d8624d0', {
                cluster: 'us2'
            });

            const channel = pusher.subscribe('chat');
            channel.bind('message', data => {
                messages.value.push(data);
            });
        },
        beforeMount() {
            this.ColumnDefs = [
                {
                    field: "userFullName",
                    headerName: 'שם לקוח'
                },
                {
                    field: "userEmail",
                    headerName: 'אימייל לקוח'
                },
                {
                    field: "taxiDriverFullName",
                    headerName: 'שם נהג'
                },
                {
                    field: "taxiNumber",
                    headerName: 'מספר מונית'
                },
                {
                    field: "startDate",
                    headerName: 'התחלת נסיעה',
                    valueGetter: params => {
                        return this.$moment(params.data).format('HH:mm ,D/MM/YYYY');
                    }
                },
                {
                    field: "finishDate",
                    headerName: 'סיום נסיעה',
                    valueGetter: params => {
                        return this.$moment(params.data).format('HH:mm ,D/MM/YYYY');
                    }
                },
                {
                    field: "cost",
                    headerName: 'מחיר נסיעה',
                }
            ];
        }

    };
</script>

<style scoped>
    .ag-grid-size {
        width: 100%;
        height: 505px;
    }
</style>