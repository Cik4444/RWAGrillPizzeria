﻿using GrillPizzeriaBL.Models;

namespace WebApp.ViewModels
{
    public class NarudzbaVM
    {
        public int Idnarudzba { get; set; }
        public DateTime Datum { get; set; }

        public virtual Korisnik? Korisnik { get; set; }

        public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; set; } = new List<NarudzbaHrana>();
    }
}
