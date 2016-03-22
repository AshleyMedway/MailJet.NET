using MailJet.Client.Enum;
using MailJet.Client.Response.Data;
using NUnit.Framework;
using System;
using System.Linq;

namespace MailJet.Client.Tests
{
    [TestFixture]
    public class ContactMetadataTests
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
        public void GetAllContactMetaData()
        {
            var data = _client.GetContactMetaData();
            Assert.GreaterOrEqual(data.Data.Count, 1);
            Assert.IsNotNull(data.Data.First().Name);
        }

        [Test]
        public void GetContactMetaDataById()
        {
            ContactMetadata item = null;
            try
            {
                var all = _client.GetContactMetaData();
                item = all.Data.First();
            }
            catch (Exception e)
            {
                Assert.Inconclusive("Failed to get a single item to check");
            }

            if (item == null)
            {
                //This should be possible, but still...
                Assert.Inconclusive("Failed to get a single item to check");
            }

            var api = _client.GetContactMetaData(item.ID);
            Assert.IsNotNull(api);
            var apiItem = api.Data.Single();
            Assert.AreEqual(item.ID, apiItem.ID);
            Assert.AreEqual(item.Name, apiItem.Name);
            Assert.AreEqual(item.NameSpace, apiItem.NameSpace);
            Assert.AreEqual(item.Datatype, apiItem.Datatype);
        }


        [Test]
        public void GetContactMetaDataByName()
        {
            ContactMetadata item = null;
            try
            {
                var all = _client.GetContactMetaData();
                item = all.Data.First();
            }
            catch (Exception e)
            {
                Assert.Inconclusive("Failed to get a single item to check");
            }

            if (item == null)
            {
                //This should be possible, but still...
                Assert.Inconclusive("Failed to get a single item to check");
            }

            var api = _client.GetContactMetaData(item.Name, item.NameSpace);
            Assert.IsNotNull(api);
            var apiItem = api.Data.Single();
            Assert.AreEqual(item.ID, apiItem.ID);
            Assert.AreEqual(item.Name, apiItem.Name);
            Assert.AreEqual(item.NameSpace, apiItem.NameSpace);
            Assert.AreEqual(item.Datatype, apiItem.Datatype);
        }

        [Test]
        public void UpdateContactMetaData()
        {
            ContactMetadata item = null;
            try
            {
                var all = _client.GetContactMetaData();
                item = all.Data.Where(x => x.Name.Contains("test") && !x.Name.Contains("updated")).First();
            }
            catch (Exception e)
            {
                Assert.Inconclusive("Failed to get a single item to check");
            }

            if (item == null)
            {
                //This should be possible, but still...
                Assert.Inconclusive("Failed to get a single item to check");
            }


            item.Name = item.Name + "updated";

            var api = _client.UpdateContactMetaData(item);
            Assert.IsNotNull(api);
            var apiItem = api.Data.Single();
            Assert.AreEqual(item.ID, apiItem.ID);
            Assert.AreEqual(item.Name, apiItem.Name);
            Assert.AreEqual(item.NameSpace, apiItem.NameSpace);
            Assert.AreEqual(item.Datatype, apiItem.Datatype);
        }

        [Test]
        public void DeleteContactMetaDataById()
        {
            ContactMetadata item = null;
            try
            {
                var all = _client.GetContactMetaData();
                item = all.Data.Where(x => x.Name.Contains("test") && x.Name.Contains("updated")).First();
            }
            catch (Exception e)
            {
                Assert.Inconclusive("Failed to get a single item to check");
            }

            if (item == null)
            {
                //This should be possible, but still...
                Assert.Inconclusive("Failed to get a single item to check");
            }


            _client.DeleteContactMetaData(item.ID);

            try
            {
                var api = _client.GetContactMetaData(item.ID);
                Assert.Fail("Item was not deleted!"); // should not be able to reach this line!
            }
            catch (Exception)
            {
                Assert.Pass(); // should not be able to find this object anymore
            }
        }

        [Test]
        public void CreateContactMetaData()
        {
            var data = new ContactMetadata()
            {
                Datatype = ContactMetadataDataType.str,
                Name = "test" + DateTime.UtcNow.ToString("ddMMyyhhmmss"),
                NameSpace = ContactMetadataNameSpace.@static
            };

            var result = _client.CreateContactMetaData(data);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count);
            var item = result.Data.Single();
            Assert.AreEqual(data.Name, item.Name);
            Assert.AreEqual(data.NameSpace, item.NameSpace);
            Assert.AreEqual(data.Datatype, item.Datatype);
        }
    }
}
