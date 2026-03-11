using NUnit.Framework;
using System;
using SmartTrafficController;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
#nullable disable // overrides compiler's nullable annotations 



namespace SmartTrafficControllerTests
{


    [TestFixture]
    public class TrafficControllerTests
    {

        [Test]
        public void InitialiseState_SetAmber()   // L1R4 (initial state amber)
        {
            // Arrange 
            var controller = new TrafficController("test");

            //Act
            string result = controller.GetCurrentVehicleSignalState();

            //Assert
            Assert.That(result, Is.EqualTo("amber"));

        }
        [Test]
        public void InitialiseState_SetWait() // Pedestrian State
        {
            //Arrange
            var controller = new TrafficController("test");

            //Act
            string result = controller.GetCurrentPedestrianSignalState();

            //Assert
            Assert.That(result, Is.EqualTo("wait"));
            Console.WriteLine(result);
        }




        [TestCase("SOUTH", "south")]
        [TestCase("NORTH", "north")]
        [TestCase("EAST", "east")]
        [TestCase("WEST", "west")]
        [TestCase("Test", "test")]
        public void InitialiseState_UpdateToLowerCase(string input, string expected) //L1R3
        {
            //Arrange
            var controller = new TrafficController("test");

            //Act
            controller.SetIntersectionID(input);

            //Assert
            Assert.That(controller.GetIntersectionID(), Is.EqualTo(expected));
        }


        [TestCase("SOUTH", "south")]
        [TestCase("NORTH", "north")]
        [TestCase("EAST", "east")]
        [TestCase("WEST", "west")]
        [TestCase("Test", "test")]
        public void ConstructorInitialise_ToLowerCase(string input, string expected) //L1R2
        {
            //Arranne + Act
            var controller = new TrafficController(input);

            //Assert
            Assert.That(controller.GetIntersectionID(), Is.EqualTo(expected));
        }


        [Test]
        public void InitialiseState_SetStateDirect_returnTrue() //L1R5
        {

            //Arrange

            var controller = new TrafficController("test");

            //Act
            bool result = controller.SetStateDirect("red", "walk");

            //Assert
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]

        public void InitialiseState_SetStateDirect_returnFalse() //L1R5 
        {
            //Arrange
            var controller = new TrafficController("test");

            //Act
            var result = controller.SetStateDirect("purple", "wait");

            //Assert
            Assert.That(result, Is.EqualTo(false));

        }
        [Test]
        public void SetCurrentState_IsValid_ReturnsTrue() //L2R1
        {

            //Arrange
            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakeTime.Delay(3).Returns(true);
            FakeVehicle.SetAllRed(true).Returns(true);
            FakePedestrian.SetWalk(true).Returns(true);
            FakePedestrian.SetAudible(true).Returns(true);
            FakeTime.Delay(60).Returns(true);

            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);



            //Act
            var result = controller.SetCurrentState("red", "walk");
            // starting color is amber so the next following color is red so its true

            // Assert
            Assert.That(result, Is.EqualTo(true));

        }

        [Test]
        public void SetCurrentState_IsInvalid_ReturnsFalse() //L2R1
        {

            //Arrange 
            var controller = new TrafficController("test");

            //Act
            var result = controller.SetCurrentState("green", "walk");
            // starting color is amber so the next following color cannot be green

            // Assert
            Assert.That(result, Is.EqualTo(false));

        }


        [Test]
        public void CheckStatus_AllOK_ReturnsTrue() //L3
        {

            //Arrange
            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            FakeVehicle.GetStatus().Returns("VehicleSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime);

            // Act
            bool result = controller.CheckStatus();

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void CheckStatus_FaultDetected_ReturnsFalse() //L3
        {

            //Arrange
            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            FakeVehicle.GetStatus().Returns("VehicleSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,");
            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime);

            // Act
            bool result = controller.CheckStatus();

            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        [Test] //L1R5
        [TestCase("amber", "walk", true)] // test 1 valid inputs - expects true
        [TestCase("green", "wait", true)] // test 2 valid inputs - expects true
        [TestCase("blue", "sprinting", false)] // test 3 invalid inputs - expects false
        [TestCase("purple", "wait", false)] // test 4 invalid inputs - expects false
        [TestCase("red", "walk", true)]
        [TestCase("red", "wait", true)]
        [TestCase("redamber", "walk", true)]
        [TestCase("redamber", "wait", true)]
        [TestCase("oosv", "walk", true)]
        [TestCase("oosv", "oosp", true)] // oosv is valid only with oosp
        [TestCase("RED", "WALK", true)] // upper case handling
        [TestCase("AMBER", "WAIT", true)]
        [TestCase("REDAMBER", "WAIT", true)]
        [TestCase("Green", "Walk", true)] // mixed case
        [TestCase("Redamber", "Wait", true)]
        [TestCase("RED", "Wait", true)]
        [TestCase("amber", "run", false)]
        [TestCase("123", "wait", false)]
        [TestCase("red", "sprint", false)]
        [TestCase("", "walk", false)]


        public void SetStateDirect_ManyInputs_ReturnsExpected(string vehicle, string pedestrian, bool expected)
        {
            //Arrange
            var controller = new TrafficController("test");

            //Act
            bool result = controller.SetStateDirect(vehicle, pedestrian);

            //Assert
            Assert.That(result, Is.EqualTo(expected));


        }

        [TestCase("blue", "walk")]
        [TestCase("amber", "run")]
        [TestCase("oosv", "oosp")]
        [TestCase(" ", "walk")]
        [TestCase("green", "sprint")]
        public void Constructor_InvalidState_ThrowArgumentException(string vehicle, string pedestrian) // L2R2
        {
            // Arrange + Act
            var exception = Assert.Throws<ArgumentException>(() => new TrafficController("test", vehicle, pedestrian));


            Console.WriteLine(exception.Message); // output the message into the console

            //Assert
            // checks if the exception message is identical
            Assert.That(exception.Message, Is.EqualTo("Argument Exception: TrafficController can only be initialised to the following states: 'green', 'amber', 'red', ‘redamber’ for the vehicle signals and ‘wait’ or ‘walk’ for the pedestrian signal"));
        }

        [Test]
        public void GetStatusReport_GetAllStatuses() // L2R4 checks the statuses of vehicle, pedestrian and time and returns the combined output
        {
            //Arrange
            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakeVehicle.GetStatus().Returns("VehiclesSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,");
            FakePedestrian.GetStatus().Returns("PedestrianSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            FakeTime.GetStatus().Returns("Timer,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,");
            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);

            // Act
            string result = controller.GetStatusReport();


            // Assert
            Assert.That(result, Is.EqualTo("VehiclesSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,PedestrianSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,Timer,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,"));
        }

        [Test]
        public void SetCurrentState_AmberToRed_MethodsCalledCorrectly_ReturnsTrue() // Test for L3R1
        {
            // Arrange

            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakeTime.Delay(3).Returns(true);
            FakeVehicle.SetAllRed(true).Returns(true);
            FakePedestrian.SetWalk(true).Returns(true);
            FakePedestrian.SetAudible(true).Returns(true);
            FakeTime.Delay(60).Returns(true);

            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);


            // Act
            bool result = controller.SetCurrentState("Red", "walk");

            //Assert

            Assert.That(result, Is.EqualTo(true));
            FakeTime.Received().Delay(3); // NSubstitute method that verifies that the mock object was actually called 
            FakeVehicle.Received().SetAllRed(true);
            FakePedestrian.Received().SetWalk(true);
            FakePedestrian.Received().SetAudible(true);
            FakeTime.Received().Delay(60);

        }

        [Test]
        public void SetCurrentState_RedAmberToGreen_MethodsCalledCorrectly_ReturnsTrue() //Test for L3R2
        {
            // Arrange

            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakePedestrian.SetWalk(false).Returns(true);
            FakePedestrian.SetAudible(false).Returns(true);
            FakePedestrian.SetWait(true).Returns(true);
            FakeVehicle.SetAllGreen(true).Returns(true);
            FakeTime.Delay(3).Returns(true);
            FakeTime.Delay(120).Returns(true);


            // using SetStateDirect to get into the redamber state with the specific dependencies
            var controller2 = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);

            controller2.SetStateDirect("redamber", "wait"); // forcing this parameters into the redamber state


            // Act
            bool result = controller2.SetCurrentState("green", "wait");
            //Assert

            Assert.That(result, Is.EqualTo(true));
            FakeVehicle.Received().SetAllGreen(true);
            FakePedestrian.Received().SetWalk(false);
            FakePedestrian.Received().SetWait(true);
            FakePedestrian.Received().SetAudible(false);
            FakeTime.Received().Delay(3);
            FakeTime.Received().Delay(120);

        }
        [Test]
        public void SetCurrentState_FaultDetected_SetOosvState() // L3R3 trigger oosv change
        {
            // Arrange

            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakeTime.Delay(3).Returns(false); // failure simulation to enable fault detection

            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);

            // Act

            bool result = controller.SetCurrentState("red", "walk");

            //Assert

            Assert.That(result, Is.EqualTo(false)); // fault path is always false
            Assert.That(controller.GetCurrentVehicleSignalState(), Is.EqualTo("oosv"));
            Assert.That(controller.GetCurrentPedestrianSignalState(), Is.EqualTo("oosp"));
            FakeWebService.Received().FaultDetected(true); // Fault detected is true
            FakeWebService.Received().LogEngineerRequired("out of service"); // appropriate message shown
        }


        [Test]
        public void GetStatusReport_FaultDetected_LogEngineerRequired() // L3R4 Verifying that LogEngineerRequired returns the correct string 
        {
            // Arrange

            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakeVehicle.GetStatus().Returns("VehiclesSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,"); // Has fault
            FakePedestrian.GetStatus().Returns("PedestrianSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,"); // Has no fault
            FakeTime.GetStatus().Returns("Timer,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,"); // Has fault
            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);

            // Act

            controller.GetStatusReport();

            // Assert
            FakeWebService.Received().LogEngineerRequired("VehicleSignal,Timer,");

        }

        [Test]
        public void SetCurrentState_LogEngineerRequiredThrows_Email() // L3R5 Verifyig that the SendMail() is being called when LogEngineerRequired is throwed
        {
            // Arrange

            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();

            FakeTime.Delay(3).Returns(false); // triggers fault path, false Wait call 
            FakeWebService.LogEngineerRequired("out of service")
            .Throws(new Exception("Log Failed")); // Simulation of LogEngineer throwing an exception, Mock

            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);


            //Act
            controller.SetCurrentState("red", "walk"); // amber to red triggers fault path 

            // Assert
            FakeMail.Received().SendMail(
                 "transportoffice@gmail.com",
              "failed to log out of service",
              "Log Failed" // -> exception message from the failed LogEngineerRequiered call
            );


        }

        [Test]
        [TestCase(false, true, true, true, true)] // Delay(3) fails
        [TestCase(true, false, true, true, true)] // SetAllRed fails
        [TestCase(true, true, false, true, true)] // setWalk fails
        [TestCase(true, true, true, false, true)] // SetAudible fails
        [TestCase(true, true, true, true, false)] // Delay(60)fails
        public void RestoreFromHistory_AmberToRedFault_RestoresAmberWait(
            bool delay, bool setAllRed, bool walk, bool audible, bool move
        )
        {
            // Arrange

            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            var FakePedestrian = Substitute.For<IPedestrianSignalManager>();
            var FakeTime = Substitute.For<ITimeManager>();
            var FakeWebService = Substitute.For<IWebService>();
            var FakeMail = Substitute.For<IEmailService>();


            FakePedestrian.SetWalk(true).Returns(walk);
            FakePedestrian.SetAudible(true).Returns(audible);
            FakePedestrian.SetWait(true).Returns(true);
            FakeVehicle.SetAllRed(true).Returns(setAllRed);
            FakeTime.Delay(3).Returns(delay);
            FakeTime.Delay(60).Returns(move);


            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime, FakeWebService, FakeMail);

            // controller will start from amber and wait by defauly


            //Act - trigger fault path
            controller.SetCurrentState("red", "walk");
            Assert.That(controller.GetCurrentVehicleSignalState(), Is.EqualTo("oosv"));
            Assert.That(controller.GetCurrentPedestrianSignalState(), Is.EqualTo("oosp"));


            // restore from history

            controller.RestoreFromHistory();

            // Assert - should get back to amber and wait state

            Assert.That(controller.GetCurrentVehicleSignalState(), Is.EqualTo("amber"));
            Assert.That(controller.GetCurrentPedestrianSignalState(), Is.EqualTo("wait"));




        }

    }





}


