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
            Assert.That(controller.GetIntersectionID, Is.EqualTo("south"));
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
        public void CheckStatus_AllOK_ReturnsTrue()
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
        public void CheckStatus_FaultDetected_ReturnsFalse()
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



    }

}
