using System.Data;
using System.Data.SqlClient;

namespace AdoFilesApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // сохранение файлов в бд
            /*
            await SaveFile("D:\\RPO\\Maxim Efimov\\images\\image01.jpg", "Бабочка");
            await SaveFile("D:\\RPO\\Maxim Efimov\\images\\image02.png", "Книга");
            await SaveFile("D:\\RPO\\Maxim Efimov\\images\\image03.png", "Цветы");
            await SaveFile("D:\\RPO\\Maxim Efimov\\images\\image04.jpg", "Тигр");
            */

            //
            await ReadFile("D:\\RPO\\Maxim Efimov\\images\\");
        }

        static async Task SaveFile(string path, string description)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LibraryDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"INSERT INTO files (name, description, data)
                                            VALUES(@name, @description, @data)";
                command.Parameters.Add("@name", SqlDbType.NVarChar, 50);
                command.Parameters.Add("@description", SqlDbType.NVarChar, 50);

                string name = path.Substring(path.LastIndexOf('\\') + 1);
                byte[] data;
                
                using(FileStream file = new(path, FileMode.Open))
                {
                    data = new byte[file.Length];
                    await file.ReadAsync(data, 0, (int)file.Length);
                    command.Parameters.Add("@data", SqlDbType.Binary, (int)file.Length);
                }

                command.Parameters["@name"].Value = name;
                command.Parameters["@description"].Value = description;
                command.Parameters["@data"].Value = data;

                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"file {name} save to db");
            }
        }

        static async Task ReadFile(string path)
        {
            List<FileImage> files = new();

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LibraryDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM files";
                
                using(SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        FileImage file = new(reader.GetInt32(0),
                                             reader.GetString(1),
                                             reader.GetString(2),
                                             (byte[])reader.GetValue(3));
                        files.Add(file);
                    }
                }

                foreach(FileImage file in files)
                {
                    using(FileStream fc = new(path + file.Name, FileMode.OpenOrCreate))
                    {
                        await fc.WriteAsync(file.Data, 0, file.Data.Length);
                        Console.WriteLine($"file {file.Name} with {file.Description} create to path {path}");
                    }
                }

            }
        }
    }
}