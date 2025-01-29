using GrillPizzeriaBL.Models;

namespace WebApp.ViewModels
{
    public class HranaVM
    {

        public int Idhrana { get; set; }
        public string Naslov { get; set; } = null!;

        public string? Opis { get; set; }

        public decimal? Cijena { get; set; }

        public virtual ICollection<HranaAlergen> HranaAlergens { get; set; } = new List<HranaAlergen>();

        public virtual KategorijaHrane? KategorijaHrane { get; set; }

        public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; set; } = new List<NarudzbaHrana>();
    }
}
