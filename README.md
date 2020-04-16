### FirstAspNetCoyote
FirstAspNetCoyote is my first attempt at using this in an ASP.NET application. I have not been able to find any posts on this so what ever you learn here remember that it has probably been superseded by smarter people. This implementation is based on the work of Chris Lovett found in a blog post [here](https://github.com/microsoft/coyote/wiki/Request-Response-Actors)

#### Making Coyote Dependency Inject-able
The first thing we need is to make Coyote runtime available and inject-able for that we need a class and an interface.
```csharp
using Microsoft.Coyote.Actors;
using Microsoft.Coyote.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAspNetCoyote
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
```
I am creating the runtime in the constructor then running the Execute method which is familiar with all of the other samples. In this method the OnFailure method is wired up and any Coyote actors that need to be registered are done here.
It is wired up the normal way as a register service. 
```csharp
public class Startup
   {+
       public Startup(IConfiguration configuration)
       {
           Configuration = configuration;
       }

       public IConfiguration Configuration { get; }

       // This method gets called by the runtime. Use this method to add services to the container.
       public void ConfigureServices(IServiceCollection services)
       {
           IActorRuntime runtime = Microsoft.Coyote.Actors.RuntimeFactory.Create();
           services.AddControllersWithViews();
           services.AddSingleton<ICoyoteRuntime, CoyoteRuntime>();
       }
    .
    .
    .
   }
}
```
It is created as a singleton so a single runtime per server. That sounds about right. If you run your application now it should run fine the runtime is created and that's about it. Right now I am assuming the container will shut down the runtime correctly, I have no idea but it makes sense.

#### Coyote Actors
Now we need some Coyote actors to interact with to prove our runtime is working and we can communicate with our actors. For this I am borrowing the PingPong sample actors from the Original Coyote Samples.

##### RequestResponseActor and RequestEvent
```csharp
public class RequestEvent<TRequest, TResult> : Event
{
    public TRequest Request;
    public Microsoft.Coyote.Tasks.TaskCompletionSource<TResult> Completed = TaskCompletionSource.Create<TResult>();

    public RequestEvent(TRequest request)
    {
        this.Request = request;
    }
}

public class RequestResponseActor<TRequest, TResult> : Actor
{
    private RequestEvent<TRequest, TResult> Request;

    protected override System.Threading.Tasks.Task OnInitializeAsync(Event initialEvent)
    {
        if (initialEvent is RequestEvent<TRequest, TResult> req)
        {
            this.Request = req;
            ProcessRequest(req.Request);
        }
        return base.OnInitializeAsync(initialEvent);
    }

    protected virtual void ProcessRequest(TRequest request)
    {
        throw new NotImplementedException();
    }


    protected void FinishRequest(TResult response)
    {
        this.Request.Completed.SetResult(response);
    }
}
```
##### PingPongServer
```csharp
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
        public ActorId Sender;
    }

    private void HandlePing(Event e)
    {
        if (e is PingEvent p)
        {
            this.SendEvent(p.Caller, new PongEvent()
            {
                Message = "Received: " + p.Message,
                Sender = this.Id
            }); ;
        }
    }

}
```
ExampleHttpServer
```csharp
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
```
Finally we need to go to the index page and update it so we can see the message being PingPonged.
Go to the Home Controller and make the following alterations
On the constructor add the runtime to the constructor to allow it to be injected on start up.
##### HomeController
```csharp
public class HomeController : Controller
   {
       private readonly ILogger<HomeController> _logger;

       public IActorRuntime Runtime { get; }

       public HomeController(ILogger<HomeController> logger, ICoyoteRuntime runtime)
       {
           _logger = logger;
           this.Runtime = runtime.Runtime;
       }

       [HttpGet]
       public async Microsoft.Coyote.Tasks.Task<ActionResult> Index()
       {
           string name = "Johnny";
           var request = new RequestEvent<string, string>(name);
           ActorId id = Runtime.CreateActor(typeof(ExampleHttpServer), request);
           
           var response = await request.Completed.Task;

           Runtime.SendEvent(id, HaltEvent.Instance);
            
           return View("Index", response);
       }

       public IActionResult Privacy()
       {
           return View();
       }

       [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
       public IActionResult Error()
       {
           return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
       }
   }
```
Well almost finally
We need to add the output to the screen so we can see it. Go to the Index.cshtml file and add this to the welcome message
```html
<h4>@ViewData.Model</h4>
```

That's about it. If you set a break point in the startup file and then in the Home controller you should see the code execute through the registration and then in the Home Controller When the welcome screen opens up you should see the message "Received: Johnny"

#### Some Notes:
1. This implementation creates a series of actors (2) on each request to the page. Which means that they need to be shut down after processing the request. To do this we are sending a message to them to stop. From the HomeController we can use the runtime to send a stop message to the ExampleHttpServer (which inherits from the RequestResponseActor)
```csharp
Runtime.SendEvent(id, HaltEvent.Instance);
```
In the ExampleHttpServer we can use the Actor base class of the class to send a halt message to the PingPongServer (which inherits from Actor)
```csharp
this.SendEvent(pe.Sender, HaltEvent.Instance);
```
Thanks to [@lovettchris](https://github.com/lovettchris) for pointing this out to me.

2. Running the Coyote testing framework
I needed to move the coyote actor code into a separate dll to get this to work. I also needed to add a test class with a test method to run as part of the test. Note the TestAttribute, there can be only one so I needed to remove that attribute from the Execute method in the CoyoteRuntime Class.
```csharp
[Test]
public static async Task Execute(IActorRuntime runtime)
{
   var request = new RequestEvent<string, string>("Hi Mom!");
   ActorId id = runtime.CreateActor(typeof(ExampleHttpServer), request);
   var response = await request.Completed.Task;
   runtime.SendEvent(id, HaltEvent.Instance);
}
```
again thanks to Chris for the assistance.



