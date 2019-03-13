using Microsoft.AspNetCore.Identity;
using System;

namespace DotHass.Sample.Model.Identity
{
    public class GameUser : IdentityUser
    {
        public string RealName { get; set; }

        public bool Sex { get; set; }

        public string RetailId { get; set; }


        public DateTimeOffset CreateDate { get; set; }

        public DateTimeOffset LoginDate { get; set; }

        public long GameRoleId { get; set; }
    }
}
