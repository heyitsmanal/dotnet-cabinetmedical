using System;
using System.Collections.Generic;

namespace cabinetMedical.Models;

public partial class Consultation
{
    public int Id { get; set; }

    public string? PatientId { get; set; }

    public string? MédecinId { get; set; }

    public DateTime? Date { get; set; }

    public int? DossierMédicalId { get; set; }

    public virtual DossierMedical? DossierMédical { get; set; }

    public virtual ICollection<Medicament> Medicaments { get; set; } = new List<Medicament>();

    public virtual Medecin? Médecin { get; set; }

    public virtual Patient? Patient { get; set; }
}
