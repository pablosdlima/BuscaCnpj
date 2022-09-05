using BuscaCnpj.Models;
using Microsoft.EntityFrameworkCore;

namespace BuscaCnpj.Data
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }
        //public DbSet<Root> Root { get; set; }
    }
}
