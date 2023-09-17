using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Author;

namespace WebApi.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorController : ControllerBase
{
    private readonly Context _context;

    public AuthorController(Context context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] NewAuthorDto newAuthor, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(newAuthor.Name)) return BadRequest("Name cannot be empty or whitespace");
        
        var author = new Author
        {
            Name = newAuthor.Name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _context.Authors.AddAsync(author, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            CreatedAt = author.CreatedAt
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var author = await _context.Authors.FindAsync(new object[] { id }, cancellationToken);

        if (author is null) return NotFound();

        return Ok(new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            CreatedAt = author.CreatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var author = await _context.Authors.FindAsync(new object[] { id }, cancellationToken);
        if (author is null) return NotFound();
        
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Ok(new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            CreatedAt = author.CreatedAt
        });
    }
}