using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Coyote.Actors;
using Microsoft.Coyote.SystematicTesting;
using Microsoft.Coyote.Tasks;

namespace FirstCoyoteLibrary
{
    class Test
    {
        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task Execute(IActorRuntime runtime)
        {
            var request = new RequestEvent<string, string>("Hi Mom!");

            ActorId id = runtime.CreateActor(typeof(ExampleHttpServer), request);

            var response = await request.Completed.Task;

            runtime.SendEvent(id, HaltEvent.Instance);
        }
    }
}
