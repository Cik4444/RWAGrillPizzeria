using WebApi.Models;

namespace WebApi.Dto
{
    public class NarudzbaDto
    {
        public DateTime Datum { get; set; }

        public virtual Korisnik? Korisnik { get; set; }

        public virtual ICollection<NarudzbaHrana> NarudzbaHranas { get; set; } = new List<NarudzbaHrana>();
    }
}

