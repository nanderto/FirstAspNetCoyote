using Microsoft.Coyote;
using Microsoft.Coyote.Specifications;
using Microsoft.Coyote.SystematicTesting;
using Microsoft.Coyote.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecondCoyoteLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SecondAspNetCoyote.Pages.Tests
{
    [TestClass]
    public class CounterViewModelTests
    {
        [TestMethod]
        public void IncrementCountTest_ExpectedToFailBecauseNoAwait()
        {
            ICoyoteRuntime runtime = new CoyoteRuntime();
            var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(runtime);

            viewmodel.CurrentCount = 10;
            viewmodel.IncrementAmount = 3;
            viewmodel.IncrementCount();
            Assert.AreEqual(13, viewmodel.CurrentCount);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IncrementCountTestAsnc()
        {
            ICoyoteRuntime runtime = new CoyoteRuntime();
            var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(runtime);

            viewmodel.CurrentCount = 10;
            viewmodel.IncrementAmount = 3;
            await viewmodel.IncrementCount();
            Assert.AreEqual(13, viewmodel.CurrentCount);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IncrementCountTestAsnc_UsingCoyoteTestEngine()
        {
            // Create some test configuration, this is just a basic one using 100 test iteration.
            // Each iteration executes the test method from scratch, exploring different interleavings.
            Configuration configuration = Configuration.Create().WithTestingIterations(100);

            // Create the Coyote runner programmatically.
            // Assign the configuration and the test method to run.
            TestingEngine engine = TestingEngine.Create(configuration, async runtime =>
            {
                var rt = new CoyoteRuntime { Runtime = runtime };
                var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(rt);

                viewmodel.CurrentCount = 10;
                viewmodel.IncrementAmount = 3;
                await viewmodel.IncrementCount();
                Specification.Assert(viewmodel.CurrentCount == 13, "CurrentCount is '{0}' instead of 13.", viewmodel.CurrentCount);
            });

            // Run the Coyote test.
            engine.Run();

            // Check for bugs.
            Console.WriteLine($"Found #{engine.TestReport.NumOfFoundBugs} bugs.");
            if (engine.TestReport.NumOfFoundBugs == 1)
            {
                Console.WriteLine($"Bug: {engine.TestReport.BugReports.First()}");
            }
        }
    }
}
