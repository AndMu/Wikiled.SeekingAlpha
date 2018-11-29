﻿using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Common.Net.Client;
using Wikiled.SeekingAlpha.Api.Request;
using Wikiled.SeekingAlpha.Api.Service;
using Wikiled.Server.Core.Testing.Server;

namespace Wikiled.SeekingAlpha.Service.Tests.Acceptance
{
    [TestFixture]
    public class AcceptanceTests
    {
        private ServerWrapper wrapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            wrapper = ServerWrapper.Create<Startup>(TestContext.CurrentContext.TestDirectory, services => { });
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }

        [Test]
        public async Task Version()
        {
            var response = await wrapper.ApiClient.GetRequest<RawResponse<string>>("api/monitor/version", CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(response.IsSuccess);
        }

        [Test]
        public async Task GetTrackingResults()
        {
            var analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            var result = await analysis.GetTrackingResults(new SentimentRequest("AMD", SentimentType.Article), CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual("AMD", result.Keyword);
            Assert.AreEqual(0, result.Total);
        }

        [Test]
        public async Task GetTrackingHistory()
        {
            var analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            var result = await analysis.GetTrackingHistory(new SentimentRequest("AMD", SentimentType.Article), 10, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(0, result.Length);
        }
    }
}
