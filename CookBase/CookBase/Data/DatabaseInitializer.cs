using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CookBase.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                using var db = new AppDbContext();

                // Utwórz bazę jeśli nie istnieje
                db.Database.EnsureCreated();

                // Dodaj dane początkowe
                SeedCategories(db);

                Debug.WriteLine("Baza danych zainicjalizowana pomyślnie");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd inicjalizacji bazy: {ex.Message}");
                throw;
            }
        }

        public static void SeedCategories(AppDbContext db)
        {
            // Sprawdź czy już są kategorie (żeby nie duplikować)
            if (db.Categories.Any())
            {
                Debug.WriteLine("Kategorie już istnieją w bazie.");
                return;
            }

            var categories = new List<Category>
            {
                new Category("Dairy", "dairy.png"),
                new Category("Fish and Seafood", "fish_and_seafood.png"),
                new Category("Flour Products", "flour_products.png"),
                new Category("Fruits", "fruits.png"),
                new Category("Legumes", "legumes.png"),
                new Category("Meat", "meat.png"),
                new Category("Nuts", "nuts.png"),
                new Category("Oil", "oil.png"),
                new Category("Spices", "spices.png"),
                new Category("Vegetables", "vegetables.png")
            };

            db.Categories.AddRange(categories);
            db.SaveChanges();

            Debug.WriteLine($"Dodano {categories.Count} kategorii.");
        }
    }
}