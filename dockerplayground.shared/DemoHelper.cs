using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public static class DemoHelper
    {
        private static Random _rand = new Random();

        public static Random Rand
        {
            get
            {
                return _rand;
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }


        public static T Random<T>(IEnumerable<T> list)
        {
            return list.ElementAt(Rand.Next(0, list.Count()));
        }

        public static IEnumerable<String> Counterparty => new[]
        {
            "Amundi Asset Management",
            "Natixis Global Asset Management",
            "AXA Investment Managers",
            "BNP Paribas Investment Partners",
            "La Banque Postale Asset Management"
        };

    }
}
