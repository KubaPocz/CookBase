namespace CookBase;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void OnPrzepisyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecipesPage));
    }

    private async void OnProduktyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProductsPage));
    }

    private async void OnKategorieClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CategoriesPage));
    }
}