using System.ComponentModel.DataAnnotations;

public class Animal
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; }

    [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [MaxLength(200, ErrorMessage = "Category cannot exceed 200 characters.")]
    public string? Category { get; set; }

    [Required(ErrorMessage = "Area is required.")]
    [MaxLength(200, ErrorMessage = "Area cannot exceed 200 characters.")]
    public string? Area { get; set; }
}