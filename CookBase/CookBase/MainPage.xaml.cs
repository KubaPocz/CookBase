namespace CookBase;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void OnPrzepisyClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RecipesPage());
    }

    private async void OnProduktyClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductsPage());
    }

    private async void OnKategorieClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CategoriesPage());
    }
}