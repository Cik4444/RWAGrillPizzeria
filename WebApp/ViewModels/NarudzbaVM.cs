using WebApp.Models;

namespace WebApp.ViewModels
{
    public class NarudzbaVM
    {
        public DateTime Datum { get; set; }

        public virtual Korisnik? Korisnik { get; set; }

        public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; set; } = new List<NarudzbaHrana>();
    }
}
