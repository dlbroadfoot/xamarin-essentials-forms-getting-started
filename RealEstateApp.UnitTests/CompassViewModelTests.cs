using Moq;
using NUnit.Framework;
using RealEstateApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace RealEstateApp.UnitTests
{
    public class CompassViewModelTests
    {
        [TestCase(0, "North")]
        [TestCase(40, "North")]
        [TestCase(60, "East")]
        [TestCase(90, "East")]
        [TestCase(170, "South")]
        [TestCase(200, "South")]
        [TestCase(260, "West")]
        [TestCase(300, "West")]
        [TestCase(330, "North")]
        public void Compass_Changed_Should_Update_Aspect(double heading, string expectedAspect)
        {
            var compassMock = new Mock<ICompassSensor>();

            var compassViewModel = new CompassViewModel(compassMock.Object);

            compassViewModel.OnAppearing();

            compassMock.Raise(c => c.ReadingChanged += null, new CompassChangedEventArgs(new CompassData(heading)));

            Assert.AreEqual(expectedAspect, compassViewModel.CurrentAspect);
            Assert.AreEqual(heading, compassViewModel.CurrentHeading);
        }
    }
}
