using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Infrastructure
{
    public class EngineContext
    {
        /// <summary>
        /// Initializes a static instance of the factory
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Singleton<IEngine>.Instance = CreateEngineInstance();
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <returns>New engine instance</returns>
        protected static IEngine CreateEngineInstance()
        {
            return new MiltonEngine();
        }

        /// <summary>
        /// Get the currenty instance of the engine
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }
    }
}
