using System;
using System.Collections.Generic;

namespace cabinetMedical.Models;

public partial class DossierMedical
{
    public int Id { get; set; }

    public string? PatientId { get; set; }

    public string? MédecinId { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCréation { get; set; }

    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public virtual Medecin? Médecin { get; set; }

    public virtual Patient? Patient { get; set; }
}
