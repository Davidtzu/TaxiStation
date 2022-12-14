<template>
    <div class="component-root">
        <v-card elevation="8" outlined class="card-settings">
            <v-card-title class="box-header">
                חיפוש כללי
            </v-card-title>
            <v-text-field outlined
                          v-model="search"
                          prepend-inner-icon="mdi-magnify"
                          label="הקלד ערך לחיפוש"
                          placeholder
                          class="my-input"
                          dense></v-text-field>
        </v-card>
        <v-card elevation="8" outlined class="card-settings historySize">
            <v-card-title class="box-header">
                היסטוריית נסיעות
                <label class="mx-1">|</label>
                <v-card-actions>
                    <v-btn outlined x-small @click="ExortToCsc" color="#28a745">
                        <v-icon>mdi-microsoft-excel</v-icon>יצוא לאקסל
                    </v-btn>
                </v-card-actions>
            </v-card-title>

            <ag-grid-vue class="ag-theme-balham ag-grid-size"
                         :columnDefs="ColumnDefs"
                         :defaultColDef="defaultColDef"
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
            user: {ID:"taxiStation"},
            defaultColDef: {
                sortable: true,
                resizable: true,
            },
        }),
        created() {
            this.$http.post("/api/main/GetDriveHistory", this.user).then((response) => {
                console.log(response.data);
                this.$store.commit('SetRowData', response.data.DriveHistory);
            });
            window.addEventListener("resize", this.SizeToFit);
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
            SizeToFit(){
                this.gridApi.sizeColumnsToFit();
            },
            onGridReady(params) {
                this.gridApi = params.api;
                this.gridApi.sizeColumnsToFit();
            },
            ExortToCsc() {
                this.gridApi.exportDataAsExcel();
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
                        return this.$moment(params.data.startDate).format('HH:MM ,D/MM/YYYY');
                    }
                },
                {
                    field: "finishDate",
                    headerName: 'סיום נסיעה',
                    valueGetter: params => {
                        return this.$moment(params.data.finishDate).format('HH:MM ,D/MM/YYYY');
                    }
                },
                {
                    field: "cost",
                    headerName: 'מחיר נסיעה',
                },
                {
                    field: "pickUp",
                    headerName: 'נקודות איסוף',
                },
                {
                    field: "takenDown",
                    headerName: 'נקודות הורדה',
                }
            ];
            
        },
    };
</script>

<style>
    .my-input {
        height: 50px !important;
    }

    .ag-grid-size {
        width:100%;
        height: 90%;
    }

    .component-root {
        display: flex;
        width: 100%;
        flex-direction: column;
        height: calc(100vh - 110px);
        background-color: whitesmoke;
        flex: 1;
    }

    .card-settings {
        background-color: white;
        box-shadow: 0 1px 8px 0 rgba(0, 0, 0, 0.2), 0 3px 4px 0 rgba(0, 0, 0, 0.14), 0 3px 3px -2px rgba(0, 0, 0, 0.12);
        margin: 10px;
        padding: 10px;
    }

    .box-header {
        margin: -10px;
        margin-bottom: 5px;
        background: -moz-linear-gradient(top, #bccecf 1%, white 100%);
        background: -webkit-linear-gradient(top, #bccecf 1%, white 100%);
        padding: 4px 10px;
        font-weight: bold;
        font-size: 14px;
        -ms-user-select: none;
        user-select: none;
        line-height: 1.9;
    }

    .v-btn {
        text-overflow: ellipsis !important;
        font-size: 16px !important;
        height: 30px !important;
        margin-right: 5px;
        display: inline-block;
        text-align: center;
        white-space: nowrap;
        vertical-align: middle;
    }
    .historySize{
        height: 100%;
    }
    .label {
        display: inline-block;
        margin-bottom: 0.5rem;
    }

    .v-text-field.v-input.__control.v-inpt__slot {
        min-height: auto !important;
        display: flex !important;
        align-items: center !important;
    }

    .v-data-table.elevation-8.v-data-table--fixed-height.theme--light {
        height: 0px;
    }

    .swal2-title {
        font-family: Arial, Helvetica, sans-serif;
    }

    .swal2-content {
        font-family: Arial, Helvetica, sans-serif;
    }

    .swal2-confirm {
        font-family: Arial, Helvetica, sans-serif;
    }
</style>




