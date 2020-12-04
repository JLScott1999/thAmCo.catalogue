namespace ThAmCo.Catalogue.Data
{
    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<ProductData> ProductData { get; set; }

    }
}