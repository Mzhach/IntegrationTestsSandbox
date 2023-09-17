using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.Post;

namespace WebApi.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly Context _context;

    public PostController(Context context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] NewPostDto newPost, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(newPost.Content)) return BadRequest("Content cannot be empty or whitespace");
        
        var isAuthorExists = await _context.Authors
            .Where(x => x.Id == newPost.AuthorId)
            .AnyAsync(cancellationToken);

        if (!isAuthorExists) return BadRequest("Unknown author identifier");
        
        var post = new Post
        {
            AuthorId = newPost.AuthorId,
            Content = newPost.Content,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        await _context.Posts.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new PostDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            Content = post.Content,
            CreatedAt = post.CreatedAt
        });
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FindAsync(new object[] { id }, cancellationToken);

        if (post is null) return NotFound(id);
        
        return Ok(new PostDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            Content = post.Content,
            CreatedAt = post.CreatedAt
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] int count = 10,
        [FromQuery] int position = 0,
        CancellationToken cancellationToken = default)
    {
        var posts = await _context.Posts
            .OrderByDescending(x => x.CreatedAt)
            .Skip(position)
            .Take(count)
            .ToArrayAsync(cancellationToken);

        return Ok(posts.Select(x => new PostDto
        {
            Id = x.Id,
            AuthorId = x.AuthorId,
            Content = x.Content,
            CreatedAt = x.CreatedAt
        }));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FindAsync(new object[] { id }, cancellationToken);

        if (post is null) return NotFound(id);

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Ok(new PostDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            Content = post.Content,
            CreatedAt = post.CreatedAt
        });
    }
}