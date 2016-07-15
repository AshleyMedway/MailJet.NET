namespace MailJet.Client.Response.Data
{
    public class AggregateGraphStatistics : DataItem
    {
        /// <summary>
        /// Number of blocked messages.
        /// </summary>
        public double BlockedCount { get; set; }

        /// <summary>
        /// BlockedStdDev count.
        /// </summary>
        public double BlockedStdDev { get; set; }

        /// <summary>
        /// Number of bounced messages.
        /// </summary>
        public double BouncedCount { get; set; }

        /// <summary>
        /// BouncedStdDev count.
        /// </summary>
        public double BouncedStdDev { get; set; }

        /// <summary>
        /// The ID of campaign aggregate object that corresponds with the statistics.
        /// </summary>
        public long CampaignAggregateID { get; set; }

        /// <summary>
        /// Number of registered clicks.
        /// </summary>
        public double ClickedCount { get; set; }

        /// <summary>
        /// ClickedStdDev count.
        /// </summary>
        public double ClickedStdDev { get; set; }

        /// <summary>
        /// Number of open registrations.
        /// </summary>
        public double OpenedCount { get; set; }

        /// <summary>
        /// OpenedStdDev count.
        /// </summary>
        public double OpenedStdDev { get; set; }

        /// <summary>
        /// Reference time in textual form.
        /// </summary>
        public long RefTimestamp { get; set; }

        /// <summary>
        /// Number of sent messages.
        /// </summary>
        public double SentCount { get; set; }

        /// <summary>
        /// SentStdDev count.
        /// </summary>
        public double SentStdDev { get; set; }

        /// <summary>
        /// Number of spam complains.
        /// </summary>
        public double SpamComplaintCount { get; set; }

        /// <summary>
        /// SpamcomplaintStdDev count.
        /// </summary>
        public double SpamcomplaintStdDev { get; set; }

        /// <summary>
        /// Number of registered unsubscribe requests.
        /// </summary>
        public double UnsubscribedCount { get; set; }

        /// <summary>
        /// UnsubscribedStdDev count.
        /// </summary>
        public double UnsubscribedStdDev { get; set; }
    }
}
