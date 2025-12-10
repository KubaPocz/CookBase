using CookBase.Caches;
using CookBase.Data;

namespace CookBase
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DatabaseInitializer.Initialize();

            CategoryCache.Initialize();

            MainPage = new AppShell();
        }

    }

}