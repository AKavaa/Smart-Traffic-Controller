using NUnit.Framework;
using System;
using SmartTrafficController;



namespace SmartTrafficControllerTests
{
    [TestFixture]
    public class TrafficControllerTests
    {
        [Test]
        public void InitialiseState_SetAmberAndWait()   // L1R4 (initial state amber)
        {
            // Arrange 
            var controller = new TrafficController("test");

            //Act
            string result = controller.GetCurrentVehicleSignalState();

            //Assert

            Assert.That(result, Is.EqualTo("amber"));

        }



    }

}
