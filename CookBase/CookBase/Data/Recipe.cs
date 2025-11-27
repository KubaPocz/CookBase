using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Recipe
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public int TimeMinutes { get; set; }
    public string Difficulty { get; set; }
    public string ImageUrl { get; set; }

    // Relacja → RecipeProduct
    public List<RecipeProduct> RecipeProducts { get; set; }
}
