using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class HranaAlergen
{
    public int IdhranaAlergen { get; set; }

    public int? HranaId { get; set; }

    public int? AlergenId { get; set; }

    public virtual Alergen? Alergen { get; set; }

    public virtual Hrana? Hrana { get; set; }
}
