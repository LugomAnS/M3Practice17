using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFAccess.Models
{
    public partial class Purchases
    {
        public int? Id { get; set; }
        public string EMail { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
    }
}
