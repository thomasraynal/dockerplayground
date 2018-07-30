using EventStore.ClientAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Proximity.Service
{
    [Route("proximity")]
    public class ProximityController : ControllerBase
    {
        private readonly IProximityService _service;

        public ProximityController(IProximityService service)
        {
            _service = service;
            _service.Start();
        }

        [HttpGet("memberId")]
        public IActionResult GetStateForMember(Guid memberId)
        {
            return this.Ok(_service.GetStateForMember(memberId));
        }

        [HttpGet]
        public IActionResult GetCurrentState()
        {
            return this.Ok(_service.GetState());
        }
    }
}
