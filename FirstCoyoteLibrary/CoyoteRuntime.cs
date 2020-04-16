using Microsoft.Coyote.Actors;
using Microsoft.Coyote.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCoyoteLibrary
{
    public interface ICoyoteRuntime
    {
        IActorRuntime Runtime { get; set; }
    }

    public class CoyoteRuntime : ICoyoteRuntime
    {
        IActorRuntime runtime; // Configuration.Create().WithVerbosityEnabled());

        public IActorRuntime Runtime { get => runtime; set => runtime = value; }

        public CoyoteRuntime()
        {
            Runtime = Microsoft.Coyote.Actors.RuntimeFactory.Create();
            Execute(Runtime);
        }

        private static void OnRuntimeFailure(Exception ex)
        {
            Console.WriteLine("Unhandled exception: {0}", ex.Message);
        }

        [Microsoft.Coyote.SystematicTesting.Test]
        public static void Execute(IActorRuntime runtime)
        {
            Console.WriteLine("Registering Monitor");
            runtime.OnFailure += OnRuntimeFailure;
            // runtime.RegisterMonitor<NetworkMonitor>();
            //ActorId driver = runtime.CreateActor(typeof(FailoverDriver), new ConfigEvent(RunForever));
            //runtime.SendEvent(driver, new FailoverDriver.StartTestEvent());
        }

    }
}
