public class RecipeProduct
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public float Quantity { get; set; }
    public string Unit { get; set; }
    public string Note { get; set; }
}
