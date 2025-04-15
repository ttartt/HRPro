using HRproDatabaseImplement.Models;
using HRProDatabaseImplement.Models;
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
        public virtual DbSet<Document> Documents { set; get; }
        public virtual DbSet<Template> Templates { set; get; }
        public virtual DbSet<Tag> Tags { set; get; }
        public virtual DbSet<Meeting> Meetings { set; get; }
        public virtual DbSet<MeetingParticipant> MeetingParticipants { set; get; }
        public virtual DbSet<DocumentTag> DocumentTags { set; get; }
        public virtual DbSet<Message> Messages { set; get; }
    }
}
