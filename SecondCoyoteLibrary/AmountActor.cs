using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Coyote;
using Microsoft.Coyote.Actors;

namespace SecondCoyoteLibrary
{
    [OnEventDoAction(typeof(AddEvent), nameof(HandleAdd))]
    [OnEventDoAction(typeof(SubtractEvent), nameof(HandleSubtract))]
    public class AmountActor : Actor
    {
        internal class AddEvent : Event
        {
            public ActorId Caller { get; set; }

            public int Amount { get; set; }
        }

        internal class SubtractEvent : Event
        {
            public ActorId Caller { get; set; }

            public int Amount { get; set; }
        }

        internal class AmountEvent : Event
        {
            public ActorId Sender { get; set; }
            public int Amount{ get; set; }
        }
        private void HandleAdd(Event e)
        {
            var random = new Random();
            int randomNumber = random.Next(0, 1000);
            if(randomNumber < 200)
            {
                throw new ApplicationException("This is a Random error with a 20% chance of happening");
            }

            if (e is AddEvent ae)
            {
                var evnt = new AmountEvent()
                {
                    Amount = ae.Amount,
                    Sender = this.Id
                };

                this.SendEvent(ae.Caller, evnt);
            }
        }

        private void HandleSubtract(Event e)
        {
            if (e is SubtractEvent ae)
            {
                var evnt = new AmountEvent()
                {
                    Amount = -ae.Amount,
                    Sender = this.Id
                };

                this.SendEvent(ae.Caller, evnt);
            }
        }
    }
}
