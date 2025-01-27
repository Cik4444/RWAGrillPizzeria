using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class NarudzbaHrana
{
    public int IdnarudzbaHrana { get; set; }

    public int? NarudzbaId { get; set; }

    public int? HranaId { get; set; }

    public int Kolicina { get; set; }

    public virtual Hrana? Hrana { get; set; }

    public virtual Narudzba? Narudzba { get; set; }
}
