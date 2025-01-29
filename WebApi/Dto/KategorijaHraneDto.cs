using GrillPizzeriaBL.Models;

namespace WebApi.Dto
{
    public class KategorijaHraneDto
    {
        public string Naziv { get; set; } = null!;

        public string? Opis { get; set; }

        public virtual ICollection<Hrana> Hranas { get; } = new List<Hrana>();
    }
}
