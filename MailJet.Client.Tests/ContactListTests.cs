using MailJet.Client.Enum;
using MailJet.Client.Request;
using MailJet.Client.Response.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public void CreateContactForList()
        {
            if (_testId == -1)
            {
                var all = _client.GetAllContactLists();
                var item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
                if (item == null)
                {
                    CreateContactList();
                    all = _client.GetAllContactLists();
                    item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
                }

                _testId = item.ID;
                _testAddress = item.Address;
            }
            string email = String.Format("test_{0}@mailjet.net", Guid.NewGuid());
            const string name = "TEST CONTACT";
            var contact = new Contact()
            {
                Action = CreateContactAction.addnoforce,
                Email = email,
                Name = name
            };

            contact.AddProperty("blah", "TestValueForBlahProperty");

            var result = _client.CreateContactForList(_testId, contact);
            var resultItem = result.Data.Single();
            Assert.AreEqual(email, resultItem.Email);
            Assert.AreEqual(name, resultItem.Name);
        }

        [Test]
        public void CreateContactList()
        {
            var result = _client.CreateContactList(String.Format("Test {0:o}", DateTime.UtcNow));
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
                    CreateContactList();
                    all = _client.GetAllContactLists();
                    item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
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
                    CreateContactList();
                    all = _client.GetAllContactLists();
                    item = all.Data.Where(x => x.Name.StartsWith("Test")).FirstOrDefault();
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
                    item = _client.CreateContactList(String.Format("Test {0:o}", DateTime.UtcNow)).Data.Single();
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
                    item = _client.CreateContactList(String.Format("Test {0:o}", DateTime.UtcNow)).Data.Single();
                }

                _testId = item.ID;
                _testAddress = item.Address;
            }
            _client.DeleteContactList(_testAddress);
            _testAddress = String.Empty;
            _testId = -1;
        }

        [Test]
        public void ListRecipient_All()
        {
            var result = _client.GetListRecipient();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(result.Count, result.Data.Count);
        }

        [Test]
        public void ListRecipient_IsActive()
        {
            var result = _client.GetListRecipient(IsActive: true);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.IsActive));
        }

        [Test]
        public void ListRecipient_IsBlocked()
        {
            //TODO: Write test
            Assert.Ignore("Not sure how to test");
        }

        [TestCase(true)]
        public void ListRecipient_ByContactId(bool firstrun)
        {
            try
            {
                var result = _client.GetListRecipient();
                var item = result.Data.First();
                var api = _client.GetListRecipient(ContactId: item.ContactID);
                Assert.IsNotNull(api);
                Assert.IsNotNull(api.Data);
                Assert.AreEqual(api.Count, api.Data.Count);
                Assert.IsTrue(api.Data.All(x => x.ContactID == item.ContactID));
            }
            catch (InvalidOperationException)
            {
                if (firstrun)
                {
                    CreateContactForList();
                    ListRecipient_ByContactId(false);
                }
                else
                {
                    throw;
                }
            }
        }

        [Test]
        public void ListRecipient_ByContactEmail()
        {
            Assert.Ignore("Test not complete, need to be able to query /contact");
            //var result = _client.GetListRecipient();
            //var item = result.Data.First();
            //TODO: remove hardcoded test
            var api = _client.GetListRecipient(ContactEmail: "test_33a8282d-e66e-4dab-86c9-1fcc548cba55@mailjet.net");
            Assert.IsNotNull(api);
            Assert.IsNotNull(api.Data);
            Assert.AreEqual(api.Count, api.Data.Count);
            //TODO: remove hardcoded test
            Assert.IsTrue(api.Data.All(x => x.ContactID == 1689224808));
        }

        [Test]
        public void ListRecipient_ByContactListId()
        {
            var result = _client.GetListRecipient();
            var item = result.Data.First();
            var api = _client.GetListRecipient(ContactsListId: item.ListID);
            Assert.IsNotNull(api);
            Assert.IsNotNull(api.Data);
            Assert.AreEqual(api.Count, api.Data.Count);
            Assert.IsTrue(api.Data.All(x => x.ListID == item.ListID));
        }

        [Test]
        public void ListRecipient_ByContactListIdAndContactId()
        {
            var result = _client.GetListRecipient();
            var item = result.Data.First();
            var api = _client.GetListRecipient(ContactsListId: item.ListID, ContactId: item.ContactID);
            Assert.IsNotNull(api);
            Assert.IsNotNull(api.Data);
            Assert.AreEqual(api.Count, api.Data.Count);
            Assert.IsTrue(api.Data.All(x => x.ListID == item.ListID && x.ContactID == item.ContactID));
        }


        [Test]
        public void ListRecipient_ByContactListIdAndContactIdAndUnSub_False()
        {
            var result = _client.GetListRecipient();
            var item = result.Data.First();
            var api = _client.GetListRecipient(ContactsListId: item.ListID, ContactId: item.ContactID, Unsub: false);
            Assert.IsNotNull(api);
            Assert.IsNotNull(api.Data);
            Assert.AreEqual(api.Count, api.Data.Count);
            Assert.IsTrue(api.Data.All(x => x.ListID == item.ListID && x.ContactID == item.ContactID && !x.IsUnsubscribed));
        }

        [Test]
        public void ListRecipient_IgnoreDeleted()
        {
            //TODO: Write test
            Assert.Ignore("Not sure how to test");
        }

        [Test]
        public void ListRecipient_ByLastActivityAt()
        {
            //TODO: Write test
            Assert.Ignore("Not sure how to test");
        }

        [Test]
        public void ListRecipient_ByListName()
        {
            Assert.Ignore("Test not complete, need to be able to query /contact");
            //var result = _client.GetListRecipient();
            //var item = result.Data.First();
            //TODO: remove hardcoded test
            var api = _client.GetListRecipient(ListName: "Test 07-03-2016 09:05:45");
            Assert.IsNotNull(api);
            Assert.IsNotNull(api.Data);
            Assert.AreEqual(api.Count, api.Data.Count);
            //TODO: remove hardcoded test
            Assert.IsTrue(api.Data.All(x => x.ListID == 1486008));
        }

        [Test]
        public void ListRecipient_ByOpenedEmail()
        {
            //TODO: Write test
            Assert.Ignore("Not sure how to test");
        }

        [Test]
        public void ListRecipient_ByStatus()
        {
            //TODO: Write test
            Assert.Ignore("Not sure how to test");
        }

        [Test]
        public void ListRecipient_ByUnSub_False()
        {
            var result = _client.GetListRecipient(Unsub: false);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => !x.IsUnsubscribed));
        }

        [Test]
        public void ListRecipient_ByUnSub_True()
        {
            var result = _client.GetListRecipient(Unsub: true);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(result.Count, result.Data.Count);
            Assert.IsTrue(result.Data.All(x => x.IsUnsubscribed));
        }

        [TestCase(true)]
        public void ContactDataUpdate_ById(bool firstrun)
        {
            try
            {
                var result = _client.GetListRecipient();
                var item = result.Data.First();
                var data = _client.UpdateContactData(item.ContactID, new Dictionary<string, object>()
                {
                    { "Blah", String.Format("Updated: {0:ddMMyy hhmmss}", DateTime.UtcNow) }
                });
            }
            catch (InvalidOperationException)
            {
                if (firstrun)
                {
                    CreateContactForList();
                    ContactDataUpdate_ById(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

