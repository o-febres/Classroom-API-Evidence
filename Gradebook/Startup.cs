using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Gradebook.Controllers;
using Gradebook.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ruffer.Consul;
using Ruffer.Consul.Interfaces;
using Ruffer.Consul.Models;
using Ruffer.Security.Filters;
using Ruffer.Security.Web;
using Ruffer.Security.Web.Interfaces;
using ServiceConnect;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Models;
using SharedContracts.Repositories;
using SharedContracts.Repositories.Data;
using StructureMap;
using ClaimsHelper = Ruffer.Security.Web.ClaimsHelper;
using IClaimsHelper = Ruffer.Security.Web.IClaimsHelper;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Gradebook
{
    public class Startup
    {
        private static IBus _bus;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();

            // Add token authentication
            services.AddScheme<TokenAuthenticationOptions, TokenAuthenticationHandler>("token", options => { });

            InitalizeBus();
            var container = new Container();
            ConfigureContainer(container);
            container.Populate(services);

            return container.GetInstance<IServiceProvider>();
        }

        public void ConfigureContainer(Container container)
        {
            var key = Configuration["TokenKey"];
            var sqlConn = Configuration["connectionString"];
            var publicKey = new X509Certificate2(Convert.FromBase64String(key)).PublicKey.Key as RSACng;
            var consulConfig = new ConsulConnectionConfiguration
            {
                Environment = Configuration["Consul:Environment"],
                Host = Configuration["Consul:Host"],
                Token = Configuration["Consul:Token"]
            };
            //Container Configuration 
            container.Configure(config =>
            {
                //Consul config
                config.For<IConfigurationManager>().Use<ConfigurationManager>()
                    .Ctor<ConsulConnectionConfiguration>("configuration").Is(consulConfig);
                //Ruffer.Security
                config.For<AsymmetricAlgorithm>().Use(publicKey);
                config.For<IEncrytionHelper>().Use<EncrytionHelper>().Ctor<byte[]>("encryptionKey")
                    .Is(Convert.FromBase64String(Configuration["EncryptionKey"]));
                config.For<ITokenHelper>().Use<TokenHelper>();
                config.For<IClaimsHelper>().Use<ClaimsHelper>();
                // Config for all classes and interfaces used in Gradebook
                config.For<IBus>().Use(_bus).Singleton();
                config.For<IGrade>().Use<Grade>();
                config.For<IStudent>().Use<Student>();
                config.For<ILesson>().Use<Lesson>();
                config.For<IClassroom>().Use<Classroom>();
                config.For<DataAccessRepository<Lesson, ILesson>>().Use<LessonRepository>();
                config.For<DataAccessRepository<Student, IStudent>>().Use<StudentRepository>();
                config.For<DataAccessRepository<Classroom, IClassroom>>();
                config.For<StudentController>().Use<StudentController>();
                config.For<ISqlRepository<IStudent>>().Use<StudentSqlRepository>();
                //config.For<SqlDataAccessRepository>().Use<StudentSqlRepository>();
                config.For<IHttpContextAccessor>().Use<HttpContextAccessor>().Singleton();
                config.For<IQueryCountWrapper>().Use<QueryCountWrapper>();
            });
        }



        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMvc();
        }

        public void InitalizeBus() // Establish Connection to RabbitMQ using ServiceConnect - auto begin consuming
        {
            _bus = Bus.Initialize(config =>
            {
                config.SetHost(Configuration["RabbitMQ:RabbitMqHostname"]);
                config.TransportSettings.SslEnabled = true;
                config.TransportSettings.Certs = new X509Certificate2Collection
                {
                    new X509Certificate2(
                        Convert.FromBase64String(Configuration["RabbitMQ:RabbitMqCertBase64"]))
                };
                config.TransportSettings.Username = Configuration["RabbitMQ:RabbitMqUsername"];
                config.TransportSettings.Password = Configuration["RabbitMQ:RabbitMqPassword"];
                config.TransportSettings.CertificateValidationCallback += (sender, certificate, chain, errors) => true;
                config.SetExceptionHandler(ex => Console.WriteLine(ex.Message));
                config.TransportSettings.ServerName = Configuration["RabbitMQ:RabbitMqHostname"];
                config.OutgoingFilters.Add(typeof(TokenInjectTokenFilter));
                config.SetQueueName("ClassroomApi_Producer");
            });
        }
    }
}