using Microsoft.AspNetCore.Mvc;
using Gestion_Compras.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Gestion_Compras.ViewModels;


namespace Gestion_Compras.Controllers
{
    [Route("[controller]")]
    public class FamiliaSubFamiliaController : Controller
    {
        private readonly DataContext context;

        public FamiliaSubFamiliaController(DataContext context)
        {
            this.context = context;
        }

        // Acción para mostrar el formulario
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var familias = await context.Familia
                .Select(f => new Familia
                {
                    Id = f.Id,
                    Descripcion = f.Descripcion
                })
                .ToListAsync();

            var viewModel = new FamiliaSubFamiliaViewModel
            {
                FamiliaList = familias
            };

            return View(viewModel);
        }


        // Acción para crear una familia y subfamilia
        [HttpPost]
        public async Task<ActionResult> CrearFamiliaYSubfamilia([FromBody] FamiliaSubFamiliaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Primero, creamos la familia
            var familiaExistente = await context.Familia
                .Where(f => f.Codigo == modelo.Familia.Codigo)
                .FirstOrDefaultAsync();

            if (familiaExistente != null)
            {
                return Conflict("Código de Familia Existente.");
            }

            context.Familia.Add(modelo.Familia);
            await context.SaveChangesAsync();

            // Ahora, creamos la subfamilia y la asociamos con la familia creada
            var subFamilia = modelo.SubFamilia;
            var codigoFamilia = modelo.Familia.Codigo;
            var subFamilias = await context.SubFamilia
                .Where(sf => sf.FamiliaId == modelo.SubFamilia.FamiliaId)
                .ToListAsync();

            var maxCodigo = subFamilias
                .Select(sf => int.TryParse(sf.Codigo.Substring(codigoFamilia.Length), out var result) ? result : 0)
                .DefaultIfEmpty(0)
                .Max();

            var nuevoCodigo = $"{codigoFamilia}{(maxCodigo + 1).ToString("D2")}";
            subFamilia.Codigo = nuevoCodigo;

            context.SubFamilia.Add(subFamilia);
            await context.SaveChangesAsync();

            return Ok(new { message = "Familia y SubFamilia creadas exitosamente", familia = modelo.Familia, subFamilia });
        }

        

    }
}
