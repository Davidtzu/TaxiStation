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
                היסטוריית נסיעות שלי
                <label class="mx-1">|</label>
                <v-card-actions>
                    <v-btn outlined x-small @click="SearchTaxi()" class="btnClass">
                        <v-icon>mdi-car</v-icon>חיפוש נסיעה
                    </v-btn>
                </v-card-actions>
            </v-card-title>

            <ag-grid-vue class="ag-theme-balham ag-grid-size"
                         :columnDefs="ColumnDefs"
                         :defaultColDef="defaultColDef"
                         :rowData="RowData"
                         :enableRtl="true"
                         @grid-ready="OnGridReady">
            </ag-grid-vue>
        </v-card>
    </div>
</template>


<script>
    import { AgGridVue } from 'ag-grid-vue';
    import Pusher from 'pusher-js';
    import Action from '../enums/actionEnum.js';
    export default {
        components: { AgGridVue },
        name: "CustomerScreen",
        data: () => ({
            gridApi: null,
            ColumnDefs: null,
            search: "",
            PusherData: { id: "3", action: Action.searchTaxi },
            id: "3",
            defaultColDef: {
                sortable: true,
                resizable: true,
            },
            rowData:null,
            pusherAppKey:null
        }),
        created() {
            this.$http.post("/api/main/GetDriveHistoryByUser", {id: this.id}).then((response) => {
                this.rowData = response.data.driveHistory;
                this.pusherAppKey = response.data.pusherAppKey;
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
                return this.rowData;
            }
        },
        methods: {
            SizeToFit() {
                this.gridApi.sizeColumnsToFit();
            },
            OnGridReady(params) {
                this.gridApi = params.api;
                this.gridApi.sizeColumnsToFit();
            },
            RefreshData() {
                setTimeout(() => {
                    this.$http.post("/api/main/GetDriveHistoryByUser", {id: this.id}).then((response) => {
                        if (response && response.data) {
                            this.rowData = response.data.driveHistory;
                        }
                    })
                }, 2000);
            },
            Subscribe() {
                Pusher.logToConsole = true;
                const pusher = new Pusher(this.pusherAppKey, {
                    cluster: 'us2'
                });

                const channel = pusher.subscribe('chat');
                channel.bind('message', data => {
                    if (data.action == Action.foundTaxi && data.userID === this.id) {
                        this.$swal.fire({
                            title: "נמצא נהג",
                            text: "הנהג בדרך אליך",
                            icon: "info",
                            timer: 3000
                        });
                        this.RefreshData();
                    }
                    else if (data.action == Action.noneTaxiAvailable && data.userID === this.id) {
                        this.$swal.fire({
                            title: "לא נמצא נהג",
                            text: "אנא נסה שנית מאוחר יותר",
                            icon: "info",
                            timer: 3000
                        });
                    }

                });
            },
            SearchTaxi() {
                this.Subscribe();
                this.$http.post("/api/main/MessagesFromUser", this.PusherData).then((response) => {
                    if(response.data === false){
                        this.$swal.fire({
                            title: "שגיאה",
                            icon: "error",
                            text: "שגיאה: " + err + "\n" + "אירעה שגיאה בחיפוש מונית."
                        });
                        console.log("ERROR in MessagesFromUser");
                    }
                });
            }
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
                        return this.$moment(params.data.startDate).format('HH:mm ,D/MM/YYYY');
                    }
                },
                {
                    field: "finishDate",
                    headerName: 'סיום נסיעה',
                    valueGetter: params => {
                        return this.$moment(params.data.finishDate).format('HH:mm ,D/MM/YYYY');
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
        }
    }
</script>

<style scoped>
    .btnClass{
    color:#28a745
    }
</style>