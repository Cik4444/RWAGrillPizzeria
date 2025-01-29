using GrillPizzeriaBL.Models;

namespace WebApi.Dto
{
    public class HranaDto
    {
        public string Naslov { get; set; } = null!;

        public string? Opis { get; set; }

        public decimal? Cijena { get; set; }

        public virtual ICollection<HranaAlergen> HranaAlergens { get; } = new List<HranaAlergen>();

        public virtual KategorijaHrane? KategorijaHrane { get; set; }

        public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; } = new List<NarudzbaHrana>();
    }
}
