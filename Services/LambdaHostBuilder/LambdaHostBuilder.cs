using System;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Services.LambdaHostBuilder
{
    public class LambdaHostBuilder : ILambdaHostBuilder
    {
        private IServiceProvider _serviceProvider;
        private static readonly Lazy<LambdaHostBuilder> _host = new Lazy<LambdaHostBuilder>(() => new LambdaHostBuilder());

        public static LambdaHostBuilder Host => _host.Value;

        public bool TryBuild(ILambdaContext lambdaContext, Func<ILambdaStartup> lambdaStartupFactory, out IServiceProvider serviceProvider)
        {
            if (_serviceProvider == null)
            {
                try
                {
                    var serviceCollection = new ServiceCollection();
                    var configuration = BuildConfiguration();
                    serviceCollection.AddSingleton(configuration);
                    lambdaStartupFactory().ConfigureServices(serviceCollection, configuration);

                    _serviceProvider = serviceCollection.BuildServiceProvider();
                }
                catch (Exception e)
                {
                    lambdaContext.Logger.LogLine($"Failed to build lambda host due to: {e}");
                    serviceProvider = null;
                    return false;
                }
            }

            serviceProvider = _serviceProvider;
            return true;
        }

        private IConfiguration BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder();

            configuration.AddEnvironmentVariables();

            return configuration.Build();
        }
    }
}