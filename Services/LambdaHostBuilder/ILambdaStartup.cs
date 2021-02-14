using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Services.LambdaHostBuilder
{
    public interface ILambdaStartup
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
