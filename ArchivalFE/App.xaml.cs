using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using ArchivalDataRepository.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMDataRepository.Context;
using Serilog;

namespace ArchivalFE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string PM_CONNECTION_STRING = "PMConnection";
        public const string ARCHIVAL_CONNECTION_STRING = "ArchivalConnection";
        public const string APPLICATION_NAME = "ArchivalFE";
        private Serilog.ILogger? logger;
        public IServiceProvider? ServiceProvider { get; private set; }
        public IConfiguration? Configuration { get; private set; }
        private static int tenMegabytes = 10 * 1024 * 1024;
        protected override void OnStartup(StartupEventArgs e)
        {

            logger = new LoggerConfiguration().WriteTo.File("/logs/startupLog.txt", rollOnFileSizeLimit: true, fileSizeLimitBytes: tenMegabytes).CreateLogger();

            //base.OnStartup(e);
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
;

            Configuration = builder.Build();

            var logConfiguration = new LoggerConfiguration().ReadFrom.Configuration(Configuration);
            logger = logConfiguration.CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        public void ConfigureServices(ServiceCollection services)
        {
            if (Configuration == null) throw new ArgumentException("Configuration not loaded");

            services.AddScoped<IConfiguration>(_ => Configuration);

            services.AddLogging(builder => builder.AddSerilog(logger));

            services.AddDbContext<PlayerManagementContext>(x =>
            {
                x.UseSqlServer(UpdateApplicationNameInConnectionString(Configuration.GetConnectionString(PM_CONNECTION_STRING)));
            });
            services.AddDbContext<PmdataArchivalContext>(x =>
            {
                x.UseSqlServer(UpdateApplicationNameInConnectionString(Configuration.GetConnectionString(ARCHIVAL_CONNECTION_STRING)));
            });

            
            services.AddTransient(typeof(MainWindow));

        }
        private string UpdateApplicationNameInConnectionString(string? connectionString)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            csb.ApplicationName = APPLICATION_NAME;
            return csb.ConnectionString;
        }
    }

}
