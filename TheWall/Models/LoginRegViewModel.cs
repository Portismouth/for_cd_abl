using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
  public class RegisterViewModel : BaseEntity
  {
    [Required (ErrorMessage = "You must enter a username!")]
    [MinLength (2, ErrorMessage = "Username must be at least {1} characters!")]
    [Display (Name = "First Name")]
    public string UserName { get; set; }

    [Required (ErrorMessage = "You must enter a first name!")]
    [MinLength (2, ErrorMessage = "First Name must be at least {1} characters!")]
    [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
    [Display (Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [MinLength (2, ErrorMessage = "Last Name must be at least {1} characters!")]
    [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
    [Display (Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType (DataType.Password)]
    public string Password { get; set; }

    [Compare ("Password", ErrorMessage = "Password and confirmation must match!")]
    [DataType (DataType.Password)]
    [Display (Name = "Confirm Password")]
    public string PasswordConfirmation { get; set; }
  }

  public class LoginViewModel : BaseEntity
  {
    [Required]
    public string UserName { get; set; }

    [Required]
    [DataType (DataType.Password)]
    public string Password { get; set; }
  }
}