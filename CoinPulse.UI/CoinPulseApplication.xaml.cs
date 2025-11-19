using System.Windows;
using CoinPulse.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CoinPulse.UI
{
    public partial class CoinPulseApplication : Application
    {
        public new static CoinPulseApplication Current => (CoinPulseApplication)Application.Current;
        public IServiceProvider Services { get; }

        [STAThread]
        public static void Main()
        {
            var app = new CoinPulseApplication();
            app.Run();
        }
        
        public CoinPulseApplication()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }
        
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<MainWindow>();
            services.AddTransient<MainViewModel>();

            services.AddHttpClient<ICoinService, CoinGeckoService>()
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            
            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}    