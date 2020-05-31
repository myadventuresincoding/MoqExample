using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace MoqExample.Tests
{
    [TestFixture]
    public class SomeAppTests
    {
        private Mock<ISomeService> _mockSomeService;

        [SetUp]
        public void Setup()
        {
            _mockSomeService = new Mock<ISomeService>();
        }

        /// <summary>
        /// Scenario: Return a different item the second time a mocked method is called.
        /// Let's say I have a call in a service that returns the next item to process, 
        /// such as pulling in customer requests from a queue, and returning them in order.
        /// </summary>
        [Test]
        public void MogMethodThatReturnsADifferentValueWhenCalledASecondTime()
        {
            var queueStuff = new Queue<SomeStuff>();
            queueStuff.Enqueue(new SomeStuff { Id = 1, Name = "Real" });
            queueStuff.Enqueue(null);

            _mockSomeService.Setup(x => x.GetNextStuff()).Returns(queueStuff.Dequeue);

            Assert.IsNotNull(_mockSomeService.Object.GetNextStuff());
            Assert.IsNull(_mockSomeService.Object.GetNextStuff());
        }

        /// <summary>
        /// Scenario: Throw an exception the first time a mocked method is called, but succeed on the second try.
        /// Lets say for example I want my application to retry on an exception, 
        /// and if it suceeds on the second try, then process the request and keep going.
        /// </summary>
        [Test]
        public void MogMethodThatThrowsAnExceptionFirstTimeCalledAndAnObjectWithSecondTime()
        {
            var calls = 0;
            _mockSomeService.Setup(x => x.GetNextStuff())
                .Returns(() => new SomeStuff())
                .Callback(() =>
                {
                    calls++;
                    if (calls == 1)
                        throw new Exception("Failure");
                });

            Assert.Throws<Exception>(() => _mockSomeService.Object.GetNextStuff());
            Assert.IsNotNull(_mockSomeService.Object.GetNextStuff());
        }

        [Test]
        public void MogMethodThatThrowsAnExceptionIsVoidAssertExceptionIsThrown()
        {
            _mockSomeService.Setup(x => x.DoStuff()).Throws(new Exception("Failure"));

            Assert.Throws<Exception>(() => _mockSomeService.Object.DoStuff());
        }
    }
}
