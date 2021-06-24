using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer
{
    public class DataServer
    {
        private IServiceProvider _provider;
        public DataServer(IServiceProvider provider)
        {
            _provider = provider;
        }
    }
}
