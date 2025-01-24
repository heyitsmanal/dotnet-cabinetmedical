using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace cabinetMedical.Models
{
    public partial class Admin
    {
        // Foreign key to IdentityUser
        public string Id { get; set; } = null!;

        public string? Nom { get; set; }

        // Navigation property for the IdentityUser
        public virtual IdentityUser IdentityUser { get; set; }
    }
}
