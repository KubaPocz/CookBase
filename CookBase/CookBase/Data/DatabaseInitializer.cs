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
                new Category("Owoce", "fruits.png"),
                new Category("Warzywa", "vegetables.png"),
                new Category("Nabiał", "dairy.png"),
                new Category("Ryby i owoce morza", "fish_and_seafood.png"),
                new Category("Produkty mączne", "flour_products.png"),
                new Category("Strączki", "legumes.png"),
                new Category("Mięso", "meat.png"),
                new Category("Orzechy", "nuts.png"),
                new Category("Oleje", "oil.png"),
                new Category("Przyprawy", "spices.png"),
                new Category("Inne", "other.png")
            };
            db.Categories.AddRange(categories);
            db.SaveChanges();

            Debug.WriteLine($"Dodano {categories.Count} kategorii.");
        }
    }
}