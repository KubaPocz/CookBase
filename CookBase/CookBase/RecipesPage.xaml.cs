using CookBase.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;

namespace CookBase;

public partial class RecipesPage : ContentPage
{
    public ObservableCollection<RecipeCardVm> Recipes { get; } = new();

    public RecipesPage()
    {
        InitializeComponent();
        DatabaseInitializer.Initialize();
        RecipesCollection.ItemsSource = Recipes;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecipes();
    }

    private async void LoadRecipes()
    {
        try
        {
            using var db = new AppDbContext();

            // Recipe model: Title, Difficulty, TimeMinutes, ImageUrl
            var list = db.Recipes
                .OrderByDescending(r => r.Id)
                .ToList();

            Recipes.Clear();
            foreach (var r in list)
            {
                Recipes.Add(new RecipeCardVm
                {
                    Id = r.Id,
                    Title = r.Title,
                    Difficulty = r.Difficulty,
                    TimeMinutes = r.TimeMinutes,
                    ImageUrl = r.ImageUrl
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ przepisów: {ex.Message}", "OK");
        }
    }

    private async void OnRecipeTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is RecipeCardVm vm)
        {
            await Shell.Current.GoToAsync($"{nameof(RecipeDetailsPage)}?recipeId={vm.Id}");
        }
    }

    private async void OnAddRecipeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddRecipePage));
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }

    public class RecipeCardVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Difficulty { get; set; } = "";
        public int TimeMinutes { get; set; }
        public string ImageUrl { get; set; } = ""; // u Ciebie to jest œcie¿ka lokalna

        public string MetaLine => $"{Difficulty} • {TimeMinutes} min";
    }
}

/// <summary>
/// Jeœli brak œcie¿ki albo plik nie istnieje -> noImageRecipe.png z Resources/Images
/// </summary>
public class RecipeImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var path = value as string;

        if (string.IsNullOrWhiteSpace(path))
            return "no_image_recipe.png";

        // jeœli to lokalna œcie¿ka i plik nie istnieje -> fallback
        try
        {
            if (Path.IsPathRooted(path) && !File.Exists(path))
                return "no_image_recipe.png";
        }
        catch { return "no_image_recipe.png"; }

        return path; // MAUI za³aduje lokalny plik po œcie¿ce
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
