namespace webapi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Breeze.AspNetCore;
    using Microsoft.AspNetCore.Mvc;
    using UnoTest.Web.Data;

    //[Route("api")]
    [Route("breeze/[controller]/[action]")]
    [BreezeQueryFilter]
    public class CustomerController : Controller
    {
        private UnoTestDbContext context;

        private UnoTestPersistenceManager PersistenceManager;

        public CustomerController(
            UnoTestDbContext context)
        {
            this.context = context;
            PersistenceManager = new UnoTestPersistenceManager(context);
        }

        [HttpGet]
        public IActionResult Metadata()
        {
            return Ok(PersistenceManager.Metadata());
        }

        [HttpGet]
        public IQueryable<Customer> Customers()
        {
            var customers = PersistenceManager.Context.Customers;
            return customers;
        }
    }
}
