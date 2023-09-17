using System;

namespace WebApi.Models.Post;

public class PostDto : NewPostDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}