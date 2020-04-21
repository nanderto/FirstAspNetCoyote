using System;
using System.Threading.Tasks;
using Microsoft.Coyote.Runtime;
using SecondCoyoteLibrary;
using Xunit;

namespace SecondAspnetCoyoteXUnitTests
{
    public class CounterViewModelTests
    {
        [Fact]
        public async Task IncrementCountTest()
        {
            SecondCoyoteLibrary.ICoyoteRuntime runtime = new CoyoteRuntime();
            var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(runtime);

            viewmodel.CurrentCount = 10;
            viewmodel.IncrementAmount = 3;
            //var task = 
            await viewmodel.IncrementCount();
            //task.Wait();
            Assert.Equal(13, viewmodel.CurrentCount);
        }

        [Fact]
        public void IncrementCountTest2()
        {
            SecondCoyoteLibrary.ICoyoteRuntime runtime = new CoyoteRuntime();
            var viewmodel = new SecondCoyoteLibrary.Pages.CounterViewModel(runtime);

            viewmodel.CurrentCount = 10;
            viewmodel.IncrementAmount = 3;
            viewmodel.IncrementCount();
            
            Assert.Equal(13, viewmodel.CurrentCount);
        }



    }
}
