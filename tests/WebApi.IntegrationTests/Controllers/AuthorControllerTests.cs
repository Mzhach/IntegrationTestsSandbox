using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using WebApi.IntegrationTests.Fixtures;
using WebApi.Models.Author;

namespace WebApi.IntegrationTests.Controllers;

[Collection("WebApi")]
public class AuthorControllerTests : ControllerTestBase
{
    public AuthorControllerTests(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNewAuthor()
    {
        // Arrange
        var newAuthor = Fixture.Create<NewAuthorDto>();
        
        // Act
        var response = await Client.PostAsJsonAsync("api/authors", newAuthor);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdAuthor = await response.Content.ReadFromJsonAsync<AuthorDto>();
        createdAuthor.Should().NotBeNull();
        createdAuthor.Id.Should().NotBeEmpty();
        createdAuthor.Name.Should().Be(newAuthor.Name);
        
        // Clean up
        var dbContext = GetDbContext();
        var authorFromDb = await dbContext.Authors.FindAsync(createdAuthor.Id);
        dbContext.Authors.Remove(authorFromDb!);
        await dbContext.SaveChangesAsync();
    }
}