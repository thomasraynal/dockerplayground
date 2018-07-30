using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class Member
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        public static Member Generate()
        {
            return new Member()
            {
                Id = Guid.NewGuid(),
                Name = DemoHelper.RandomString(10)
            };
        }
    }
}
