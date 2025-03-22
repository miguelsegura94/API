using BBDD.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDD
{
    public class AMSEDbContext : DbContext
    {
        public AMSEDbContext(DbContextOptions<AMSEDbContext> options) : base(options) { }

        public DbSet<Usuario> usuarios { get; set; }
        //public DbSet<Elo> Elos { get; set; }
    }
}
