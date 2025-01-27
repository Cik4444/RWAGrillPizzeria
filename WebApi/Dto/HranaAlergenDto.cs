using WebApi.Models;

namespace WebApi.Dto
{
    public class HranaAlergenDto
    {
        public virtual Alergen? Alergen { get; set; }

        public virtual Hrana? Hrana { get; set; }
    }
}
