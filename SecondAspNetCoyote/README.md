## SecondAspNetCoyote
SecondAspNetCoyote is a server side Blazor application that adds Coyote actors into the mix to do the Addition and subtraction of the counter. It uses the same CoyoteRuntime and ICoyoteRuntime to make it inject-able, for details look at the FirstAspNetCoyote application. I am also using a ViewModel in Blazor because it makes for a nice clean implementation.
Here I am building on the Counter page. I have a RequestResponse actor as the base class for connecting between the Asp.Net code and the Coyote code. I create 2 specific implementations of it the Add Actor and the Subtract actor. They both create a third actor the Amount actor which determines if the amount to be added is positive or negative. The amount is handed back to the AddActor or SubtractActor which returns the amount to the view model which adds it to the current amount. The Subtract Actor amount will be negative.

ViewModel --> AddActor --> AmountActor --> AddActor --> ViewModel
--> create actor and send message.

### The ViewModel
```csharp
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
```

### The AddActor
```csharp
[OnEventDoAction(typeof(AmountActor.AmountEvent), nameof(HandleAmountEvent))]
public class AddActor : RequestResponseActor<int, int>
{
    protected override void ProcessRequest(int request)
    {
        var id = this.CreateActor(typeof(AmountActor));
        this.SendEvent(id, new AmountActor.AddEvent() { Caller = this.Id,  Amount = request});
    }

    public void HandleAmountEvent(Event e)
    {
        if (e is AmountActor.AmountEvent ae)
        {
            int amt = ae.Amount;
            this.FinishRequest(amt);
            this.SendEvent(ae.Sender, HaltEvent.Instance);
        }
    }
}
```

### The Amount Actor
```csharp
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
```
Notice that I have added in the Handle add a few lines of code to randomly throw an exception. At this point YOu can run the application and watch it work. If it throws the exception it brings Blazor down but its hard to find.

## Running the Coyote test tool
The coyote tool did not work as advertised I had to run the dotnet command:
```dos
c:\Dotnet C:\<pathtopackages>\packages\microsoft.coyote\1.0.4\lib\netcoreapp3.1\coyote.dll test c:\<Path to Library to test>\SecondCoyoteLibrary.dll -i 100 -ms 500
```
This should run and find the random bug.


