using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    // FK → Category
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    // Relacja → RecipeProduct (wiele-do-wielu)
    public List<RecipeProduct> RecipeProducts { get; set; }
}
