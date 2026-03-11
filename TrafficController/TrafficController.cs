using System;






namespace SmartTrafficController
{




    public class TrafficController
    {
        // object variables mentioned in the brief

        private string intersectionID;
        private string CurrentVehicleSignalState = "amber";
        private string CurrentPedestrianSignalState = "wait";

        private string historyVehicleManager = "amber";
        private string historyPedestrianState = "wait";
        private IVehicleSignalManager vehicle_manager;
        private IPedestrianSignalManager pedestrian_manager;
        private ITimeManager time_manager;
        private IWebService webService_manager;
        private IEmailService email_manager;




        public TrafficController(string id)
        {
            intersectionID = id.ToLower();
            //L1R4
            CurrentVehicleSignalState = "amber";
            CurrentPedestrianSignalState = "wait";
            vehicle_manager = null!; //! suppress compiler warnings
            pedestrian_manager = null!;
            time_manager = null!;
            webService_manager = null!;
            email_manager = null!;


        }


        public TrafficController(string id, string vehicleStartState, string pedestrianStartState) // This class initializes a new instance of the TrafficController with more signal states
        {
            //L1R1 - L1R2
            intersectionID = id.ToLower();
            vehicle_manager = null!;
            pedestrian_manager = null!;
            time_manager = null!;
            webService_manager = null!;
            email_manager = null!;


            string vehicle_state = vehicleStartState.ToLower();
            string pedestrian_state = pedestrianStartState.ToLower();

            bool vehicleValid = (vehicle_state == "red" || vehicle_state == "redamber" || vehicle_state == "green" || vehicle_state == "amber");
            bool pedestrianValid = (pedestrian_state == "wait" || pedestrian_state == "walk");

            if (!vehicleValid || !pedestrianValid)
            {
                throw new ArgumentException("Argument Exception: TrafficController can only be initialised to the following states: 'green', 'amber', 'red', ‘redamber’ for the vehicle signals and ‘wait’ or ‘walk’ for the pedestrian signal");
            }

            CurrentVehicleSignalState = vehicle_state;
            CurrentPedestrianSignalState = pedestrian_state;



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
                        // L3R2
                        bool walk = pedestrian_manager.SetWalk(false); // false -> disabled
                        bool audible = pedestrian_manager.SetAudible(false);
                        bool wait = pedestrian_manager.SetWait(true);
                        bool waitFirst = time_manager.Delay(3);
                        bool set_all_green = vehicle_manager.SetAllGreen(true);
                        bool move = time_manager.Delay(120);

                        if (walk && audible && wait && set_all_green && move && waitFirst)
                        {
                            vehicleMove = true;
                        }
                        else
                        {
                            // L3R3
                            webService_manager.FaultDetected(true); // Fault detection detected
                            try
                            {
                                // L3R5
                                webService_manager.LogEngineerRequired("out of service");// appropriate message shown 

                            }
                            catch (Exception exception)
                            {
                                email_manager.SendMail( // SendMail method taked 3 parameteers
                                    "transportoffice@gmail.com",
                                    "failed to log out of service",
                                    exception.Message
                                );
                            }


                            historyVehicleManager = CurrentVehicleSignalState; // saves history
                            historyPedestrianState = CurrentPedestrianSignalState; // saves history
                            CurrentVehicleSignalState = "oosv";
                            CurrentPedestrianSignalState = "oosp";


                            return false;
                        }



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
                        // L3R1
                        bool wait = time_manager.Delay(3); // wait 3 seconds
                        bool set_all_red = vehicle_manager.SetAllRed(true); // set all to red
                        bool walk = pedestrian_manager.SetWalk(true); // wall 
                        bool audible = pedestrian_manager.SetAudible(true); // set to audible 
                        bool move = time_manager.Delay(60);

                        if (wait && set_all_red && walk && audible && move)
                        {
                            vehicleMove = true;
                        }
                        else
                        {
                            // L3R3
                            webService_manager.FaultDetected(true); // Fault detection detected

                            try
                            {
                                // L3R5
                                webService_manager.LogEngineerRequired("out of service");// appropriate message shown 

                            }
                            catch (Exception exception) // if LogEngineerRequired throws exception, email with exception is sent 
                            {
                                email_manager.SendMail( // SendMail method taked 3 parameteers
                                    "transportoffice@gmail.com",
                                    "failed to log out of service",
                                    exception.Message
                                );
                            }

                            historyVehicleManager = CurrentVehicleSignalState; // saves history
                            historyPedestrianState = CurrentPedestrianSignalState; // saves history
                            CurrentVehicleSignalState = "oosv";
                            CurrentPedestrianSignalState = "oosp";


                            return false;

                        }



                    }
                    break;
            }

            switch (CurrentPedestrianSignalState) // toggle correct pedestrian sequence
            {
                case "walk":
                    if (pedestrianSignal_input == "wait") { pedestrianMove = true; }
                    break;

                case "wait":
                    if (pedestrianSignal_input == "walk") { pedestrianMove = true; }
                    break;
            }

            if (vehicleMove && pedestrianMove)
            {
                // the state is updated only if both of the signals are legal
                CurrentVehicleSignalState = vehicleSignal_input;
                CurrentPedestrianSignalState = pedestrianSignal_input;
                return true;
            }
            else
            {
                return false;
            }

        }

        public TrafficController(string id, IVehicleSignalManager iVehicleSignalManager, IPedestrianSignalManager iPedestrianSignalManager, ITimeManager iTimeManager, IWebService iWebService, IEmailService iEmailService)
        {

            intersectionID = id.ToLower();

            CurrentVehicleSignalState = "amber";
            CurrentPedestrianSignalState = "wait";
            vehicle_manager = iVehicleSignalManager;
            pedestrian_manager = iPedestrianSignalManager;
            time_manager = iTimeManager;
            webService_manager = iWebService;
            email_manager = iEmailService;

        }

        public TrafficController(string id, IVehicleSignalManager vehicleSignal, IPedestrianSignalManager pedestrianSignal, ITimeManager timeSignal) // fake vehicle, pedestrian, time constructor 
        {
            intersectionID = id.ToLower();

            CurrentVehicleSignalState = "amber";
            CurrentPedestrianSignalState = "wait";
            vehicle_manager = vehicleSignal;
            pedestrian_manager = pedestrianSignal;
            time_manager = timeSignal;
            webService_manager = null!;
            email_manager = null!;


        }



        // check status method for future tests

        public bool CheckStatus()
        {
            string status = vehicle_manager.GetStatus();
            return !status.Contains("FAULT");

        }

        public string GetStatusReport() // L2R4
        {
            // Getting the status of the each class 
            string vehicleStatus = vehicle_manager.GetStatus();
            string pedestrianStatus = pedestrian_manager.GetStatus();
            string timeStatus = time_manager.GetStatus();

            string faultMessage = "";

            // check each status for FAULT
            if (vehicleStatus.Contains("FAULT"))
            {
                faultMessage += "VehicleSignal,";
            }
            if (pedestrianStatus.Contains("FAULT"))
            {
                faultMessage += "PedestrianSignal,";
            }

            if (timeStatus.Contains("FAULT"))
            {
                faultMessage += "Timer,";
            }

            // log is activated only if there is a fault
            if (faultMessage != "")
            {
                webService_manager.LogEngineerRequired(faultMessage);
            }



            // returning all the strings as a single string 
            return vehicleStatus + pedestrianStatus + timeStatus;



        }



        public void RestoreFromHistory() // restoring previous state after fault
        {
            CurrentVehicleSignalState = historyVehicleManager;
            CurrentPedestrianSignalState = historyPedestrianState;
        }

    }


}