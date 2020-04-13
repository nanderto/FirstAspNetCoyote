using Microsoft.Coyote;
using Microsoft.Coyote.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAspNetCoyote
{
    [OnEventDoAction(typeof(PingEvent), nameof(HandlePing))]
    public class PingPongServer : Actor
    {
        public class PingEvent : Event
        {
            public string Message;
            public ActorId Caller;
        }
        public class PongEvent : Event
        {
            public string Message;
        }

        private void HandlePing(Event e)
        {
            if (e is PingEvent p)
            {
                this.SendEvent(p.Caller, new PongEvent() { Message = "Received: " + p.Message });
            }
        }

    }
}
