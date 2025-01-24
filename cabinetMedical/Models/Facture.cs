using System;
using System.Collections.Generic;

namespace CabinetMedical.Models;

public partial class Facture
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string? PatientId { get; set; }

    public int ConsultationId { get; set; }


}
