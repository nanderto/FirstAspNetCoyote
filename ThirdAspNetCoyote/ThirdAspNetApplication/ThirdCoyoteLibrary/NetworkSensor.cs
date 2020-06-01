using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Coyote;
using Microsoft.Coyote.Actors;
using Microsoft.Coyote.Actors.Timers;
using ThirdAspNetApplication.Shared;

namespace ThirdCoyoteLibrary
{
    [OnEventDoAction(typeof(TimerElapsedEvent), nameof(HandleTimeout))]
    public class NetworkSensor : Actor
    {
        public TimerInfo PeriodicTimer { get; private set; }

        HttpClient Http = new HttpClient();

        protected override Task OnInitializeAsync(Event initialEvent)
        {
            Console.WriteLine("NetworkSensor Starting a non-periodic timer");
            this.StartTimer(TimeSpan.FromSeconds(1));
            return base.OnInitializeAsync(initialEvent);
        }

        private void HandleTimeout(Event e)
        {
            TimerElapsedEvent te = (TimerElapsedEvent)e;

            Console.WriteLine("<Client> Handling timeout from timer");

            Console.WriteLine("<Client> Starting a period timer");
            this.PeriodicTimer = this.StartPeriodicTimer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new CustomTimerEvent());
        }

        private void HandlePeriodicTimeout(Event e)
        {
            Console.WriteLine("<Client> Handling timeout from periodic timer");
            if (e is CustomTimerEvent ce)
            {
                object PingEvent = Http.GetJsonAsync<Ping>("Ping").Result;
                ce.Count++;
                if (ce.Count == 3)
                {
                    Console.WriteLine("<Client> Stopping the periodic timer");
                    this.StopTimer(this.PeriodicTimer);
                }
            }
        }

        internal class CustomTimerEvent : TimerElapsedEvent
        {
            /// <summary>
            /// Count of timeout events processed.
            /// </summary>
            internal int Count;
        }
    }
}
