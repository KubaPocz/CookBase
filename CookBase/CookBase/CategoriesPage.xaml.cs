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

            BindableLayout.SetItemsSource(CategoriesList, categories);
        }
        catch (Exception ex)
        {
            DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ danych: {ex.Message}", "OK");
        }
    }

    private async void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var category = frame?.BindingContext as Category;
        if (category != null)
        {
            await Shell.Current.GoToAsync($"{nameof(ProductsPage)}?categoryId={category.Id}");
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }
}