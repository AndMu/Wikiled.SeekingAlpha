﻿using System;
using Autofac;
using System.Net;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Retriever;
using Wikiled.SeekingAlpha.Api.Containers;

namespace Wikiled.SeekingAlpha.Api.Tests.Helpers
{
    public class NetworkHelper : IDisposable
    {
        public NetworkHelper()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.RegisterModule(
                new RetrieverModule(
                    new RetrieveConfiguration
                    {
                        LongRetryDelay = 1000,
                        CallDelay = 100,
                        LongRetryCodes = new[] {HttpStatusCode.Forbidden},
                        RetryCodes = new[]
                                     {
                                         HttpStatusCode.Forbidden,
                                         HttpStatusCode.RequestTimeout, // 408
                                         HttpStatusCode.InternalServerError, // 500
                                         HttpStatusCode.BadGateway, // 502
                                         HttpStatusCode.ServiceUnavailable, // 503
                                         HttpStatusCode.GatewayTimeout // 504
                                     },
                        MaxConcurrent = 1
                    }));

            builder.RegisterModule(new AlphaModule("Data", "AMD"));
            Container = builder.Build();
            Retrieval = Container.Resolve<ITrackedRetrieval>();
        }

        public IContainer Container { get; }

        public ITrackedRetrieval Retrieval { get; }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}