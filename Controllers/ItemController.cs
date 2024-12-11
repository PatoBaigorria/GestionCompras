using Microsoft.AspNetCore.Mvc;
using Gestion_Compras.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gestion_Compras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly DataContext context;

        public ItemController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet("ByFamiliaDescripcion/{descripcionFamilia}")]
        public async Task<ActionResult<IEnumerable<object>>> GetItemsByFamiliaDescripcion(string descripcionFamilia)
        {
            var familia = await context.Familia
                .Where(f => f.Descripcion == descripcionFamilia)
                .Select(f => new { f.Id, f.Codigo, f.Descripcion })
                .FirstOrDefaultAsync();

            if (familia == null)
            {
                return NotFound("Familia no encontrada.");
            }

            var subFamilias = await context.SubFamilia
                .Where(sf => sf.FamiliaId == familia.Id)
                .Select(sf => new { sf.Id, sf.Codigo, sf.Descripcion })
                .ToListAsync();

            if (!subFamilias.Any())
            {
                return NotFound("No se encontraron subfamilias para esta familia.");
            }

            var items = await context.Item
                .Where(i => subFamilias.Select(sf => sf.Id).Contains(i.SubFamiliaId))
                .Select(i => new
                {
                    CodigoFamilia = familia.Codigo,
                    DescripcionFamilia = familia.Descripcion,
                    CodigoSubFamilia = i.SubFamilia.Codigo,
                    DescripcionSubFamilia = i.SubFamilia.Descripcion,
                    i.Descripcion
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("ByFamiliaYSubfamilia/{descripcionFamilia}/{descripcionSubfamilia}")]
        public async Task<ActionResult<IEnumerable<object>>> GetItemsByFamiliaYSubfamilia(string descripcionFamilia, string descripcionSubfamilia)
        {
            var familia = await context.Familia
                .Where(f => f.Descripcion == descripcionFamilia)
                .Select(f => new { f.Id, f.Codigo, f.Descripcion })
                .FirstOrDefaultAsync();

            if (familia == null)
            {
                return NotFound("Familia no encontrada.");
            }

            var subFamilia = await context.SubFamilia
                .Where(sf => sf.FamiliaId == familia.Id && sf.Descripcion == descripcionSubfamilia)
                .Select(sf => new { sf.Id, sf.Codigo, sf.Descripcion })
                .FirstOrDefaultAsync();

            if (subFamilia == null)
            {
                return NotFound("Subfamilia no encontrada.");
            }

            var items = await context.Item
                .Where(i => i.SubFamiliaId == subFamilia.Id)
                .Select(i => new
                {
                    CodigoFamilia = familia.Codigo,
                    DescripcionFamilia = familia.Descripcion,
                    CodigoSubFamilia = subFamilia.Codigo,
                    DescripcionSubFamilia = subFamilia.Descripcion,
                    i.Descripcion
                })
                .ToListAsync();

            return Ok(items);
        }


        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subFamilia = await context.SubFamilia
                .Where(sf => sf.Id == item.SubFamiliaId)
                .FirstOrDefaultAsync();

            if (subFamilia == null)
            {
                return BadRequest("Subfamilia no encontrada.");
            }

            var subFamiliaCodigo = subFamilia.Codigo;

            var items = await context.Item
                .Where(i => i.SubFamiliaId == item.SubFamiliaId)
                .ToListAsync();

            var maxCodigo = items
                .Select(i => int.TryParse(i.Codigo?.Substring(subFamiliaCodigo.Length), out var result) ? result : 0)
                .DefaultIfEmpty(0)
                .Max();

            var nuevoCodigo = $"{subFamiliaCodigo}{(maxCodigo + 1).ToString("D3")}";
            item.Codigo = nuevoCodigo;

            if (string.IsNullOrEmpty(item.Codigo))
            {
                return BadRequest("No se pudo generar el código del item.");
            }

            context.Item.Add(item);
            await context.SaveChangesAsync();

            // Devolver un mensaje de éxito
            return Ok(new { message = "Item creado con éxito", item });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            context.Entry(item).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            context.Item.Remove(item);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return context.Item.Any(e => e.Id == id);
        }
    }
}
