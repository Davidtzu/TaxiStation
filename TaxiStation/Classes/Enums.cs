using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaxiStation.Classes
{
    public enum userType
    {
        users = 1,
        taxis = 2,
        taxiStation = 3
    }
    public enum PusherAction
    {
        SearchTaxi = 1,
        GetDrive = 2,
        foundTaxi = 3,
        noneTaxiAvailable = 4
    }

}