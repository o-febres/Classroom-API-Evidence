using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Consumer_dotFramework.Handlers.Sql;
using Ruffer.Consul.Interfaces;
using Ruffer.Consul.Models;
using Ruffer.Security.Filters;
using ServiceConnect;
using ServiceConnect.Container.StructureMap;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;
using SharedContracts.Repositories;
using StructureMap;

namespace Consumer_dotFramework
{
    internal class Program
    {
        private static IBus _bus;

        private static void Main(string[] args)
        {
            // Start connection - consuming
            InitalizeBus();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        public static void ConfigureContainer(Container container)
        {
            var key = ConfigurationManager.AppSettings["TokenKey"];
            var encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            var publicKey = new X509Certificate2(Convert.FromBase64String(key)).PublicKey.Key as RSACryptoServiceProvider;

            var consulConfig = new ConsulConnectionConfiguration
            {
                Environment = ConfigurationManager.AppSettings["ConsulEnvironment"],
                Host = ConfigurationManager.AppSettings["ConsulHost"],
                Token = ConfigurationManager.AppSettings["ConsulToken"]
            };
            container.Configure(config =>
            {
                //Set container to work with contract classes
                config.For<IGrade>().Use<Grade>();
                config.For<IStudent>().Use<Student>();
                config.For<ILesson>().Use<Lesson>();
                config.For<IClassroom>().Use<Classroom>();
                config.For<IRepository<Student, IStudent>>().Use<StudentRepository>();
                config.For<ISqlRepository<IStudent>>().Use<StudentSqlRepository>();
                config.For<SqlGetAllStudentsHandler>().Use<SqlGetAllStudentsHandler>();
                //consul
                config.For<IConfigurationManager>().Use<Ruffer.Consul.ConfigurationManager>()
                    .Ctor<ConsulConnectionConfiguration>("configuration").Is(consulConfig);

                // security
                config.For<AsymmetricAlgorithm>().Use(publicKey);
                config.For<TokenDecryptFilter>()
                    .Use<TokenDecryptFilter>()
                    .Ctor<RSA>("publicKey").Is(publicKey)
                    .Ctor<string>("encryptionKey").Is(encryptionKey);
                config.For<IClaimsHelper>().Use<ClaimsHelper>();
            });
        }

        public static void InitalizeBus() // Establish Connection using ServiceConnect
        {
            var container = new Container();
            ConfigureContainer(container);
            _bus = Bus.Initialize(config =>
            {
                // var host = ConfigurationManager.AppSettings["RabbitMqHostname"];
                config.SetContainer(container);
                config.SetHost(ConfigurationManager.AppSettings["RabbitMqHostname"]);
                config.TransportSettings.SslEnabled = true;
                config.TransportSettings.Certs = new X509Certificate2Collection
                {
                    new X509Certificate2(
                        Convert.FromBase64String(ConfigurationManager.AppSettings["RabbitMqCertBase64"]))
                };
                config.TransportSettings.Username = ConfigurationManager.AppSettings["RabbitMqUsername"];
                config.TransportSettings.Password = ConfigurationManager.AppSettings["RabbitMqPassword"];
                config.TransportSettings.CertificateValidationCallback += (sender, certificate, chain, errors) => true;
                config.SetExceptionHandler(ex => Console.WriteLine(ex.Message)); // able to catch errors on bus
                config.TransportSettings.ServerName = ConfigurationManager.AppSettings["RabbitMqHostname"];

                //Consumes the token message

                config.BeforeConsumingFilters.Add(typeof(TokenDecryptFilter));
                config.SetQueueName("ClassroomApi_Consumer");
                // Assigns the ques for the replies to allow the endpoint application to cosume
                config.AddQueueMapping(typeof(GetAllStudentsReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(GetAllStudentsQueryReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(CreateStudentReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(DeleteStudentReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(GetStudentIdReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(GetStudentLastNameReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(UpdateStudentReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(CreateStudentGradeReplyMessage), "ClassroomApi_Producer");
                config.AddQueueMapping(typeof(UpdateStudentGradeReplyMessage), "ClassroomApi_Producer");
                config.TransportSettings.PurgeQueueOnStartup = true; // removes all outstanding messages on exchange on startup
            });
        }
    }
}