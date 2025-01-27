using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Korisnik
{
    public int Idkorisnik { get; set; }

    public string Ime { get; set; } = null!;

    public string Prezime { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Lozinka { get; set; }

    public string? Salt { get; set; }

    public virtual ICollection<Narudzba> Narudzbas { get; } = new List<Narudzba>();
}
