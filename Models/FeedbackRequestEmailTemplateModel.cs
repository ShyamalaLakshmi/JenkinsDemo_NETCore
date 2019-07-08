using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class FeedbackRequestEmailTemplateModel
    {
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public string ParticipantName { get; set; }
        public string FeedbackUrl { get; set; }
    }
}
