using CookBase.Data;
using Microsoft.EntityFrameworkCore;

namespace CookBase;

[QueryProperty(nameof(ProductId), "productId")]
public partial class EditProductPage : ContentPage
{
    private List<Category> _categories = new();
    private int _productId;

    public string ProductId
    {
        get => _productId.ToString();
        set
        {
            if (int.TryParse(value, out var id))
                _productId = id;
        }
    }

    public EditProductPage()
    {
        InitializeComponent();
        DatabaseInitializer.Initialize();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            using var db = new AppDbContext();

            // Kategorie do pickera
            _categories = db.Categories
                .OrderBy(c => c.Name)
                .ToList();

            CategoryPicker.ItemsSource = _categories;
            CategoryPicker.ItemDisplayBinding = new Binding(nameof(Category.Name));

            // Produkt + ile razy u¿yty w przepisach (RecipeProducts) :contentReference[oaicite:5]{index=5}
            var product = db.Products
                .FirstOrDefault(p => p.Id == _productId);

            if (product == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono produktu.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            NameEntry.Text = product.Name;

            var selected = _categories.FirstOrDefault(c => c.Id == product.CategoryId);
            CategoryPicker.SelectedItem = selected;

            var usedCount = db.RecipeProducts.Count(rp => rp.ProductId == _productId);

            if (usedCount > 0)
            {
                UsageFrame.IsVisible = true;
                UsageLabel.Text = $"Ten produkt jest u¿ywany w przepisach ({usedCount}×). " +
                                  $"Edycja jest OK, ale usuwanie mog³oby rozwaliæ przepisy.";
            }
            else
            {
                UsageFrame.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ danych: {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var selectedCategory = CategoryPicker.SelectedItem as Category;

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("B³¹d", "Podaj nazwê produktu.", "OK");
            return;
        }

        if (selectedCategory == null)
        {
            await DisplayAlert("B³¹d", "Wybierz kategoriê.", "OK");
            return;
        }

        try
        {
            using var db = new AppDbContext();

            var product = db.Products.FirstOrDefault(p => p.Id == _productId);
            if (product == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono produktu do edycji.", "OK");
                return;
            }

            product.Name = name;
            product.CategoryId = selectedCategory.Id; // FK do Category :contentReference[oaicite:6]{index=6}

            db.SaveChanges();

            await DisplayAlert("Sukces", "Zapisano zmiany.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê zapisaæ zmian: {ex.Message}", "OK");
        }
    }
}
