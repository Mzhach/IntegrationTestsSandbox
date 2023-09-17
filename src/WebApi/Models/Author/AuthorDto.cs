using System;

namespace WebApi.Models.Author;

public class AuthorDto : NewAuthorDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}