using WebApi.Models;

namespace WebApi.Dto
{
    public class AlergenDto
    {
        public string Naziv { get; set; } = null!;

        public virtual ICollection<HranaAlergen> HranaAlergens { get; } = new List<HranaAlergen>();
    }
}
