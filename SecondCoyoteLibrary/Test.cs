using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Coyote.Actors;
using Microsoft.Coyote.Tasks;

namespace SecondCoyoteLibrary
{
    public class Test
    {
        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task Execute(IActorRuntime runtime)
        {
            int CurrentCount = 0;
            int incrementAmount = 3;
            var request = new RequestEvent<int, int>(incrementAmount);
            var AddActor = runtime.CreateActor(typeof(AddActor), request);

            var response = await request.Completed.Task;

            CurrentCount = CurrentCount + response;
            runtime.SendEvent(AddActor, HaltEvent.Instance);
        }
    }
}
