namespace UnoWeb.Test
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Newtonsoft.Json.Serialization;
    using System.IO;

    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<OouiTestDbContext>(cfg =>
            //{
            //    //cfg.UseInMemoryDatabase("OouiTest");
            //    cfg.UseSqlServer(configuration.GetConnectionString("OouiTestConnectionString"));
            //});

            // services.AddTransient<OouiTestDbSeeder>();

            var mvcBuilder = services.AddMvc();

            #region Breeze
            //mvcBuilder.AddJsonOptions(opt => {
            //    var ss = JsonSerializationFns.UpdateWithDefaults(opt.SerializerSettings);
            //    var resolver = ss.ContractResolver;
            //    if (resolver != null)
            //    {
            //        var res = resolver as DefaultContractResolver;
            //        res.NamingStrategy = null;  // <<!-- this removes the camelcasing
            //    }
            //});

            //mvcBuilder.AddMvcOptions(o => { o.Filters.Add(new GlobalExceptionFilter()); });

            #endregion

            //var connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=OouiTest;Trusted_Connection=True";
            //services.AddDbContext<OouiTestDbContext>(options => options.UseSqlServer(connection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Set up custom content types - associating file extension to MIME type
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".wasm"] = "application/wasm";
            provider.Mappings[".dll"] = "application/dll";
            provider.Mappings[".clrdll"] = "application/clrdll";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseWebSockets();
            //app.UseMiddleware<WebSocketMiddleware>();

   
            var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
            var newWwwRootDirectory = Path.Combine(parentDirectory.FullName, "UnoTest", "UnoTest.Wasm", "bin", "Debug", "netstandard2.0", "dist");

            app.UseDefaultFiles();

            //app.UseDefaultFiles("/uno");

            //app.UseDefaultFiles(new DefaultFilesOptions
            //{
            //    DefaultFileNames = new[] { "index.html" },
            //    // RequestPath = "/uno"
            //});


            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                FileProvider = new PhysicalFileProvider(newWwwRootDirectory),
                //RequestPath = "/uno"
            });        

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //if (env.IsDevelopment())
            //{
            //    using (var scope = app.ApplicationServices.CreateScope())
            //    {
            //        var seeder = scope.ServiceProvider.GetService<OouiTestDbSeeder>();
            //        seeder.Seed().Wait();
            //    }
            //}
        }
    }
}
