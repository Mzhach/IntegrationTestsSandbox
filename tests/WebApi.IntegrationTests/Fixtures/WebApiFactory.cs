using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace WebApi.IntegrationTests.Fixtures;

[CollectionDefinition("WebApi")]
public class WebApiFactoryCollection : ICollectionFixture<WebApiFactory>
{
    // This class has no code, and is never created
}

[UsedImplicitly]
public class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgsqlContainer;  
    
    // ReSharper disable once ConvertConstructorToMemberInitializers
    public WebApiFactory()
    {
        _pgsqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15.1")
            .Build();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _pgsqlContainer.GetConnectionString();
        
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("DbConnectionString", connectionString)
            });
        });
        
        // Swagger don't work in tests
        builder.UseEnvironment("Test");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
        dbContext.Database.Migrate();
        
        return host;
    }

    public async Task InitializeAsync()
    {
        await _pgsqlContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _pgsqlContainer.StopAsync();
    }
}