using System;

namespace SmartTrafficController
{


    public class TrafficController
    {
        // object variables mentioned in the brief

        private string intersectionID;
        private string CurrentVehicleSignalState;
        private string CurrentPedestrianSignalState;


        public TrafficController(string id)
        {
            //L1R1 - L1R2
            intersectionID = id.ToLower();
            CurrentVehicleSignalState = "amber";
            CurrentPedestrianSignalState = "wait";


        }

        public string GetIntersectionID()
        {
            return intersectionID;
        }

        public string GetCurrentVehicleSignalState()
        {
            return CurrentVehicleSignalState;
        }

        public string GetCurrentPedestrianSignalState()
        {
            return CurrentPedestrianSignalState;

        }


        // L1R5 
        public void SetIntersectionID(string id)
        {
            intersectionID = id.ToLower();

        }
        public bool SetStateDirect(string vehicle, string pedestrian)
        {
            string vehicle_input = vehicle?.ToLower() ?? ""; // if vehicle is "RED" it will become "red", and ?? "" eliminates the warning inside the code
            string pedestrian_input = pedestrian?.ToLower() ?? "";

            bool isVehicleValid = (vehicle_input == "red" || vehicle_input == "redamber" || vehicle_input == "green" || vehicle_input == "amber" || vehicle_input == "oosv");

            bool isPedestrianValid = (pedestrian_input == "wait" || pedestrian_input == "walk" || pedestrian_input == "oosp");

            if (isVehicleValid && isPedestrianValid)
            {
                CurrentVehicleSignalState = vehicle_input;
                CurrentPedestrianSignalState = pedestrian_input;
                return true;
            }
            else
            {
                return false;
            }




        }

    }
}
