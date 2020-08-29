using AutoMapper;
using LimeHomeTest.Dto.CrossCut;
using LimeHomeTest.Repository;
using LimeHomeTest.Repository.Models;
using LimeHomeTest.Services;
using LimeHomeTest.Services.Contracts;
using LimeHomeTest.Services.Mapper;
using LimeHomeTest.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.IO;
using System.Reflection;

namespace LimeHomeTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()                
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)                
                .AddJsonFile($"limeConfig.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.Configure<HereApi>(options => Configuration.GetSection("HereApi").Bind(options));

            services.AddDbContext<Context>(opt => opt.UseInMemoryDatabase("LimeDB"));
            services.AddScoped<IService, Service>();
            services.AddScoped<IDBRepository, DBRepository>();
            services.AddScoped<IRestClient, RestClient>();
            services.AddScoped<IMessanger, Messanger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(),"Logs/myapp-{Date}.txt"));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");               
            });

          

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<Context>();
                AddTestData(context);
            }
           

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddTestData(Context context)
        {
            var testProperty = new Hotel
            {
                Id = 1,
                Title = "Test Hotel",
                Distance=100
            };

            context.Hotels.Add(testProperty);

            context.SaveChanges();
        }
    }
}