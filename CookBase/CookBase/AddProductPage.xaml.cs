using CookBase.Data;
using Microsoft.EntityFrameworkCore;

namespace CookBase;

public partial class AddProductPage : ContentPage
{
    private List<Category> _categories = new();

    public AddProductPage()
    {
        InitializeComponent();
        LoadCategories();
    }

    private async void LoadCategories()
    {
        try
        {
            DatabaseInitializer.Initialize();

            using var db = new AppDbContext();
            _categories = db.Categories.ToList();

            CategoryPicker.ItemsSource = _categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ kategorii: {ex.Message}", "OK");
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

            var product = new Product
            {
                Name = name,
                CategoryId = selectedCategory.Id
            };

            db.Products.Add(product);
            db.SaveChanges();

            await DisplayAlert("Sukces", "Produkt zosta³ dodany.", "OK");

            // wyczyszczenie formularza
            NameEntry.Text = string.Empty;
            CategoryPicker.SelectedItem = null;

            // jeœli chcesz wróciæ na listê produktów:
            // await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê zapisaæ produktu: {ex.Message}", "OK");
        }
    }
}
