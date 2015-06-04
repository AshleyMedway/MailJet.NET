using NUnit.Framework;
using System;
using System.IO;
using System.Net.Mail;

namespace MailJet.Client.Tests
{
    [TestFixture]
    public class SendMail
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
        public void MailMessage_Text_NoAttachements()
        {
            var message = BaseMessage();
            message.Body = "test";
            var result = _client.SendMessage(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void MailMessage_Text_WithDisplayName()
        {
            var message = new MailMessage();
#if DEBUG
            var testFrom = Environment.GetEnvironmentVariable("MailJetTestFrom", EnvironmentVariableTarget.User);
            var testTo = Environment.GetEnvironmentVariable("MailJetTestTo", EnvironmentVariableTarget.User);
#else
            var testFrom = Environment.GetEnvironmentVariable("MailJetTestFrom");
            var testTo = Environment.GetEnvironmentVariable("MailJetTestTo");
#endif
            message.To.Add(new MailAddress(testTo, "To Test"));
            message.From = new MailAddress(testFrom, "To From");
            message.Subject = "test";
            message.Body = "test";
            var result = _client.SendMessage(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void MailMessage_Text_WithAttachements()
        {
            var message = BaseMessage();
            message.Body = "test";
            var path = Path.Combine(Environment.CurrentDirectory, "TestData", "TextFile.txt");
            message.Attachments.Add(new Attachment(path));
            var result = _client.SendMessage(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void MailMessage_Html_NoAttachements()
        {
            var message = BaseMessage();
            message.Body = "<b>TEST</b>";
            message.IsBodyHtml = true;
            var result = _client.SendMessage(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        private MailMessage BaseMessage()
        {
#if DEBUG
            var testFrom = Environment.GetEnvironmentVariable("MailJetTestFrom", EnvironmentVariableTarget.User);
            var testTo = Environment.GetEnvironmentVariable("MailJetTestTo", EnvironmentVariableTarget.User);
#else
            var testFrom = Environment.GetEnvironmentVariable("MailJetTestFrom");
            var testTo = Environment.GetEnvironmentVariable("MailJetTestTo");
#endif


            var message = new MailMessage()
            {
                From = new MailAddress(testFrom),
                Subject = "test"
            };
            message.To.Add(new MailAddress(testTo));
            return message;
        }
    }
}

