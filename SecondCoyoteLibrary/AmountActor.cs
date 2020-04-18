using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (e is AddEvent ae)
            {
                for (int i = 0; i < 10; i++)
                {
                    Slow();
                }

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
                for (int i = 0; i < 50; i++)
                {
                    Slow();
                }

                var evnt = new AmountEvent()
                {
                    Amount = -ae.Amount,
                    Sender = this.Id
                };

                this.SendEvent(ae.Caller, evnt);
            }
        }

        public void Slow()
        {
            long nthPrime = FindPrimeNumber(1000); //set higher value for more time
        }

        public long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
    }
}
