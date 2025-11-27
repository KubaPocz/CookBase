using System.ComponentModel.DataAnnotations;
public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    // Relacja 1..* → Produkty
    public List<Product> Products { get; set; }

    public string IconPath { get; set; }
    public Category() { }

    public Category(string name, string iconPath)
    {
        Name = name;
        IconPath = iconPath;
    }
}
