using CookBase.Data;
namespace CookBase;
public partial class CategoriesPage : ContentPage
{
    public CategoriesPage()
    {
        InitializeComponent();
        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            // Inicjalizuj bazê
            DatabaseInitializer.Initialize();

            // Za³aduj kategorie do UI
            using var db = new AppDbContext();
            var categories = db.Categories.ToList();

            CategoriesFlexLayout.BindingContext = this;
            BindableLayout.SetItemsSource(CategoriesFlexLayout, categories);
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ danych: {ex.Message}", "OK");
        }
    }
    private void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var category = frame?.BindingContext as Category;
        if (category != null)
        {
            // Obs³u¿ klikniêcie kategorii (np. nawigacja do listy produktów)
            DisplayAlert("Kategoria", $"Wybrano kategoriê: {category.Name}", "OK");
        }
    }
}