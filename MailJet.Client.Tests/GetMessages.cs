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
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
        }

        [Test]
        public void GetMessages_Test()
        {
            var result = _client.GetMessages();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
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

