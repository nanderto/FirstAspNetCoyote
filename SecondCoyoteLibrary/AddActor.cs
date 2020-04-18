using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Coyote;
using Microsoft.Coyote.Actors;

namespace SecondCoyoteLibrary
{
    [OnEventDoAction(typeof(AmountActor.AmountEvent), nameof(HandleAmountEvent))]
    public class AddActor : RequestResponseActor<int, int>
    {
        protected override void ProcessRequest(int request)
        {
            var id = this.CreateActor(typeof(AmountActor));
            for (int i = 0; i < 200; i++)
            {
                this.SendEvent(id, new AmountActor.AddEvent() { Caller = this.Id, Amount = request });
            }
        }

        public void HandleAmountEvent(Event e)
        {
            if (e is AmountActor.AmountEvent ae)
            {
                int amt = ae.Amount;
                this.FinishRequest(amt);
                //this.SendEvent(ae.Sender, HaltEvent.Instance);
            }
        }
    }

    [OnEventDoAction(typeof(AmountActor.AmountEvent), nameof(HandleAmountEvent))]
    public class SubtractActor : RequestResponseActor<int, int>
    {
        protected override void ProcessRequest(int request)
        {
            var id = this.CreateActor(typeof(AmountActor));
            for (int i = 0; i < 200; i++)
            {
                this.SendEvent(id, new AmountActor.SubtractEvent() { Caller = this.Id, Amount = request });
            }
        }

        public void HandleAmountEvent(Event e)
        {
            if (e is AmountActor.AmountEvent ae)
            {
                int amt = ae.Amount;
                this.FinishRequest(amt);
                //this.SendEvent(ae.Sender, HaltEvent.Instance);
            }
        }
    }
}
