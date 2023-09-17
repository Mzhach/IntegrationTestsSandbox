using System;

namespace WebApi.Models.Post;

public class NewPostDto
{
    public Guid AuthorId { get; set; }
    public string Content { get; set; }
}