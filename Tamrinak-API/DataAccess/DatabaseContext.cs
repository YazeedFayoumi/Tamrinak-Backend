using Microsoft.EntityFrameworkCore;

namespace Tamrinak_API.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
        {
        }


    }
}
