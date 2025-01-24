using System;
using System.Collections.Generic;

namespace cabinetMedical.Models;

public partial class Planning
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public DateTime? DateDébut { get; set; }

    public DateTime? DateFin { get; set; }
}
