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
        public void GetMessages_Test()
        {
            var result = _client.GetMessages();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
            Assert.AreEqual(result.Count, result.Data.Count);
        }
    }
}

