using WebApp.Models;

namespace WebApp.ViewModels
{
    public class KategorijaHraneVM
    {
        public int IdkategorijaHrane { get; set; }
        public string Naziv { get; set; } = null!;

        public string? Opis { get; set; }

        public virtual ICollection<Hrana> Hranas { get; set; } = new List<Hrana>();
    }
}
