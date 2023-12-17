using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace FoxORM
{
    public class FoxOrm : IDisposable
    {
        private SQLiteAsyncConnection sqliteConnection { get; }
        private string sqliteFilePath { get; }

        /// <summary>
        /// Open the SQLITE db you need for your plugin.
        /// You NEED to register at least one table to create the file
        /// </summary>
        public FoxOrm(string sqliteFilePath)
        {
            this.sqliteFilePath = sqliteFilePath;
            if (!Directory.Exists(Path.GetDirectoryName(this.sqliteFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.sqliteFilePath) ??
                                          throw new InvalidOperationException());
            }

            this.sqliteConnection = new SQLiteAsyncConnection(this.sqliteFilePath);
        }

        /// <summary>
        /// Init if needed the class in the database
        /// The model need to be a sqlite type class with Annotation
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        public async void RegisterTable<T>() where T : new()
        {
            await this.sqliteConnection.CreateTableAsync<T>(CreateFlags.None);
        }


        /// <summary>
        /// Saves an object to the SQLite database.
        /// </summary>
        /// <typeparam name="T">The type of object being saved.</typeparam>
        /// <param name="objectToSave">The object to be saved.</param>
        /// <returns>True if the object is saved successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when objectToSave is null.</exception>
        /// <exception cref="Exception">Thrown when the type T does not have an 'id' property, or an unknown exception occurs during the save operation.</exception>
        public async Task<bool> Save<T>(T objectToSave) where T : new()
        {
            if (objectToSave == null) throw new ArgumentNullException(nameof(objectToSave));

            try
            {
                var idProperty = typeof(T).GetProperty("id");
                if (idProperty == null)
                    throw new Exception(
                        $"The type {typeof(T).Name} does not have a 'id' property, which is expected for entities.");

                var idValue = (int)idProperty.GetValue(objectToSave);
                var existingRecord = await this.sqliteConnection.FindAsync<T>(idValue);

                if (existingRecord != null)
                {
                    await this.sqliteConnection.UpdateAsync(objectToSave);
                }
                else
                {
                    await this.sqliteConnection.InsertAsync(objectToSave);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves an item with the specified id from the SQLite database.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="id">The id of the item to retrieve.</param>
        /// <returns>
        /// The retrieved item from the database with the specified id, if found; otherwise, the default value of type T.
        /// </returns>
        /// <remarks>
        /// The method returns a Task object representing the asynchronous operation.
        /// </remarks>
        public async Task<T> Query<T>(int id) where T : new()
        {
            try
            {
                var item = await this.sqliteConnection.FindAsync<T>(id);
                return item;
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Queries all records of the specified type from the SQLite database.
        /// </summary>
        /// <typeparam name="T">The type of records to query.</typeparam>
        /// <returns>A list of records of the specified type, or null if an error occurred.</returns>
        public async Task<List<T>> QueryAll<T>() where T : new()
        {
            try
            {
                var items = await this.sqliteConnection.Table<T>().ToListAsync();
                return items;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Deletes an object from the SQLite database.
        /// </summary>
        /// <typeparam name="T">The type of object being deleted.</typeparam>
        /// <param name="objectToDelete">The object to be deleted.</param>
        /// <returns>True if the object is deleted successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when objectToDelete is null.</exception>
        /// <exception cref="Exception">Thrown when the type T does not have an 'id' property, or an unknown exception occurs during the delete operation.</exception>
        public async Task<bool> Delete<T>(T objectToDelete) where T : new()
        {
            if (objectToDelete == null) throw new ArgumentNullException(nameof(objectToDelete));
            try
            {
                var idProperty = typeof(T).GetProperty("id");
                if (idProperty == null) throw new Exception($"The type {typeof(T).Name} does not have a 'id' property, which is expected for entities.");
                var idValue = (int)idProperty.GetValue(objectToDelete);
                var existingRecord = await this.sqliteConnection.FindAsync<T>(idValue);
                if (existingRecord != null)
                {
                    await this.sqliteConnection.DeleteAsync(objectToDelete);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            sqliteConnection.CloseAsync();
        }
    }
}