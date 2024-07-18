using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace ToggleCalculator
{
    internal class Program
    {
        private  static void ConfigureServices(IServiceCollection services)
        {
        }

        static async Task Main(string[] args)
        {
            // check if appsettings file exist
            const string cfgFileName = "appsettings.json";
            
            ShowCurrentDirectoryInfo();    // particularly useful to know where to put ur appsettings file when debugging
            
            if (!File.Exists(cfgFileName))
            {
                Console.WriteLine("Config File Not Found!");
                return;
            }
            
            // Setup Configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(cfgFileName, false)
                .Build();
            
            // Setup Feature Definition Provider
            IFeatureDefinitionProvider featureDefinitionProvider = new ConfigurationFeatureDefinitionProvider(configuration);

            // Setup Feature Manager
            IFeatureManager featureManager = new FeatureManager(
                featureDefinitionProvider, 
                new FeatureManagementOptions());
            
            // Setup application services + feature management
            IServiceCollection services = new ServiceCollection();
            
            services.AddSingleton(configuration);
            services.AddFeatureManagement();
            
            Console.WriteLine("Config loaded!");
            
            var calculator = new Calculator(featureManager);
            await calculator.RunCalculator();
        }

        private static void ShowCurrentDirectoryInfo()
        {
            var curDir = Directory.GetCurrentDirectory();
            var curFiles = Directory.GetFiles(curDir).Select(x => Path.GetFileName(x)).ToArray();
            Console.WriteLine($"Current Path: {curDir}\nCurrent Files:\n");
            foreach (var fileEntry in curFiles)
            {
                Console.Write($"{fileEntry}\t");
            }
        }
    }
}
