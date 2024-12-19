using HRproDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;

namespace HRproDatabaseImplement
{
    public class HRproDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=HRproDatabase;Username=postgres;Password=12345");
            }
            base.OnConfiguring(optionsBuilder);
        }
        public virtual DbSet<User> Users { set; get; }
        public virtual DbSet<Company> Companies { set; get; }
        public virtual DbSet<Vacancy> Vacancies { set; get; }
        public virtual DbSet<Resume> Resumes { set; get; }
    }
}
