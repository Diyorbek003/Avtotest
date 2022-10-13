using System.ComponentModel.DataAnnotations;



namespace Avtotest.Web.Models;

public class User
{
    public int Index { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 5)]
    public string? Name { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [Password(8)]
    public string? Password { get; set; }

    public string? Image { get; set; }

    public IFormFile? ImageFile { get; set; }
}