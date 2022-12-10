<template>
    <div class="component-root">
        <v-card elevation="8" outlined class="card-settings" height="calc(75vh - 500px)">

            <v-card-title class="box-header" >
                פעולות כלליות
                <label class="mx-1">|</label>
                <v-card-actions>
                    <v-btn outlined x-small @click="searchTaxi" color="#28a745">
                        <v-icon>mdi-car</v-icon>חיפוש נסיעה
                    </v-btn>
                </v-card-actions>
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
                         :enableRtl ="true"
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
            socketInstance: null
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
            searchTaxi() {
                this.$http.get("/api/main/StartSocket").then((response) => {
                    const message = {
                        id: 1,
                        text: "asd"
                    }
                    this.socketInstance.emit("message", message);

                });

                this.socketInstance.on(
                    "message:received", (data) =>{
                        //do something
                    }
                )


            },
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