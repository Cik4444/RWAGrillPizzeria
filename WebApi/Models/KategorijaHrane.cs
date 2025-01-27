using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class KategorijaHrane
{
    public int IdkategorijaHrane { get; set; }

    public string Naziv { get; set; } = null!;

    public string? Opis { get; set; }

    public virtual ICollection<Hrana> Hranas { get; } = new List<Hrana>();
}
