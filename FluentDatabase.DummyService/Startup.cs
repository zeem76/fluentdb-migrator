using System;
using Autofac;
using FluentDatabase.DummyService.Models;
using FluentDatabase.Migrator.MigrationEngine;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluentDatabase.DummyService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    builder => builder
                        .AddPostgres()
                        .WithGlobalConnectionString(Configuration.GetValue<string>(Constants.MainDbConnStr))
                        .WithMigrationsIn(typeof(IDatabaseMigrator).Assembly));
            
            services.AddLogging(lb =>
            {
                lb.SetMinimumLevel(LogLevel.Trace);
            });
        }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Config Values
            var migrationConfig = GetMigrationConfig();
            
            //Registrations
            builder.RegisterType<DatabaseMigrationEngine>().As<IDatabaseMigrationEngine>()
             .WithParameter("rollback", migrationConfig.ToRollback)
             .WithParameter("version",migrationConfig.MigrationVersion)
             .WithParameter("connectionString",migrationConfig.ConnectionString)
             .SingleInstance();
            
        }
        
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            //Pre run task
            ConfigureDatabase(app.ApplicationServices);
        }

        private void ConfigureDatabase(IServiceProvider provider)
        {
            try
            {
                using var scope = provider.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                scope.ServiceProvider.GetService<IDatabaseMigrationEngine>().Run(runner);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"During Startup while trying to initiate database configuration the following exception occurred: {ex}");
                throw;
            }
        }

        private MigrationConfig GetMigrationConfig()
        {
            return new MigrationConfig()
            {
                ConnectionString = Configuration.GetValue<string>(Constants.MainDbConnStr),
                MigrationVersion = Configuration.GetValue<long>(Constants.MainDbVersion),
                ToRollback = Configuration.GetValue<bool>(Constants.MainDbRollback)
            };
        }
    }
}