﻿using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Xunit;

namespace MoqExample.Tests
{
    [TestFixture]
    public class SomeAppTests
    {
       
        /// Scenario: Return a different item the second time a mocked method is called.
        /// Let's say I have a call in a service that returns the next item to process, 
        /// such as pulling in customer requests from a queue, and returning them in order. 

        /// <summary>
        /// The more well known approach is to just use a Queue and have the mock call 
        /// Dequeue and return the result each time the mocked method is called.
        /// </summary>
        [Test]
        public void Mock_ShouldReturnDifferentValue_WhenCalledASecondTimeUsingQueue()
        {
            // Arrange
            Mock<ISomeService> _mockSomeService = new Mock<ISomeService>();

            var queueStuff = new Queue<SomeStuff>();
            queueStuff.Enqueue(new SomeStuff { Id = 1, Name = "Real" });
            queueStuff.Enqueue(null);

            _mockSomeService.Setup(x => x.GetNextStuff()).Returns(queueStuff.Dequeue);

            // Act
            var resultOne = _mockSomeService.Object.GetNextStuff();
            var resultTwo = _mockSomeService.Object.GetNextStuff();

            // Assert
            Assert.AreEqual("Real", resultOne.Name);
            Assert.IsNull(resultTwo);
        }

        /// <summary>
        /// The alternative is to use a feature in Moq called Sequences which allows 
        /// you to set multiple return values, that will be returned one at a time in order, 
        /// each time the mocked method is called.
        /// </summary>
        [Test]
        public void Mock_ShouldReturnsDifferentValue_WhenCalledASecondTimeUsingSequences()
        {
            // Arrange
            Mock<ISomeService> _mockSomeService = new Mock<ISomeService>();

            _mockSomeService.SetupSequence(x => x.GetNextStuff())
                    .Returns(new SomeStuff { Id = 1, Name = "Real" })
                    .Returns((SomeStuff)null);

            // Act
            var resultOne = _mockSomeService.Object.GetNextStuff();
            var resultTwo = _mockSomeService.Object.GetNextStuff();

            // Assert
            Assert.AreEqual("Real", resultOne.Name);
            Assert.IsNull(resultTwo);
        }

        /// Scenario: Throw an exception the first time a mocked method is called, but succeed on the second try.
        /// Lets say for example I want my application to retry on an exception, 
        /// and if it suceeds on the second try, then process the request and keep going.


        /// <summary>
        /// In this case I want to test that my application will handle the case that when a call to a service 
        /// throws an exception, it will retry and if it receives a valid response on the second try, process the 
        /// request successfully and continue. You can accomplish this by using a feature in Moq called “Callback”.
        /// </summary>
        [Test]
        public void Mock_ShouldSucceedOnSecondCall_WhenThrowsExceptionOnFirstCall()
        {
            // Arrange
            Mock<ISomeService> _mockSomeService = new Mock<ISomeService>();
            var calls = 0;

            _mockSomeService.Setup(x => x.GetNextStuff())
                .Returns(() => new SomeStuff { Id = 1, Name = "Real" })
                .Callback(() =>
                {
                    calls++;
                    if (calls == 1)
                        throw new Exception("Failure");
                });

            // Act
            var resultOneException = Record.Exception(() => _mockSomeService.Object.GetNextStuff());
            var resultTwo = _mockSomeService.Object.GetNextStuff();

            // Assert
            Assert.AreEqual("Failure", resultOneException.Message);
            Assert.AreEqual("Real", resultTwo.Name);
        }

        /// <summary>
        /// As an alternative to using a Callback, you can use the Moq feature Sequence for your mock setup.
        /// </summary>
        [Test]
        public void Mock_ShouldThrowExceptionOnSecondCall_WhenSucceedsOnFirstCallUsingSequences()
        {
            // Arrange
            Mock<ISomeService> _mockSomeService = new Mock<ISomeService>();

            _mockSomeService.SetupSequence(x => x.GetNextStuff())
                .Throws(new Exception("Failure"))
                .Returns(new SomeStuff { Id = 1, Name = "Real" });

            // Act
            var resultOneException = Record.Exception(() => _mockSomeService.Object.GetNextStuff());
            var resultTwo = _mockSomeService.Object.GetNextStuff();
            

            // Assert
            Assert.AreEqual("Failure", resultOneException.Message);
            Assert.AreEqual("Real", resultTwo.Name);
        }

        /// Scenario: Mock a void method to throw an exception

        /// <summary>
        /// Lets say I have some void method that normally just silently does some task for me and has no need 
        /// to have a return type, such as a call to write a stat or a log entry.However, if I want to test how 
        /// my application handles the case when this call throws an exception, I can use the following setup to 
        /// mock this method.
        /// </summary>
        [Test]
        public void Mock_ShouldThrowException_WhenMockedVoidMethodIsCalled()
        {
            // Arrange
            Mock<ISomeService> _mockSomeService = new Mock<ISomeService>();

            _mockSomeService.Setup(x => x.DoStuff())
                .Throws(new Exception("Failure"));

            // Act
            var resultException = Record.Exception(() => _mockSomeService.Object.DoStuff());

            // Assert
            Assert.AreEqual("Failure", resultException.Message);
        }
    }
}
