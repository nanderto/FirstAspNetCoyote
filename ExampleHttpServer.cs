using Microsoft.Coyote;
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
            string msg = ((PingPongServer.PongEvent)e).Message;
            this.FinishRequest(msg);
        }
    }

}
