namespace UnoTest.Web.Data
{
    using Microsoft.EntityFrameworkCore;

    public partial class UnoTestDbContext : DbContext
    {
        public UnoTestDbContext()
        {
            // Configuration.ProxyCreationEnabled = false;
            // Configuration.LazyLoadingEnabled = false;
        }

        public UnoTestDbContext(DbContextOptions<UnoTestDbContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UnoTest;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }

        public DbSet<Customer> Customers { get; set; }
    }
}
