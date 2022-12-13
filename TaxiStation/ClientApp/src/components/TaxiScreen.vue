<template>
    <div class="component-root">
        <v-card elevation="8" outlined class="card-settings">
            <v-card-title class="box-header">
                <div height="50px">חיפוש כללי</div>
            </v-card-title>
            <v-text-field outlined
                          v-model="search"
                          prepend-inner-icon="mdi-magnify"
                          label="הקלד ערך לחיפוש"
                          placeholder
                          class="my-input"
                          dense></v-text-field>
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
    import Pusher from 'pusher-js';
    import Action from '../enums/taxiStationEnums';

    export default {
        components: { AgGridVue },
        name: "TaxiScreen",
        data: () => ({
            gridApi: null,
            ColumnDefs: null,
            search: "",
            PusherData: { userID: "", taxiID: "30", action: Action.SearchTaxi },
            user: {ID:"30"}   
        }),
        created() {
            this.$http.post("/api/main/GetDriveHistory", this.user).then((response) => {
                console.log(response.data);
                this.$store.commit('SetRowData', response.data.DriveHistory);
            });
            this.subscribe();
            // this.$http.get("/api/main/addKafkaCunsomer").then((response) => {
            //     console.log(response.data);
            // });

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
            refreshData(){
                this.$http.post("/api/main/InsertDrive", {taxiID: this.user.ID}).then((response) => {
                        if(response.data === true){
                            this.$http.post("/api/main/GetDriveHistory", this.user).then((response) => {
                            this.$store.commit('SetRowData', response.data.DriveHistory);
                            }); 
                        }
                        else{                                
                            this.$swal.fire({
                                title: "שגיאה",
                                icon: "error",
                                text: "אירעה שגיאה בהכנסת הנסיעה אל מאגר הנתונים."
                            }); 
                        }}).catch(function (err){
                            this.$swal.fire({
                                title: "שגיאה",
                                icon: "error",
                                text: "שגיאה: " + err + "\n" + "אירעה שגיאה באישור הנסיעה."
                            }); 
                        });
            },
            subscribe(){
                Pusher.logToConsole = true;
                const pusher = new Pusher('52a43643bf829d8624d0', {
                    cluster: 'us2'
                });

                const channel = pusher.subscribe('chat');
                channel.bind('message', data => {
                    if(data.action == Action.SearchTaxi){ //try to get drive
                        this.$swal.fire({
                            title: "?האם ברצונך לקבל נסיעה",
                            icon: "info",
                            showDenyButton: true,
                            confirmButtonText: "כן",
                            denyButtonText: "לא"
                        }).then((result) =>{
                            if(result.isConfirmed){
                                this.$http.post("/api/main/Messages", {userID: "", taxiID:this.PusherData.taxiID, action: Action.GetDrive}).then((response) => {
                                });
                            }
                        }).catch(function (err){
                            this.$swal.fire({
                                title: "שגיאה",
                                icon: "error",
                                text: "שגיאה: " + err + "\n" + "אירעה שגיאה בקבלת הנסיעה."
                            });
                        });
                    }
                    else if(data.userID === this.user.ID && data.action == Action.foundTaxi){ //response to driver - got the drive
                        this.$swal.fire({
                                title: "קבלת הנסיעה",
                                text: "!הנסיעה התקבלה",
                                icon: "success",
                                timer: 3000
                            });
                            setInterval(this.refreshData, 10000);

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