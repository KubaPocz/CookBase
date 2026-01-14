using System.Collections.ObjectModel;
using System.Globalization;
using CookBase.Data;

namespace CookBase;

public partial class AddRecipePage : ContentPage
{
    private List<Product> _allProducts = new();
    private List<Category> _allCategories = new();

    public ObservableCollection<Product> FilteredProducts { get; } = new();
    public ObservableCollection<IngredientRow> Ingredients { get; } = new();

    private Product? _selectedProduct;

    // lokalna œcie¿ka do zdjêcia (AppDataDirectory/recipeimages/...)
    private string? _recipePhotoPath;

    public AddRecipePage()
    {
        InitializeComponent();
        BindingContext = this;

        SetupDifficultyPicker();
        LoadData();
    }

    private void SetupDifficultyPicker()
    {
        DifficultyPicker.ItemsSource = new List<string> { "Easy", "Medium", "Hard" };
    }

    private async void LoadData()
    {
        try
        {
            DatabaseInitializer.Initialize();
            using var db = new AppDbContext();

            _allCategories = db.Categories.OrderBy(c => c.Name).ToList();
            _allProducts = db.Products.OrderBy(p => p.Name).ToList();

            // Picker kategorii: All + reszta
            var categoriesForPicker = new List<Category>();
            categoriesForPicker.Add(new Category("All", "")); // pseudo
            categoriesForPicker.AddRange(_allCategories);

            CategoryPicker.ItemsSource = categoriesForPicker;
            CategoryPicker.ItemDisplayBinding = new Binding(nameof(Category.Name));
            CategoryPicker.SelectedIndex = 0;

            ApplyProductFilters();
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ danych: {ex.Message}", "OK");
        }
    }

    // --------------------
    // FOTO (MediaPicker) - BEZ SKIASHARP
    // --------------------
    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Wybierz zdjêcie przepisu"
            });

            if (photo == null) return;

            var newPath = await SaveFileToAppDataAsync(photo);
            _recipePhotoPath = newPath;

            RecipeImagePreview.Source = ImageSource.FromFile(newPath);
            RecipeImagePreview.IsVisible = true;

            PhotoInfoLabel.Text = $"Wybrane: {Path.GetFileName(newPath)}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê wybraæ zdjêcia: {ex.Message}", "OK");
        }
    }

    private void OnClearPhotoClicked(object sender, EventArgs e)
    {
        _recipePhotoPath = null;

        RecipeImagePreview.Source = null;
        RecipeImagePreview.IsVisible = false;

        PhotoInfoLabel.Text = string.Empty;
    }

    private static async Task<string> SaveFileToAppDataAsync(FileResult file)
    {
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var recipesDir = Path.Combine(FileSystem.AppDataDirectory, "recipeimages");
        Directory.CreateDirectory(recipesDir);

        var fileName = $"recipe_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{ext}";
        var destPath = Path.Combine(recipesDir, fileName);

        await using var sourceStream = await file.OpenReadAsync();
        await using var destStream = File.OpenWrite(destPath);
        await sourceStream.CopyToAsync(destStream);

        return destPath;
    }

    // --------------------
    // FILTROWANIE PRODUKTÓW
    // --------------------
    private void OnCategoryChanged(object sender, EventArgs e) => ApplyProductFilters();
    private void OnProductSearchChanged(object sender, TextChangedEventArgs e) => ApplyProductFilters();

    private void ApplyProductFilters()
    {
        var search = ProductSearchBar.Text?.Trim() ?? string.Empty;
        var selectedCategory = CategoryPicker.SelectedItem as Category;

        IEnumerable<Product> query = _allProducts;

        // jeœli u Ciebie Product ma CategoryId (typowe)
        if (selectedCategory != null && selectedCategory.Name != "All")
        {
            query = query.Where(p => p.CategoryId == selectedCategory.Id);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name != null &&
                                     p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var result = query.Take(60).ToList();

        FilteredProducts.Clear();
        foreach (var p in result)
            FilteredProducts.Add(p);
    }

    private async void OnSelectProductClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            _selectedProduct = product;
            SelectedProductLabel.Text = product.Name;

            await Task.Delay(50);
            QuantityEntry.Focus();
        }
    }

    private void OnClearSelectedProductClicked(object sender, EventArgs e)
    {
        _selectedProduct = null;
        SelectedProductLabel.Text = "(brak)";
    }

    // --------------------
    // SK£ADNIKI
    // --------------------
    private async void OnAddIngredientClicked(object sender, EventArgs e)
    {
        if (_selectedProduct == null)
        {
            await DisplayAlert("B³¹d", "Najpierw wybierz produkt z listy.", "OK");
            return;
        }

        var qtyText = (QuantityEntry.Text ?? "").Trim().Replace(',', '.');
        if (!float.TryParse(qtyText, NumberStyles.Float, CultureInfo.InvariantCulture, out var qty) || qty <= 0)
        {
            await DisplayAlert("B³¹d", "Podaj poprawn¹ iloœæ (np. 200 lub 1.5).", "OK");
            return;
        }

        var unit = (UnitEntry.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(unit))
        {
            await DisplayAlert("B³¹d", "Podaj jednostkê (np. g, ml, szt.).", "OK");
            return;
        }

        var note = (NoteEntry.Text ?? "").Trim();

        Ingredients.Add(new IngredientRow
        {
            ProductId = _selectedProduct.Id,
            ProductName = _selectedProduct.Name,
            Quantity = qty,
            Unit = unit,
            Note = note
        });

        QuantityEntry.Text = "";
        UnitEntry.Text = "";
        NoteEntry.Text = "";
    }

    private void OnRemoveIngredientClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is IngredientRow row)
            Ingredients.Remove(row);
    }

    // --------------------
    // ZAPIS PRZEPISU
    // --------------------
    private async void OnSaveRecipeClicked(object sender, EventArgs e)
    {
        var title = (TitleEntry.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("B³¹d", "Podaj tytu³ przepisu.", "OK");
            return;
        }

        var desc = (DescriptionEditor.Text ?? "").Trim();

        if (!int.TryParse((TimeEntry.Text ?? "").Trim(), out var timeMinutes) || timeMinutes < 0)
        {
            await DisplayAlert("B³¹d", "Czas musi byæ liczb¹ ca³kowit¹ (minuty).", "OK");
            return;
        }

        var difficulty = DifficultyPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(difficulty))
        {
            await DisplayAlert("B³¹d", "Wybierz trudnoœæ.", "OK");
            return;
        }

        if (Ingredients.Count == 0)
        {
            var ok = await DisplayAlert("Uwaga", "Nie doda³eœ ¿adnych sk³adników. Zapisaæ mimo to?", "Tak", "Nie");
            if (!ok) return;
        }

        try
        {
            using var db = new AppDbContext();

            var recipe = new Recipe
            {
                Title = title,
                Description = desc,
                TimeMinutes = timeMinutes,
                Difficulty = difficulty,

                // u Ciebie to pole nazywa siê ImageUrl, ale tu jest œcie¿ka lokalna
                ImageUrl = _recipePhotoPath ?? string.Empty
            };

            db.Recipes.Add(recipe);
            db.SaveChanges(); // ¿eby mieæ recipe.Id

            foreach (var ing in Ingredients)
            {
                db.RecipeProducts.Add(new RecipeProduct
                {
                    RecipeId = recipe.Id,
                    ProductId = ing.ProductId,
                    Quantity = ing.Quantity,
                    Unit = ing.Unit,
                    Note = ing.Note
                });
            }

            db.SaveChanges();

            await DisplayAlert("Sukces", "Przepis zosta³ dodany.", "OK");

            // reset formularza
            TitleEntry.Text = "";
            DescriptionEditor.Text = "";
            TimeEntry.Text = "";
            DifficultyPicker.SelectedItem = null;

            OnClearPhotoClicked(this, EventArgs.Empty);

            Ingredients.Clear();
            OnClearSelectedProductClicked(this, EventArgs.Empty);

            ProductSearchBar.Text = "";
            CategoryPicker.SelectedIndex = 0;
            ApplyProductFilters();
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê zapisaæ przepisu: {ex.Message}", "OK");
        }
    }

    public class IngredientRow
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public float Quantity { get; set; }
        public string Unit { get; set; } = "";
        public string Note { get; set; } = "";

        public bool HasNote => !string.IsNullOrWhiteSpace(Note);
        public string DisplayLine => $"{ProductName} — {Quantity.ToString(CultureInfo.InvariantCulture)} {Unit}";
    }
}
