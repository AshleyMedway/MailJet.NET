using NUnit.Framework;
using System;
using System.Linq;

namespace MailJet.Client.Tests
{
    [TestFixture]
    public class GetMessages
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

        [Test]
        public void GetMessage_Test()
        {
            var message = _client.GetMessages().Data.First();
            var result = _client.GetMessage(message.ID);
            Assert.IsNotNull(result);
            Assert.AreEqual(message.ID, result.Data.Single().ID);
        }

        [Test]
        public void GetMessages_Test()
        {
            var result = _client.GetMessages();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            //default limit is 10
            Assert.AreEqual(10, result.Data.Count);
            Assert.AreEqual(10, result.Count);
        }

        [Test]
        public void GetMessagesFilterByContact_Test()
        {
            var result = _client.GetMessages(ContactId: 1);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.ContactID == 1));
        }

        [Test]
        public void GetMessagesFilterByCampaign_Test()
        {
            var result = _client.GetMessages(CampaignId: 1);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.CampaignID == 1));
        }

        [Test]
        public void GetMessagesFilterByDestination_Test()
        {
            var result = _client.GetMessages(DestinationId: 42);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.DestinationID == 42));
        }

        [Test]
        public void GetMessagesFilterByMessageState_Test()
        {
            var result = _client.GetMessages(MessageStateId: 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.StateID == 0));
        }


        [Test]
        public void GetMessagesFilterBySender_Test()
        {
            var result = _client.GetMessages(SenderId: 3);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.FromID == 3));
        }

        [Test]
        public void GetMessagesFilterChangeLimit_Increase()
        {
            var result = _client.GetMessages(Limit: 20);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.AreEqual(20, result.Data.Count);
            Assert.AreEqual(20, result.Count);
        }

        [Test]
        public void GetMessagesFilterChangeLimit_Decrease()
        {
            var result = _client.GetMessages(Limit: 3);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.AreEqual(3, result.Data.Count);
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void GetMessageHistory_Test()
        {
            var message = _client.GetMessages().Data.First();
            var result = _client.GetMessage(message.ID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
        }
    }
}

