using Breeze.Sharp;
using System;
using UnoTest.Web.Data;

namespace UnoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GetData();
            Console.ReadLine();
        }

        private async static void GetData()
        {
            var serviceAddress = "http://localhost:53333/breeze/Customer/";

            var assembly = typeof(Customer).Assembly;
            var rslt = Configuration.Instance.ProbeAssemblies(assembly);

            var entityManager = new EntityManager(serviceAddress);

            var query = new EntityQuery<Customer>();

            // query = query.Where(c => c.FirstName == "Adam1");

            try
            {
                var result = await entityManager.ExecuteQuery(query);
                foreach (var customer in result)
                {
                    Console.WriteLine($"{customer.FirstName} {customer.LastName}");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);

            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
