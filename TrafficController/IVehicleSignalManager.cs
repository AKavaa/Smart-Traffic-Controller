using System;

namespace SmartTrafficController
{
    public interface IVehicleSignalManager
    {
        string GetStatus();
        bool SetAllRed(bool on);
        bool SetAllGreen(bool on);

    }


}