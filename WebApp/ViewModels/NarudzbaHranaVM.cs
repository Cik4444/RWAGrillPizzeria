using GrillPizzeriaBL.Models;

namespace WebApp.ViewModels
{
    public class NarudzbaHranaVM
    {
        public int Kolicina { get; set; }

        public virtual Hrana? Hrana { get; set; }

        public virtual Narudzba? Narudzba { get; set; }
    }
}
