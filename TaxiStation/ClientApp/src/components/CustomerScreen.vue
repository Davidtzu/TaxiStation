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
                    <v-btn outlined x-small @click="searchTaxi" color="#28a745">
                        <v-icon>mdi-car</v-icon>חיפוש נסיעה
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
    import Pusher from 'pusher-js';
    import { onMounted } from 'vue'
    import Action from "./../enums/taxiStationEnums";

    export default {
        components: { AgGridVue },
        name: "CustomerScreen",
        data: () => ({
            gridApi: null,
            ColumnDefs: null,
            search: "",
            PusherData: { userID: "3", taxiID: "", action: Action.SearchTaxi },
            user: {ID:"3"},
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
            this.subscribe();
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
            refreshData(){
                this.$http.post("/api/main/GetDriveHistory", this.user).then((response) => {
                                this.$store.commit('SetRowData', response.data.DriveHistory);
                            });
            },
            subscribe(){
                Pusher.logToConsole = true;
                const pusher = new Pusher('52a43643bf829d8624d0', {
                    cluster: 'us2'
                });

                const channel = pusher.subscribe('chat');
                channel.bind('message', data => {
                        if(data.action == Action.foundTaxi){
                            this.$swal.fire({
                                title: "נמצא נהג",
                                text: "הנהג בדרך אליך",
                                icon: "info",
                                timer: 3000
                            });
                            this.refreshData();
                        }
                        else if(data.action == Action.noneTaxiAvailable){
                            this.$swal.fire({
                                    title: "לא נמצא נהג",
                                    text: "אנא נסה שנית מאוחר יותר",
                                    icon: "info",
                                    timer: 3000
                                });
                        }

                });
            },
            searchTaxi() {
                this.$http.post("/api/main/MessagesFromUser", this.PusherData).then((response) => {
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
        }
    }
</script>
