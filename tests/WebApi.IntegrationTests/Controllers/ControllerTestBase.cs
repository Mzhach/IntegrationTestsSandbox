using System;
using System.Net.Http;
using AutoFixture;
using Data;
using Microsoft.Extensions.DependencyInjection;
using WebApi.IntegrationTests.Fixtures;

namespace WebApi.IntegrationTests.Controllers;

public abstract class ControllerTestBase : IDisposable
{
    protected readonly HttpClient Client;
    protected readonly Fixture Fixture;

    private readonly IServiceScope _serviceProvider;

    protected ControllerTestBase(WebApiFactory factory)
    {
        Client = factory.CreateClient();
        _serviceProvider = factory.Services.CreateScope();
        Fixture = new Fixture();
    }

    protected Context GetDbContext() => _serviceProvider.ServiceProvider.GetRequiredService<Context>();

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}