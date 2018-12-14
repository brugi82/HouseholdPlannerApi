using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Models.Options
{
    public class EmailOptions
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ReplyToEmail { get; set; }
        public string ReplyToName { get; set; }
        public string WelcomeTemplateName { get; set; }
        public string RegisterTemplateName { get; set; }
		public string InvitationTempateName { get; set; }
    }
}
