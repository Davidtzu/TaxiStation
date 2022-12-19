using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TaxiStation.Enums
{
    public enum pusherAction
    {
        searchTaxi = 1,
        getDrive = 2,
        foundTaxi = 3,
        noneTaxiAvailable = 4
    }
    public enum userType
    {
        user = 1,
        taxi = 2,
        station = 3
    }
}