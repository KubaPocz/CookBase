using CookBase.Data;
using Microsoft.EntityFrameworkCore;

namespace CookBase.Caches;

public static class CategoryCache
{
    private static List<Category>? _categories;

    public static IReadOnlyList<Category> Categories =>
    _categories ?? new List<Category>();


    public static void Initialize()
    {
        if (_categories is not null)
            return;

        using var db = new AppDbContext();

        _categories = db.Categories
                        .AsNoTracking()
                        .ToList();
    }

    public static void Invalidate()
    {
        _categories = null;
    }
}
