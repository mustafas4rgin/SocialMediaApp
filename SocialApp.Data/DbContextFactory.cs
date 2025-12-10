using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SocialApp.Data.Contexts;

namespace SchoolApp.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=ep-shiny-surf-agw6tuvy-pooler.c-2.eu-central-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_MTPB3g4JvoGn;SSL Mode=Require;Trust Server Certificate=true;");

            return new AppDbContext(optionsBuilder.Options);
        }
        
    }