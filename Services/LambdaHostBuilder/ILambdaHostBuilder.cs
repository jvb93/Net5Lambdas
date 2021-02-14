using Amazon.Lambda.Core;
using System;

namespace Services.LambdaHostBuilder
{
    public interface ILambdaHostBuilder
    {
        bool TryBuild(ILambdaContext lambdaContext, Func<ILambdaStartup> lambdaStartupFactory, out IServiceProvider serviceProvider);
    }
}
