using System;

namespace CoreProject.Helpers
{
    public class LoginInfo
    {
        public Guid ID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public UserLevel Level { get; set; } 

    }
}
