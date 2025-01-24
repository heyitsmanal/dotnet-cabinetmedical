using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace cabinetMedical.Models
{
    public partial class Medecin
    {
        // Foreign key to IdentityUser
        public string Id { get; set; } = null!;
      
        public string? Nom { get; set; }
        
        public string? Spécialité { get; set; }

        // Navigation property for the IdentityUser
        public virtual IdentityUser IdentityUser { get; set; }

        public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

        public virtual ICollection<DossierMedical> DossierMedicals { get; set; } = new List<DossierMedical>();
    }
}
