using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Hrana
{
    public int Idhrana { get; set; }

    public string Naslov { get; set; } = null!;

    public string? Opis { get; set; }

    public decimal? Cijena { get; set; }

    public int? KategorijaHraneId { get; set; }

    public virtual ICollection<HranaAlergen> HranaAlergens { get; } = new List<HranaAlergen>();

    public virtual KategorijaHrane? KategorijaHrane { get; set; }

    public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; } = new List<NarudzbaHrana>();
}
