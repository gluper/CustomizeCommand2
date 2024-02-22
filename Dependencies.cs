using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeCommand
{
    static class APEnvironment
    {
        private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

        public static IEngine3 Engine
        {
            get { return s_bag.Value.EngineProvider.Value; }
        }

        public static IObjectManager ObjectMgr
        {
            get { return s_bag.Value.ObjectManagerProvider.Value; }
        }

        public static IOnlineManager OnlineMgr
        {
            get { return s_bag.Value.OnlineManagerProvider.Value; }
        }
        // more access methods and properties...
    }

    class DependencyBag : IDependencyInjectable
    {
        public DependencyBag() { ComponentModel.Singleton.InjectDependencies(this, GetType()); }

        public void InjectionComplete() { }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IEngine3> EngineProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IObjectManager> ObjectManagerProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IOnlineManager> OnlineManagerProvider { get; private set; }

        // more dependencies...
    }
}

