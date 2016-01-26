using NUnit.Framework;
using System;
using System.Linq;

namespace MailJet.Client.Tests
{
    [TestFixture]
    public class AccountTests
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
        public void GetAllDNS()
        {
            var result = _client.GetDNS();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
        }

        [Test]
        public void GetSingleDNS_ById()
        {
            var data = _client.GetDNS().Data.First();
            var result = _client.GetDNS(data.ID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Data.Count, 1);
        }

        [Test]
        public void GetSingleDNS_ByDomain()
        {
            var data = _client.GetDNS().Data.First();
            var result = _client.GetDNS(data.Domain);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(data.Domain, result.Data[0].Domain);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Data.Count, 1);
        }

        [Test]
        public void ForceDNSRecheck()
        {
            var data = _client.GetDNS().Data.First();
            var result = _client.ForceDNSRecheck(data.ID);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Data.Count, 1);
        }

        [Test]
        public void MetaSender_Create()
        {
            var email = Guid.NewGuid();
            var result = _client.CreateMetaSender(email + "@mailjet.net", "[TEST]");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data.Single().Email, email + "@mailjet.net");
        }

        [Test]
        public void MetaSender_GetAll()
        {
            var result = _client.GetMetaSender();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, result.Data.Count);
        }

        [Test]
        public void MetaSender_GetSingleById()
        {
            var sender = _client.GetMetaSender().Data.First();
            var result = _client.GetMetaSender(sender.ID);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data.Single().ID, sender.ID);
        }

        [Test]
        public void MetaSender_GetSingleByEmail()
        {
            var sender = _client.GetMetaSender().Data.First();
            var result = _client.GetMetaSender(sender.Email);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data.Single().ID, sender.ID);
        }

        [Test]
        public void MetaSender_Update()
        {
            var sender = _client.GetMetaSender().Data.FirstOrDefault(x => x.Description.Equals("[TEST]"));
            if (sender == null)
                Assert.Inconclusive("Test ran in wrong order and no update could be preformed.");

            var result = _client.UpdateMetaSender(sender.ID, Description: "[TEST2]", IsEnabled: false);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data.Single().ID, sender.ID);
        }
    }
}

