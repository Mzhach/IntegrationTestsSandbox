using System;

namespace Data.Entities;

public class Post : EntityBase
{
    public Guid AuthorId { get; set; }
    public virtual Author Author { get; set; }
    public string Content { get; set; }
}