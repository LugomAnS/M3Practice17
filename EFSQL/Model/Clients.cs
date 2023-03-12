using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFSQL.Model
{
    public partial class Clients
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public string ClientPatronymic { get; set; }
        public string? Phone { get; set; }
        public string EMail { get; set; }
    }
}
