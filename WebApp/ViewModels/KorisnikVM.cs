using System.ComponentModel.DataAnnotations;
using GrillPizzeriaBL.Models;

namespace WebApp.ViewModels
{
    public class KorisnikVM
    {
        public int Idkorisnik { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan email format.")]
        public string Email { get; set; }
        public string? Lozinka { get; set; }

        public virtual ICollection<Narudzba> Narudzbas { get; } = new List<Narudzba>();
    }
}
