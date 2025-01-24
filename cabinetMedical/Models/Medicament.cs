using System;
using System.Collections.Generic;

namespace cabinetMedical.Models;

public partial class Medicament
{
    public int Id { get; set; }

    public string? Nom { get; set; }

    public string? Description { get; set; }

    public int? ConsultationId { get; set; }

    public virtual Consultation? Consultation { get; set; }
}
