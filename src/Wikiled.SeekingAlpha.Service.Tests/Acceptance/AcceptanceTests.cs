﻿using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Sentiment.Tracking.Api.Request;
using Wikiled.Sentiment.Tracking.Api.Service;
using Wikiled.Server.Core.Testing.Server;

namespace Wikiled.SeekingAlpha.Service.Tests.Acceptance
{
    [TestFixture]
    public class AcceptanceTests
    {
        private ServerWrapper wrapper;

        private SentimentTracking analysis;

        [OneTimeSetUp]
        public void SetUp()
        {
            wrapper = ServerWrapper.Create<Startup>(TestContext.CurrentContext.TestDirectory, services => { });
            analysis = new SentimentTracking(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
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
            var result = await analysis.GetTrackingResults(new SentimentRequest("AMD", "TSLA"), CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result["AMD"][0].TotalMessages);
        }

        [Test]
        public async Task GetTrackingHistory()
        {
            var result = await analysis.GetTrackingHistory(new SentimentRequest("AMD", "TSLA"), CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result["AMD"].Length);
        }
    }
}
