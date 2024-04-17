using Microsoft.EntityFrameworkCore;

namespace Chinook
{
    public static class DataInitializer
    {
        public static void ExecuteEfMigration(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ChinookContext>();
            context.Database.Migrate();
        }
    }
}
