using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace ToggleCalculator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Initiating the program...");
            // ShowCurrentDirectoryInfo();    // particularly useful to know where to put your appsettings file when debugging | Obsolete with AdjustCurrentDirectory
            AdjustCurrentDirectory();
            // check if appsettings file exist
            Console.WriteLine("Checking for the config file...");
            const string cfgFileName = "appsettings.json";
            if (!File.Exists(cfgFileName))
            {
                Console.WriteLine("Config File Not Found!");
                return;
            }
            Console.WriteLine("File found! Loading...");
            
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
            
            Console.WriteLine("Configuration loaded!");
            
            var shouldUseCache = await featureManager.IsEnabledAsync("CacheFlags");
            var calculator = new Calculator(featureManager, shouldUseCache);
            await calculator.RunCalculator();
        }

        [Obsolete("Make sure to use the AdjustCurrentDirectory method", false)]
        private static void ShowCurrentDirectoryInfo()
        {
            var curDir = Directory.GetCurrentDirectory();
            var curFiles = Directory.GetFiles(curDir).Select(Path.GetFileName).ToArray();
            Console.WriteLine($"Current Path: {curDir}\nCurrent Files:\n");
            foreach (var fileEntry in curFiles)
            {
                Console.Write($"{fileEntry}\t");
            }
        }

        private static void AdjustCurrentDirectory()
        {
            Console.WriteLine("Adjusting the current directory...");
            var curDir = Directory.GetCurrentDirectory();
#if DEBUG            
            const string debugPath = "bin/Debug/net8.0/";
            Directory.SetCurrentDirectory(Path.GetRelativePath(debugPath, curDir));
#endif
#if RELEASE
            const string releasePath = "bin/Release/net8.0/";
            Directory.SetCurrentDirectory(Path.GetRelativePath(releasePath, curDir));
#endif
            Console.WriteLine("Done!");
        }
    }
}
