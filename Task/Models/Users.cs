using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task
{
    using System;
    using System.Collections.Generic;

    public partial class Users
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public Nullable<System.DateTime> SignupDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }
        public Nullable<System.DateTime> BlockedDate { get; set; }
    }
}