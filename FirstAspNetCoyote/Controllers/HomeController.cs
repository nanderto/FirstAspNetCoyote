using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FirstAspNetCoyote.Models;
using Microsoft.Coyote.Actors;
using FirstCoyoteLibrary;

namespace FirstAspNetCoyote.Controllers
{
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
}
