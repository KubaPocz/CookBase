using CookBase.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CookBase;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeDetailsPage : ContentPage
{
    private int _recipeId;
    public string RecipeId
    {
        get => _recipeId.ToString();
        set { if (int.TryParse(value, out var id)) _recipeId = id; }
    }

    public ObservableCollection<IngredientVm> Ingredients { get; } = new();

    public RecipeDetailsPage()
    {
        InitializeComponent();
        DatabaseInitializer.Initialize();
        IngredientsList.ItemsSource = Ingredients;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecipe();
    }

    private async void LoadRecipe()
    {
        try
        {
            using var db = new AppDbContext();

            // pobierz przepis + sk³adniki
            var recipe = db.Recipes.FirstOrDefault(r => r.Id == _recipeId);
            if (recipe == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono przepisu.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            TitleLabel.Text = recipe.Title;
            MetaLabel.Text = $"{recipe.Difficulty} • {recipe.TimeMinutes} min";
            DescriptionLabel.Text = string.IsNullOrWhiteSpace(recipe.Description) ? "(brak opisu)" : recipe.Description;

            // obraz
            var imgSource = new RecipeImageSourceConverter().Convert(recipe.ImageUrl, typeof(string), null, null);
            HeroImage.Source = imgSource as string;

            // sk³adniki: RecipeProducts + Product
            var items = db.RecipeProducts
                .Where(rp => rp.RecipeId == _recipeId)
                .Include(rp => rp.Product)
                .OrderBy(rp => rp.Product.Name)
                .ToList();

            Ingredients.Clear();
            foreach (var rp in items)
            {
                Ingredients.Add(new IngredientVm
                {
                    Line = $"{rp.Product.Name} — {rp.Quantity} {rp.Unit}",
                    Note = rp.Note ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê wczytaæ szczegó³ów: {ex.Message}", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert(
            "Usuñ przepis",
            "Na pewno chcesz usun¹æ ten przepis? Tej operacji nie da siê cofn¹æ.",
            "Usuñ",
            "Anuluj");

        if (!confirm) return;

        try
        {
            using var db = new AppDbContext();

            // pobierz przepis
            var recipe = db.Recipes.FirstOrDefault(r => r.Id == _recipeId);
            if (recipe == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono przepisu.", "OK");
                return;
            }

            // 1) usuñ powi¹zania z produktami (¿eby nie by³o konfliktu FK)
            var links = db.RecipeProducts.Where(rp => rp.RecipeId == _recipeId).ToList();
            if (links.Count > 0)
                db.RecipeProducts.RemoveRange(links);

            // 2) usuñ przepis
            db.Recipes.Remove(recipe);
            db.SaveChanges();

            // 3) opcjonalnie usuñ plik zdjêcia z dysku (tylko jeœli to lokalna œcie¿ka)
            TryDeleteLocalPhoto(recipe.ImageUrl);

            await DisplayAlert("Sukces", "Przepis zosta³ usuniêty.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê usun¹æ przepisu: {ex.Message}", "OK");
        }
    }

    private void TryDeleteLocalPhoto(string? path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            // nie kasuj fallbacków z Resources, kasuj tylko realne œcie¿ki plików
            if (Path.IsPathRooted(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // ignorujemy – usuniêcie zdjêcia nie jest krytyczne
        }
    }


    public class IngredientVm
    {
        public string Line { get; set; } = "";
        public string Note { get; set; } = "";
        public bool HasNote => !string.IsNullOrWhiteSpace(Note);
    }
}
