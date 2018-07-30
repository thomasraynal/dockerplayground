using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemberService
{
    [Route("members")]
    public class MembersController : ControllerBase
    {
        private ILogger _logger;
    
        public MembersController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MembersController>();
        }

        [HttpGet("memberId")]
        public IActionResult GetMember(Guid memberId)
        {
            var member = MembersRepository.Members.FirstOrDefault(memb => memb.Id == memberId);
            if(null== member) return this.NotFound($"Member [{memberId}] not found");
            return this.Ok(member);
        }

        [HttpGet]
        public IActionResult GetAllMembers()
        {
            return this.Ok(MembersRepository.Members);
        }
    }
}
