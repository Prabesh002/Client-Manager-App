using Client_Manager_App.Entities;
using Microsoft.EntityFrameworkCore;

namespace Client_Manager_App.AppDb
{
    public class AppDatabase : DbContext
    {
        public AppDatabase(DbContextOptions<AppDatabase> options) : base(options)
        {

        }
        public DbSet<ClientModel> Clients { get; set; }

    }
}
