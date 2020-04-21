using Microsoft.Coyote.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecondCoyoteLibrary;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SecondAspNetCoyote.Pages.Tests
{
    [TestClass]
    public class CounterViewModelTests
    {
        [TestMethod]
        public void IncrementCountTest()
        {
            ICoyoteRuntime runtime = new CoyoteRuntime();
            var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(runtime);

            viewmodel.CurrentCount = 10;
            viewmodel.IncrementAmount = 3;
            viewmodel.IncrementCount();
            Assert.AreEqual(13, viewmodel.CurrentCount);
        }

        [TestMethod]
        public async Task IncrementCountTestAsnc()
        {
            ICoyoteRuntime runtime = new CoyoteRuntime();
            var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(runtime);

            viewmodel.CurrentCount = 10;
            viewmodel.IncrementAmount = 3;
            await viewmodel.IncrementCount();
            Assert.AreEqual(13, viewmodel.CurrentCount);
        }
    }
}
