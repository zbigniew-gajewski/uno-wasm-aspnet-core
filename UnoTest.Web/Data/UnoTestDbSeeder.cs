namespace UnoTestWeb.Data
{
    using Microsoft.AspNetCore.Hosting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnoTest.Web.Data;

    public class UnoTestDbSeeder
    {
        private readonly UnoTestDbContext ctx;
        private readonly IHostingEnvironment hosting;

        public UnoTestDbSeeder(
            UnoTestDbContext ctx,
            IHostingEnvironment hosting)
        {
            this.ctx = ctx;
            this.hosting = hosting;
        }

        public Task Seed()
        {
            return Task.Run(() => {
                ctx.Database.EnsureCreated();

                if (!ctx.Customers.Any())
                {
                    ctx.Customers.AddRange(
                        new List<Customer>(
                        new[] {
                        new Customer { FirstName = "Adam1", LastName = "Novak1" },
                        new Customer { FirstName = "Adam2", LastName = "Novak2" },
                        new Customer { FirstName = "Adam3", LastName = "Novak3" }
                        }));
                    ctx.SaveChanges();
                }
            });

        }
    }
}
