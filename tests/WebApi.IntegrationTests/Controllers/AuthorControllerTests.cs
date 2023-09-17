using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using WebApi.IntegrationTests.Fixtures;
using WebApi.Models.Author;

namespace WebApi.IntegrationTests.Controllers;

[Collection("WebApi")]
public class AuthorControllerTests
{
    private readonly WebApiFactory _factory;
    private readonly Fixture _fixture;

    public AuthorControllerTests(WebApiFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNewAuthor()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAuthor = _fixture.Create<NewAuthorDto>();
        
        // Act
        var response = await client.PostAsJsonAsync("api/authors", newAuthor);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdAuthor = await response.Content.ReadFromJsonAsync<AuthorDto>();
        createdAuthor.Should().NotBeNull();
        createdAuthor.Id.Should().NotBeEmpty();
        createdAuthor.Name.Should().Be(newAuthor.Name);
    }
}