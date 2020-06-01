using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThirdAspNetApplication.Shared;

namespace ThirdAspNetApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public ThirdAspNetApplication.Shared.Ping Get()
        {
            return new Ping();
        }
    }
}
