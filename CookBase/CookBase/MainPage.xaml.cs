using CookBase.Data;
namespace CookBase;
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            // Inicjalizuj bazę
            DatabaseInitializer.Initialize();

            // Załaduj kategorie do UI
            using var db = new AppDbContext();
            var categories = db.Categories.ToList();

            CategoriesFlexLayout.BindingContext = this;
            BindableLayout.SetItemsSource(CategoriesFlexLayout, categories);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
        }
    }
    private void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var category = frame?.BindingContext as Category;
        if (category != null)
        {
            // Obsłuż kliknięcie kategorii (np. nawigacja do listy produktów)
            DisplayAlert("Kategoria", $"Wybrano kategorię: {category.Name}", "OK");
        }
    }
}