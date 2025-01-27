using WebApi.Models;

namespace WebApi.Dto
{
    public class NarudzbaHranaDto
    {
        public int Kolicina { get; set; }

        public virtual Hrana? Hrana { get; set; }

        public virtual Narudzba? Narudzba { get; set; }
    }
}
