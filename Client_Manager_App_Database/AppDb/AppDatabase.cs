using Client_Manager_App;
using Microsoft.EntityFrameworkCore;
using Client_Manager_App_Models;
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
