using NUnit.Framework;
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
            _client = new MailJetClient("xxx", "xxx");
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
    }
}

