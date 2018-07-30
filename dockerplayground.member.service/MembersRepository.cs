using DockerPlayground.Shared.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MemberService
{
    public class MembersRepository
    {
        private static List<Member> _members;

        static MembersRepository()
        {
            _members = Enumerable.Range(0, 10).Select(_ => Member.Generate()).ToList();
        }

        public static List<Member> Members
        {
            get
            {
                return _members;
            }
        }
    }
}
