using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gestion_Compras.Models
{

	public class UnidadDeMedida
	{
		[Key]
		[Display(Name = "Unidad de Medida")]
		public int Id { get; set; }
		[Required]
		
        public string Descripcion { get; set; }
        
	}
}