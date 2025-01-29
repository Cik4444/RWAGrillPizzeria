using System;
using System.Collections.Generic;

namespace GrillPizzeriaBL.Models;

public partial class Narudzba
{
    public int Idnarudzba { get; set; }

    public int? KorisnikId { get; set; }

    public DateTime Datum { get; set; }

    public virtual Korisnik? Korisnik { get; set; }

    public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; } = new List<NarudzbaHrana>();
}
