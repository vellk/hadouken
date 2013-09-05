﻿using System;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Http;
using Hadouken.Plugins.Events.Rpc;
using InjectMe;
using InjectMe.Registration;

namespace Hadouken.Plugins.Events
{
    public class EventsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = BuildContainer(config);
            return container.ServiceLocator.Resolve<Plugin>();
        }

        private static IContainer BuildContainer(IBootConfig config)
        {
            return
                Container.Create(
                    containerConfiguration => BuildContainerConfiguration(containerConfiguration, config));
        }

        private static void BuildContainerConfiguration(IContainerConfiguration cfg, IBootConfig config)
        {
            string baseUri = String.Concat("http://", config.HostBinding, ":", config.Port);

            cfg.Register<IEventServer>().AsSingleton().UsingFactory(() => new EventServer(baseUri + "/events"));

            cfg.Register<IJsonRpcServer>().AsTransient().UsingConcreteType<HttpJsonRpcServer>();
            cfg.Register<IHttpUriFactory>().AsTransient().UsingFactory(() => new HttpUriFactory(baseUri + "/"));

            cfg.Register<IRequestBuilder>().AsTransient().UsingConcreteType<RequestBuilder>();

            cfg.Register<IRequestHandler>().AsTransient().UsingConcreteType<RequestHandler>();

            // Register RPC methods
            cfg.Register<IRpcMethod>().AsTransient().UsingFactory(() => new EventsPublish(baseUri + "/events"));

            cfg.Register<Plugin>().AsTransient().UsingConcreteType<EventsPlugin>();
        }
    }
}
