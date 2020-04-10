using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryBuckets.Client.Components.Counter;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Client.Components.Counter.Tests
{
    [TestClass()]
    public class CounterViewModelTests
    {
        [TestMethod()]
        public void CurrentCount_StartsAt0()
        {
            //Arrange
            var vm = new CounterViewModel();

            //Act         

            //Assert
            Assert.AreEqual(0, vm.CurrentCount);
        }

        [TestMethod()]
        public void ClickingButton_IncrementsCurrentCountBy1()
        {
            //Arrange
            var vm = new CounterViewModel();

            //Act         
            var before = vm.CurrentCount;
            vm.IncrementButtonClick();

            //Assert
            Assert.AreEqual(before + 1, vm.CurrentCount);
        }
    }
}