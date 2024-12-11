using Gestion_Compras.Models;

namespace Gestion_Compras.ViewModels
{
    public class FamiliaSubFamiliaViewModel
    {
        public Familia Familia { get; set; }
        public SubFamilia SubFamilia { get; set; }

         public IEnumerable<Familia> FamiliaList { get; set; }
    }
}


