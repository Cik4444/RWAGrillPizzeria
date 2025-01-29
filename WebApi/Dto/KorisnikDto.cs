using GrillPizzeriaBL.Models;

namespace WebApi.Dto
{
    public class KorisnikDto
    {
        public string Ime { get; set; } = null!;

        public string Prezime { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Lozinka { get; set; }

        public virtual ICollection<NarudzbaDto> Narudzbas { get; } = new List<NarudzbaDto>();

    }
}
