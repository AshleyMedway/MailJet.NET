using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailJet.Client.Response.Data;

namespace MailJet.Client.Tests
{
    [TestFixture]
    public class StatisticsTests
    {
        private MailJetClient _client;

        [SetUp]
        public void Setup()
        {
#if DEBUG
            var publicKey = Environment.GetEnvironmentVariable("MailJetPub", EnvironmentVariableTarget.User);
            var privateKey = Environment.GetEnvironmentVariable("MailJetPri", EnvironmentVariableTarget.User);
#else
            var publicKey = Environment.GetEnvironmentVariable("MailJetPub");
            var privateKey = Environment.GetEnvironmentVariable("MailJetPri");
#endif


            if (String.IsNullOrWhiteSpace(publicKey))
                throw new InvalidOperationException("Add your MailJet public API Key to the Environment Variable \"MailJetPub\".");
            if (String.IsNullOrWhiteSpace(privateKey))
                throw new InvalidOperationException("Add your MailJet private API Key to the Environment Variable \"MailJetPri\".");

            _client = new MailJetClient(publicKey, privateKey);
        }

        /// <summary>
        /// The period of the aggregates (24 hours or 7 days). Allowed values: "7d" or "24h"
        /// </summary>
        [Test]
        public void GetAggregateGraphStatistics()
        {
            //-- Get last message campaign id
            var message = _client.GetMessages().Data.First();

            //-- Create CampaignAggregate from campaign id
            CampaignAggregate lRequestCampaignAggregate = new CampaignAggregate();
            lRequestCampaignAggregate.CampaignIDS = message.CampaignID.ToString();
            lRequestCampaignAggregate.Name = string.Format("Message-{0}", message.ID);

            var lResponseCampaignAggregate = _client.CreateCampaignAggregates(lRequestCampaignAggregate);

            //-- Request statistic Api from campaign aggreate id
            var result = _client.GetAggregateGraphStatistics(Convert.ToInt32(lResponseCampaignAggregate.Data.First().ID), "7d");
            Assert.IsNotNull(result);
        }
    }
}
