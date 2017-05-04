using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Infrastructure
{
    public interface IDependencyRegistrar
    {
        void RegisterDependencies(ContainerBuilder builder);
    }
}
