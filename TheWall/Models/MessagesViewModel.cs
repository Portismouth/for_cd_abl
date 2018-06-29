using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
  public class MessagingViewModel : BaseEntity
  {
    [Required]
    [MinLength (5, ErrorMessage = "Please enter an actual message.")]
    public string MessageText { get; set; }

    [MinLength (5, ErrorMessage = "Please enter an actual comment.")]
    public string CommentText { get; set; }
  }
}