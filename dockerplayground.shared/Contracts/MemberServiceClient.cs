using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DockerPlayground.Shared.Dao
{
    public class MemberServiceClient : ServiceBase<IMemberClient>, IMemberClient
    {
        public MemberServiceClient() : base(ServiceConstants.MemberServiceUrl)
        {
        }

        public async Task<IEnumerable<Member>> GetAllMembers()
        {
            return await _client.GetAllMembers();
        }

        public async Task<Member> GetMember(Guid memberId)
        {
            return await _client.GetMember(memberId);
        }
    }
}
