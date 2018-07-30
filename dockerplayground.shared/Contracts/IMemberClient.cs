using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DockerPlayground.Shared.Dao
{
    public interface IMemberClient
    {
        [Get("/members/{memberId}")]
        Task<Member> GetMember(Guid memberId);

        [Get("/members")]
        Task<IEnumerable<Member>> GetAllMembers();
    }
}
