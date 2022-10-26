using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class FooManager
    {
        private readonly IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        public FooManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}
