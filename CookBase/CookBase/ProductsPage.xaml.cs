namespace CookBase;

using Microsoft.EntityFrameworkCore;
using CookBase.Data;

public partial class ProductsPage : ContentPage
{
    public ProductsPage()
    {
        InitializeComponent();
        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            DatabaseInitializer.Initialize();

            using var db = new AppDbContext();

            // £adujemy produkty + kategoriê
            var products = db.Products
                             .Include(p => p.Category)
                             .ToList();

            ProductsList.ItemsSource = products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ produktów: {ex.Message}", "OK");
        }
    }
    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage());
    }
}
