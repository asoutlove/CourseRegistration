using System;

namespace CoreProject.Models
{
    public class AccountViewModel
    {
        public Guid Manager_ID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string department { get; set; }
        public string Account { get; set; }
        public string password { get; set; }
    }
}
