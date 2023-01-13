using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class KeyStorage
    {
        public volatile string _key;
        public string Key
        {
            get => _key;
            set => _key = value;
        }
    }
}
