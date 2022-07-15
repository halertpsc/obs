using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class KeyStorage
    {
        private readonly object _locker = new object();
        private string _key;
        public string Key
        {
            get
            {
                lock (_locker)
                {
                    return _key;
                }
            }
            set
            {
                 lock(_locker)
                {
                    _key = value;
                }
            }
        }
    }
}
