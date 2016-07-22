using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailJet.Client.Response.Data
{
    /// <summary>
    /// http://dev.mailjet.com/guides/#send-api-json-properties
    /// </summary>
    public class MailjetSendMail
    {
       
        public MailjetSendMail()
        {

        }
        public MailjetSendMail(MailMessage pMailMessage)
        {

        }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string Subject { get; set; }

        public string Textpart { get; set; }

        [JsonProperty("Htmlpart")]
        public string HtmlPart { get; set; }

        public List<MailjetRecipient> Recipients { get; set; }

        public List<MailjetRecipient> Cc { get; set; }

        public List<MailjetRecipient> Bcc { get; set; }

        public List<MailjetAttachment> Attachments { get; set; }

        /// <summary>
        /// Groups multiple messages in one campaign
        /// Equivalent of using X-Mailjet-Campaign header through SMTP.
        /// </summary>
        /// <see cref="https://app.mailjet.com/docs/emails_headers"/>
        [JsonProperty("Mj-campaign")]
        public string MjCampaign { get; set; }

        /// <summary>
        /// Block/unblock messages to be sent multiple times inside one campaign to the same contact.
        ///- 0: unblocked(default behavior)
        ///- 1: blocked
        ///Equivalent of using X-Mailjet-DeduplicateCampaign header through SMTP.
        ///Can only be used if mj-campaign is specified.
        /// </summary>
        /// <see cref="https://app.mailjet.com/docs/emails_headers"/>
        [JsonProperty("Mj-deduplicatecampaign")]
        public int MjDeduplicateCampaign { get; set; }

        /// <summary>
        /// The Template ID or Name to use as this email content. Overrides the HTML/Text parts if any.
        /// MANDATORY IF NO HTML/TEXT - MAX TEMPLATEID: 1
        /// </summary>
        [JsonProperty("Mj-TemplateID")]
        public int MjTemplateID { get; set; }

        /// <summary>
        /// Activate the template language processing. By default the template language processing is desactivated. Use True to activate.
        ///Equivalent to using the X-MJ-TemplateLanguage header through SMTP.
        /// </summary>
        /// <see cref="http://dev.mailjet.com/guides/#use-the-template-in-send-api"/>
        [JsonProperty("Mj-TemplateLanguage")]
        public bool MJTemplateLanguage { get; set; }


        /// <summary>
        /// Force or disable open tracking on this message, can overriding account preferences.
        /// Equivalent of using X-Mailjet-TrackOpen header through SMTP.
        /// Can only be used with a HTML part.
        ///        - 0: take the values defined on the account.The ones shown here
        ///        - 1: disable the tracking
        ///        - 2: enable the tracking
        /// </summary>
        /// <see cref="https://app.mailjet.com/docs/emails_headers"/>
        [JsonProperty("Mj-trackopen")]
        public int MjTrackopen { get; set; }


        /// <summary>
        /// Force or disable click tracking on this message, can overriding account preferences.
        /// Equivalent to using the X-Mailjet-TrackClick header through SMTP.
        /// Can only be specified if the HTML part is provided.
        ///           - 0: take the values defined on the account.The ones shown here
        ///         - 1: disable the tracking
        ///         - 2: enable the tracking
        /// </summary>
        /// <see cref="https://app.mailjet.com/docs/emails_headers"/>
        [JsonProperty("Mj-trackclick")]
        public int MjTrackclick { get; set; }

        /// <summary>
        /// Attach a custom ID to the message
        /// Equivalent to using the X-MJ-CustomID header through SMTP.
        /// </summary>
        /// <see cref="http://dev.mailjet.com/guides/#sending-an-email-with-a-custom-id"/>
        [JsonProperty("Mj-CustomID")]
        public int MjCustomID { get; set; }

        /// <summary>
        /// Attach a payload to the message
        /// Equivalent to using the X-MJ-EventPayload header through SMTP.
        /// </summary>
        /// <see cref="http://dev.mailjet.com/guides/#sending-an-email-with-a-payload"/>
        [JsonProperty("Mj-EventPayLoad")]
        public int MjEventPayLoad { get; set; }

        public Dictionary<string, object> Vars { get; set; }

        public Dictionary<string, string> Headers { get; set; }

    }


    public class MailjetRecipient
    {
        public MailjetRecipient()
        {

        }
        public MailjetRecipient(string pEmail, string pName = null)
        {
            Email = pEmail;
            Name = pName;
        }
        public string Email { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Vars { get; set; }
    }

   


    /// <summary>
    /// {"Content-type": "MIME TYPE", "Filename": "FILENAME.EXT", "content":"BASE64 ENCODED CONTENT"}
    /// </summary>
    public class MailjetAttachment
    {
        /// <summary>
        /// MIME TYPE
        /// </summary>
        [JsonProperty("Content-type")]
        public string ContentType { get; set; }
        /// <summary>
        /// FILENAME.EXT
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// BASE64 ENCODED CONTENT
        /// </summary>
         [JsonProperty("content")]
        public string Content { get; set; }
    }



}
