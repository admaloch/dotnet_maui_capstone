using c971_project.Services;

namespace c971_project
{
    public partial class App : Application
    {
        // Expose the database service globally
        public static DatabaseService Database { get; private set; } = null!;

        public App()
        {
            InitializeComponent();

            // Initialize the database once at startup
            Database = new DatabaseService();

            MainPage = new AppShell();

        }
    }
}