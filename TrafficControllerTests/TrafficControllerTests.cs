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

        [Test]
        public void InitialiseState_UpdateToLowerCase() //L1R3
        {
            //Arrange
            var controller = new TrafficController("test");

            //Act
            controller.SetIntersectionID("SOUTH");

            //Assert
            Assert.That(controller.GetIntersectionID(), Is.EqualTo("south"));
        }

        [Test]
        public void ConstructorInitialise_ToLowerCase() //L1R2
        {
            //Arranne + Act
            var controller = new TrafficController("SOUTH");

            //Assert
            Assert.That(controller.GetIntersectionID(), Is.EqualTo("south"));
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
            var controller = new TrafficController("test");

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

        public void SetStateDirect_ManyInputs_ReturnsExpected(string vehicle, string pedestrian, bool expected)
        {
            //Arrange
            var controller = new TrafficController("test");

            //Act
            bool result = controller.SetStateDirect(vehicle, pedestrian);

            //Assert
            Assert.That(result, Is.EqualTo(expected));


        }

        [Test]
        public void Constructor_InvalidState_ThrowArgumentException() // L2R2
        {
            // Arrange + Act
            var exception = Assert.Throws<ArgumentException>(() => new TrafficController("test", "pink", "run"));


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

            FakeVehicle.GetStatus().Returns("VehiclesSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,");
            FakePedestrian.GetStatus().Returns("PedestrianSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            FakeTime.GetStatus().Returns("Timer,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,");
            var controller = new TrafficController("test", FakeVehicle, FakePedestrian, FakeTime);

            // Act
            string result = controller.GetStatusReport();


            // Assert
            Assert.That(result, Is.EqualTo("VehiclesSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,PedestrianSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,Timer,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,"));
        }

    }


}


