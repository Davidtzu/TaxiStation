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
                    <v-btn v-if="!rdyForDrive" outlined x-small @click="Toggle()" class="btnClassNotActive">
                        <v-icon>mdi-car</v-icon>זמין לנסיעה
                    </v-btn>
                    <v-btn v-else outlined x-small @click="Toggle()" class="btnClassActive">
                        <v-icon>mdi-car</v-icon>ביטול זמינות
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
        name: "TaxiScreen",
        data: () => ({
            gridApi: null,
            ColumnDefs: null,
            search: "",
            PusherData: {id: "20", action: Action.searchTaxi },
            id: "20",
            defaultColDef: {
                sortable: true,
                resizable: true,
            },
            isTaxAvailable: false,
            rowData:null,
            requestedUser:null,
            pusherAppKey:null,
            rdyForDrive:false
        }),
        created() {
            this.$http.post("/api/main/GetDriveHistoryByTaxi", {id: this.id}).then((response) => {
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
            Toggle(){
              this.rdyForDrive = !this.rdyForDrive;
              if(this.rdyForDrive){
                this.Subscribe();
                this.$http.get("/api/main/TaxiSubscribeAsConsumer").then((response) => {
                    if (response.data === true) {
                        this.isTaxAvailable = true;
                    }
                    else{
                        this.$swal.fire({
                            title: "שגיאה",
                            icon: "error",
                            text: ".אירעה שגיאה בהרשמה אל המוניות הזמינות"
                        });
                        this.isTaxAvailable = false;
                        this.rdyForDrive = !this.rdyForDrive;
                    }
                })
              } 
              else{
                this.isTaxAvailable = false;
              } 
            },
            SizeToFit() {
                this.gridApi.sizeColumnsToFit();
            },
            OnGridReady(params) {
                this.gridApi = params.api;
                this.gridApi.sizeColumnsToFit();
            },
            RefreshData() {
                this.$http.post("/api/main/InsertDrive", { taxiID: this.id, userID: this.requestedUser }).then((response) => {
                    if (response.data === true) {
                        setTimeout(() => {
                            this.$http.post("/api/main/GetDriveHistoryByTaxi", {id: this.id}).then((response) => {
                                if (response && response.data) {
                                    this.rowData = response.data.driveHistory;
                                    this.isTaxAvailable = true;
                                }
                            })
                        }, 2000);
                    }
                    else {
                        this.$swal.fire({
                            title: "שגיאה",
                            icon: "error",
                            text: "אירעה שגיאה בהכנסת הנסיעה אל מאגר הנתונים."
                        });
                    }
                }).catch(function (err) {
                    this.$swal.fire({
                        title: "שגיאה",
                        icon: "error",
                        text: "שגיאה: " + err + "\n" + "אירעה שגיאה באישור הנסיעה."
                    });
                });
            },
            Subscribe() {
                Pusher.logToConsole = true;
                const pusher = new Pusher(this.pusherAppKey, {
                    cluster: 'us2'
                });
                const channel = pusher.subscribe('chat');
                channel.bind('message', data => {
                    if (data.action == Action.searchTaxi && this.isTaxAvailable) { //try to get drive
                        this.$swal.fire({
                            title: "?האם ברצונך לקבל נסיעה",
                            icon: "info",
                            showDenyButton: true,
                            confirmButtonText: "כן",
                            denyButtonText: "לא",
                            timer: 7000
                        }).then((result) => {
                            if (result.isConfirmed) {
                                this.isTaxAvailable = false;
                                this.requestedUser = data.id;
                                this.$http.post("/api/main/MessagesFromTaxi", {taxiId: this.id, userID: this.requestedUser,action: Action.getDrive }).then((response) => {
                                    if(response.data === false){
                                        this.$swal.fire({
                                            title: "שגיאה",
                                            icon: "error",
                                            text: "שגיאה: " + err + "\n" + "אירעה שגיאה בקבלת הנסיעה."
                                        });
                                        console.log("ERROR in PusherMessagesFromTaxi");
                                    }
                                });
                            }
                        }).catch(function (err) {
                            this.$swal.fire({
                                title: "שגיאה",
                                icon: "error",
                                text: "שגיאה: " + err + "\n" + "אירעה שגיאה בקבלת הנסיעה."
                            });
                        });
                    }
                    else if (data.taxiID === this.id && data.action == Action.foundTaxi && !this.isTaxAvailable) {
                        this.$swal.fire({
                            title: "קבלת הנסיעה",
                            text: "!הנסיעה התקבלה",
                            icon: "success",
                            timer: 3000
                        });
                        this.RefreshData();
                    }
                    else if (data.taxiID !== this.id && data.action == Action.foundTaxi && !this.isTaxAvailable) {
                        this.$swal.fire({
                            title: "מידע אודות הנסיעה",
                            text: "מונית קרובה יותר אל הלקוח קיבלה את הנסיעה",
                            icon: "info",
                            timer: 4000
                        });
                        this.isTaxAvailable = true;
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
    };
</script>

<style scoped>
    .btnClassNotActive{
    color:green
    }
    .btnClassActive{
    color:red
    }
</style>