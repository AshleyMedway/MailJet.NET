# MailJet .NET (unofficial)
An unofficial .NET Client for use with MailJet v3 API.

[![Build Status](https://travis-ci.org/AshleyMedway/MailJet.NET.svg?branch=master)](https://travis-ci.org/AshleyMedway/MailJet.NET)
[![Gitter](https://badges.gitter.im/AshleyMedway/MailJet.NET.svg)](https://gitter.im/AshleyMedway/MailJet.NET?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=body_badge)
[![NuGet](https://img.shields.io/nuget/dt/MailJet.NET.svg)](https://www.nuget.org/packages/MailJet.NET)

Currently implemented features:  

 * Sending Messages & Getting MessageInfo
 * Some account management (DNS & MetaSender) - [api doc](http://dev.mailjet.com/email-api/v3/dns/)
 * Contact list management (create/sub/unsub/etc.) - [api doc](http://dev.mailjet.com/email-api/v3/contactslist/)
 * Contact Metadata management (CRUD) - [api doc](http://dev.mailjet.com/email-api/v3/contactmetadata/)

Next features will be (unless anyone raises issues otherwise):

 * Completing account management (Sender, MyProfile & User) - [api doc](http://dev.mailjet.com/email-api/v3/myprofile/)
 * Contact management - [api doc](http://dev.mailjet.com/email-api/v3/contact/)


The package uses `System.Net.Mail.MailMessage` for sending outgoing mail as this hopefully allows for simple transition from using `System.Net.Mail.SmtpClient`.  
If you find any features of `MailMessage` have not been implemented please raise an issue.


If there is a specific feature of the API you would like please create an issue or fork, develop, pull request :smiley:

###Basic Usage

    MailJetClient client = new MailJetClient("{PublicKey}", "{PrivateKey}");
    client.SendMessage(new System.Net.Mail.MailMessage("from@email.com", "to@email.com", "subject", "email body"));

For a more detailed usage you can see the [SendMail](https://github.com/AshleyMedway/MailJet.NET/blob/master/MailJet.Client.Tests/SendMail.cs) UnitTests.
