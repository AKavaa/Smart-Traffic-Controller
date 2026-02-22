using NUnit.Framework;
using System;
using SmartTrafficController;
using NSubstitute;



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
        public void CheckStatus_AllOK_ReturnsTrue() //L3
        {

            //Arrange
            var FakeVehicle = Substitute.For<IVehicleSignalManager>();
            FakeVehicle.GetStatus().Returns("VehicleSignal,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            var controller = new TrafficController("test", FakeVehicle);

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
            FakeVehicle.GetStatus().Returns("VehicleSignal,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,");
            var controller = new TrafficController("test", FakeVehicle);

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

    }

}
