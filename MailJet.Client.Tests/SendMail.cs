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
            var publicKey = Environment.GetEnvironmentVariable("MailJetPub", EnvironmentVariableTarget.Machine);
            var privateKey = Environment.GetEnvironmentVariable("MailJetPri", EnvironmentVariableTarget.Machine);

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
            var testFrom = Environment.GetEnvironmentVariable("MailJetTestFrom", EnvironmentVariableTarget.Machine);
            var testTo = Environment.GetEnvironmentVariable("MailJetTestTo", EnvironmentVariableTarget.Machine);

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

