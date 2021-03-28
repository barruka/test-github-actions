using apiExample.Api;
using apiExample.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace apiExample.Tests
{
    public class DemoIntegration
    {
        [Fact]
        public async Task CustomerIntegrationTest()
        {
            // Create DB Context
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

            var context = new CustomerContext(optionsBuilder.Options);

            // Just to make sure: Delete all existing customers in the DB
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            //context.Customers.RemoveRange(await context.Customers.ToArrayAsync());
            //await context.SaveChangesAsync();

            // Create Controller
            var controller = new CustomersController(context);

            // Add customer
            await controller.Add(new Customer() { CustomerName = "John Doe" });

            // Check: Does GetAll return all the added customers?
            var result = (await controller.GetAll()).ToArray();
            Assert.Single(result);
            Assert.Equal("John Doe", result[0].CustomerName);
        }
    }
}

