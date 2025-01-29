using GrillPizzeriaBL.Models;

namespace WebApp.ViewModels
{
    public class AlergenVM
    {
        public string Naziv { get; set; } = null!;

        public virtual ICollection<HranaAlergen> HranaAlergens { get; } = new List<HranaAlergen>();
    }
}
