using System;
using System.Collections.Generic;

namespace cabinetMedical.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? MedecinId { get; set; }

    public string? PatientId { get; set; }
}
