using CookBase.Caches;

namespace CookBase;
public partial class CategoriesPage : ContentPage
{
    public CategoriesPage()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var categories = CategoryCache.Categories;

            BindableLayout.SetItemsSource(CategoriesFlexLayout, categories);
        }
        catch (Exception ex)
        {
            DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ danych: {ex.Message}", "OK");
        }
    }

    private void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var category = frame?.BindingContext as Category;
        if (category != null)
        {
            DisplayAlert("Kategoria", $"Wybrano kategoriê: {category.Name}", "OK");
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }
}