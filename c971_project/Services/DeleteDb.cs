using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace c971_project.Services
{
    public static class DeleteDb
    {
        public static async Task DeleteDatabaseAsync(string dbLocation)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbLocation);

            await Task.Run(() =>
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Debug.WriteLine("Database deleted successfully.");
                }
                else
                {
                    Debug.WriteLine("No database file found to delete.");
                }
            });
        }

        //// Optional: Sync version if you need it
        //public static void DeleteDatabase()
        //{
        //    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
        //    if (File.Exists(dbPath))
        //    {
        //        File.Delete(dbPath);
        //        Debug.WriteLine("Database deleted successfully.");
        //    }
        //    else
        //    {
        //        Debug.WriteLine("No database file found to delete.");
        //    }
        //}
    }
}