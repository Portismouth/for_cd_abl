using System;

namespace TheWall.Models
{
  public class Comment : BaseEntity
  {
    public int CommentId { get; set; }
    public string CommentText { get; set; }
    public int MessageId { get; set; }
    public Message Message { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}