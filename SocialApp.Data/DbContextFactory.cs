using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SocialApp.Data.Contexts;

namespace SchoolApp.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=dpg-d3onpbs9c44c738bpgng-a.oregon-postgres.render.com;Database=s4rgin;Username=s4rgin_user;Password=AuIbUXiPZ5ThC0T6EEw6o1xtrTzkAYTZ;Port=5432;Ssl Mode=Require;Trust Server Certificate=true");

            return new AppDbContext(optionsBuilder.Options);
        }
        
    }