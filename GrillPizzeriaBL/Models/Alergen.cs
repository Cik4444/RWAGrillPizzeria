using System;
using System.Collections.Generic;

namespace GrillPizzeriaBL.Models;

public partial class Alergen
{
    public int Idalergen { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<HranaAlergen> HranaAlergens { get; } = new List<HranaAlergen>();
}
