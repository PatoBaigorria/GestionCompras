using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gestion_Compras.Models
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<Familia> Familia { get; set; }

		public DbSet<SubFamilia> SubFamilia { get; set; }

        public DbSet<Item> Item { get; set; }

        public DbSet<UnidadDeMedida> UnidadDeMedida { get; set; }
		
		public DbSet<Usuario> Usuario { get; set; }
	}
}