using Microsoft.Coyote;
using Microsoft.Coyote.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAspNetCoyote
{
    [OnEventDoAction(typeof(PingPongServer.PongEvent), nameof(HandlePong))]
    public class ExampleHttpServer : RequestResponseActor<string, string>
    {
        protected override void ProcessRequest(string request)
        {
            var id = this.CreateActor(typeof(PingPongServer));
            this.SendEvent(id, new PingPongServer.PingEvent() { Caller = this.Id, Message = request });
        }

        public void HandlePong(Event e)
        {
            if (e is PingPongServer.PongEvent pe)
            {
                string msg = pe.Message;
                this.FinishRequest(msg);
                this.SendEvent(pe.Sender, HaltEvent.Instance);
            }
        }
    }

}
