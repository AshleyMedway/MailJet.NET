using MailJet.Client.Response.Data;
using NUnit.Framework;
using System;
using System.Linq;

namespace MailJet.Client.Tests
{
    [TestFixture]
    public class ContactListTests
    {
        private MailJetClient _client;
        private static long _testId = -1;
        private static string _testAddress = String.Empty;

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
        public void CreateContactList()
        {
            var result = _client.CreateContactList(String.Format("Test {0:dd-MM-yyyy hh:mm:ss}", DateTime.UtcNow));
            Assert.IsNotNull(result);
            var item = result.Data.Single();
            _testId = item.ID;
            _testAddress = item.Address;
        }

        [Test]
        public void GetAllContactLists()
        {
            var result = _client.GetAllContactLists();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, result.Data.Count);
        }

        [Test]
        public void GetContactListById()
        {
            ContactListData item;
            if (_testId == -1)
            {
                var all = _client.GetAllContactLists();
                item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
                if (item == null)
                {
                    Assert.Fail("Could not find a test item to test this method");
                }

                _testId = item.ID;
                _testAddress = item.Address;
            }
            var result = _client.GetContactList(_testId);
            Assert.IsNotNull(result);
            item = result.Data.Single();
            Assert.AreEqual(_testId, item.ID);
            Assert.AreEqual(_testAddress, item.Address);
        }

        [Test]
        public void GetContactListByAddress()
        {
            ContactListData item;
            if (_testAddress.Equals(String.Empty))
            {
                var all = _client.GetAllContactLists();
                item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
                if (item == null)
                {
                    Assert.Fail("Could not find a test item to test this method");
                }

                _testId = item.ID;
                _testAddress = item.Address;
            }
            var result = _client.GetContactList(_testAddress);
            Assert.IsNotNull(result);
            item = result.Data.Single();
            Assert.AreEqual(_testId, item.ID);
            Assert.AreEqual(_testAddress, item.Address);
        }

        [Test]
        public void DeleteContactListById()
        {
            if (_testId == -1)
            {
                var all = _client.GetAllContactLists();
                var item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
                if (item == null)
                {
                    item = _client.CreateContactList(String.Format("Test {0:dd-MM-yyyy hh:mm:ss}", DateTime.UtcNow)).Data.Single();
                }

                _testId = item.ID;
                _testAddress = item.Address;
            }
            _client.DeleteContactList(_testId);
            _testAddress = String.Empty;
            _testId = -1;
        }

        [Test]
        public void DeleteContactListByAddress()
        {
            if (_testAddress.Equals(String.Empty))
            {
                var all = _client.GetAllContactLists();
                var item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
                if (item == null)
                {
                    item = _client.CreateContactList(String.Format("Test {0:dd-MM-yyyy hh:mm:ss}", DateTime.UtcNow)).Data.Single();
                }

                _testId = item.ID;
                _testAddress = item.Address;
            }
            _client.DeleteContactList(_testAddress);
            _testAddress = String.Empty;
            _testId = -1;
        }
    }
}

