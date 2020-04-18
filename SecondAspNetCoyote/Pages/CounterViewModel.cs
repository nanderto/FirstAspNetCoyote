using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Coyote.Actors;
using SecondCoyoteLibrary;

namespace SecondAspNetCoyote.Pages
{
    public interface ICounterViewModel
    {
        int CurrentCount { get; set; }

        Task IncrementCount();

        Task DecrementCount();

        int IncrementAmount { get; set; }

        int DecrementAmount { get; set; }
    }

    public class CounterViewModel : ICounterViewModel
    {
        private readonly IActorRuntime runtime;

        private ActorId AddActor;

        private ActorId SubtractActor;

        public CounterViewModel(SecondCoyoteLibrary.ICoyoteRuntime runtime)
        {
            this.runtime = runtime.Runtime;
        }

        public int CurrentCount { get; set; }

        public int IncrementAmount { get; set; } = 1;

        public int DecrementAmount { get; set; } = 1;

        public async Task IncrementCount()
        {
            var request = new RequestEvent<int, int>(IncrementAmount);
            AddActor = runtime.CreateActor(typeof(AddActor), request);

            var response = await request.Completed.Task;

            CurrentCount = CurrentCount + response;
            runtime.SendEvent(AddActor, HaltEvent.Instance);
        }

        public async Task DecrementCount()
        {
            var request = new RequestEvent<int, int>(DecrementAmount);

            SubtractActor = runtime.CreateActor(typeof(SubtractActor), request);

            var response = await request.Completed.Task;

            CurrentCount = CurrentCount + response;
            runtime.SendEvent(SubtractActor, HaltEvent.Instance);
        }
    }
}
