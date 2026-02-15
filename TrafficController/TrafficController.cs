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

            //L1R4
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


        public void SetIntersectionID(string id)   // L1R3
        {
            intersectionID = id.ToLower();

        }
        // L1R5 
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

        // L2R1 
        public bool SetCurrentState(string vehicleSignal, string pedestrianSignal)
        {
            string vehicleSignal_input = vehicleSignal?.ToLower() ?? "";
            string pedestrianSignal_input = pedestrianSignal?.ToLower() ?? "";

            // if the color or pedestrian state change, the switch statement will check if the change is legal
            bool vehicleMove = (vehicleSignal_input == CurrentVehicleSignalState);
            bool pedestrianMove = (pedestrianSignal_input == CurrentPedestrianSignalState);

            switch (CurrentVehicleSignalState) // correct lights sequence
            {
                case "red":
                    if (vehicleSignal_input == "redamber")
                    {
                        vehicleMove = true;
                    }
                    break;
                case "redamber":
                    if (vehicleSignal_input == "green")
                    {
                        vehicleMove = true;
                    }
                    break;
                case "green":
                    if (vehicleSignal_input == "amber")
                    {
                        vehicleMove = true;
                    }
                    break;
                case "amber":
                    if (vehicleSignal_input == "red")
                    {
                        vehicleMove = true;
                    }
                    break;
            }

            switch (CurrentPedestrianSignalState) // toggle correct pedestrian sequence
            {
                case "walk":
                    if (pedestrianSignal_input == "wait")
                    {
                        pedestrianMove = true;
                    }
                    break;
                case "wait":
                    if (pedestrianSignal_input == "walk")
                    {
                        pedestrianMove = true;
                    }
                    break;
            }

            if (vehicleMove && pedestrianMove)
            {
                CurrentVehicleSignalState = vehicleSignal_input;
                CurrentPedestrianSignalState = pedestrianSignal_input;
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
