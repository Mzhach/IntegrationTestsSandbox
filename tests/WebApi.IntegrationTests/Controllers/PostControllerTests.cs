using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoFixture;
using Data.Entities;
using FluentAssertions;
using WebApi.IntegrationTests.Fixtures;
using WebApi.Models.Post;

namespace WebApi.IntegrationTests.Controllers;

[Collection("WebApi")]
public class PostControllerTests : ControllerTestBase
{
    public PostControllerTests(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNewPost()
    {
        // Arrange
        var author = Fixture.Build<Author>()
            .With(x => x.CreatedAt, DateTimeOffset.UtcNow)
            .Create();
        
        var dbContext = GetDbContext();
        await dbContext.Authors.AddAsync(author);
        await dbContext.SaveChangesAsync();

        var newPost = Fixture.Build<NewPostDto>()
            .With(x => x.AuthorId, author.Id)
            .Create();
        
        // Act
        var response = await Client.PostAsJsonAsync("api/posts", newPost);
        
        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdPost = await response.Content.ReadFromJsonAsync<PostDto>();
        createdPost.Should().NotBeNull();
        createdPost.Id.Should().NotBeEmpty();
        createdPost.Content.Should().Be(newPost.Content);
        
        // Clean up
        var postFromDb = await dbContext.Posts.FindAsync(createdPost.Id);
        dbContext.Posts.Remove(postFromDb!);
        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync();
    }
}