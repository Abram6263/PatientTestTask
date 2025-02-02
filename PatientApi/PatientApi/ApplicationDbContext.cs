using Microsoft.EntityFrameworkCore;
using PatientApi.Models;

namespace PatientApi;

public class ApplicationDbContext : DbContext
{
   public DbSet<Patient> Patients { get; set; }
   public DbSet<Name> Names { get; set; }

   public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
   {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Patient>()
         .HasOne(p => p.Name)
         .WithOne(n => n.Patient)
         .HasForeignKey<Name>(n => n.PatientId);
      
      base.OnModelCreating(modelBuilder);
   }
}