namespace CookBase;

public partial class RecipesPage : ContentPage
{
	public RecipesPage()
	{
		InitializeComponent();
	}
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }
}