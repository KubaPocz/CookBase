namespace CookBase;

using CookBase.Data;
using Microsoft.EntityFrameworkCore;
[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class ProductsPage : ContentPage
{
    private List<Category> _categories = new();
    private List<Product> _allProducts = new();
    private int? _preselectedCategoryId;

    public string CategoryId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _preselectedCategoryId = id;
        }
    }
    public ProductsPage()
    {
        InitializeComponent();
        DatabaseInitializer.Initialize();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            using var db = new AppDbContext();

            _categories = db.Categories
                .OrderBy(c => c.Name)
                .ToList();

            // do widoku potrzebujesz Category.Name, wiêc Include
            _allProducts = db.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToList();

            // Picker: All + kategorie
            var pickerList = new List<Category>();
            pickerList.Add(new Category { Id = 0, Name = "Wszystkie", IconPath = "" }); // pseudo
            pickerList.AddRange(_categories);

            CategoryFilterPicker.ItemsSource = pickerList;
            CategoryFilterPicker.ItemDisplayBinding = new Binding(nameof(Category.Name));

            if (CategoryFilterPicker.SelectedItem == null)
                CategoryFilterPicker.SelectedIndex = 0;

            if (_preselectedCategoryId.HasValue)
            {
                var match = pickerList.FirstOrDefault(c => c.Id == _preselectedCategoryId.Value);
                if (match != null)
                    CategoryFilterPicker.SelectedItem = match;

                _preselectedCategoryId = null; // ¿eby przy kolejnych wejœciach nie trzyma³o starego
            }

            ApplyFilters();
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ produktów: {ex.Message}", "OK");
        }
    }

    private void OnFiltersChanged(object sender, EventArgs e) => ApplyFilters();

    private void ApplyFilters()
    {
        var selectedCategory = CategoryFilterPicker.SelectedItem as Category;
        var search = SearchBar.Text?.Trim() ?? "";

        IEnumerable<Product> q = _allProducts;

        if (selectedCategory != null && selectedCategory.Id != 0)
        {
            q = q.Where(p => p.CategoryId == selectedCategory.Id);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(p => p.Name != null &&
                             p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        ProductsList.ItemsSource = q.ToList();
    }

    private async Task GoToEdit(Product product)
    {
        // Shell navigation z parametrem
        await Shell.Current.GoToAsync($"{nameof(EditProductPage)}?productId={product.Id}");
    }

    private async void OnEditProductClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
            await GoToEdit(product);
    }

    private async void OnEditProductInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe && swipe.CommandParameter is Product product)
            await GoToEdit(product);
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddProductPage));
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }
}
