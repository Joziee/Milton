using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Database.Infrastructure
{
    public class MiltonEngine : IEngine
    {
        private ContainerManager _container;

        #region Constructor
        /// <summary>
        /// Creates a new instance of the engine and registers all dependencies
        /// </summary>
        public MiltonEngine()
        {
            this.RegisterDependencies();
        }
        #endregion

        #region IEngine
        public ContainerManager ContainerManager
        {
            get { return _container; }
        }

        /// <summary>
        /// Resolve the specified type from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }


        /// <summary>
        /// Resolve the specified type from the container
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        public void InstallAllPlugins()
        {

            //foreach (var manifest in PluginManager.Plugins)
            //{
            //    var plugin = manifest.Instance();
            //    plugin.Install();
            //}
        }
        #endregion

        #region Methods
        /// <summary>
        /// Register dependencies
        /// </summary>
        protected virtual void RegisterDependencies()
        {
            //Create an empty container (we will update it several times since you can only build it once)
            var builder = new ContainerBuilder();
            var container = builder.Build();

            //Register the engine as a dependency
            builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.Update(container);

            //Find other dependency registrars
            var registrars = AppDomain.CurrentDomain
                                          .GetAssemblies()
                                          .SelectMany(a => a.GetTypes())
                                          .Where(t => typeof(IDependencyRegistrar).IsAssignableFrom(t) && !t.IsInterface)
                                          .Select(c => Activator.CreateInstance(c) as IDependencyRegistrar);

            //Register the other dependencies
            foreach (var registrar in registrars)
            {
                builder = new ContainerBuilder();
                registrar.RegisterDependencies(builder);
                builder.Update(container);
            }

            //Set the container in the engine
            this._container = new ContainerManager(container);

            //Set the dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //Set the dependency resolver for Web API
            //GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
        #endregion
    }
}
