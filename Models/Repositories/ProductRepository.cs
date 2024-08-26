using ShoeMart.Models.Interfaces;
using ShoeMart.Models.Entities;
using Microsoft.Data.SqlClient;
namespace ShoeMart.Models.Repositories
{
    public class ProductRepository : GenericRepository<Products>, IProductRepository
    {
        // implementations
        public (bool result, string imagePath) ValidateFileAndSaveToProductsFolder(IFormFile file)
        {
            if (file == null) return (true, "");

            // List of allowed image file extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };

            // List of allowed MIME types
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff" };

            // Get the file extension
            var extension = Path.GetExtension(file.FileName).ToLower();

            // Check if the file extension is allowed
            if (!allowedExtensions.Contains(extension))
            {
                return (false, "");
            }

            // Check the MIME type
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                return (false, "");
            }

            // If validation passes, proceed with saving the file
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return (true, uniqueFileName);
        }

        public void LoadProducts()
        {
            string[] names = { "Air Max", "Classic Sneaker", "Sporty Runner", "Urban Boots", "Casual Loafers", "High-Top Sneakers", "Elegant Flats", "Running Shoes", "Stylish Heels", "Comfort Sandals" };
            string[] sizes = { "xs", "sm", "m", "lg", "xl" };
            string[] categories = { "men", "women", "kids" };
            string[] colors = { "white", "black", "red", "brown", "blue", "pink" };
            string[] images = { "2.jpg", "3.jpg", "4.jpg", "5.jpg", "6.jpg", "7.jpg", "8.jpg", "9.jpg", "10.jpg", "11.jpg" };

            Random random = new Random();

            // Base directory for images
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

            // Ensure the upload directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ShoeMart;Integrated Security=True;"))
            {
                connection.Open();

                for (int i = 0; i < 10; i++)
                {
                    string id = Guid.NewGuid().ToString();
                    string name = names[random.Next(names.Length)];
                    decimal price = random.Next(50, 200); // Price between 50 and 200 dollars
                    string size = sizes[random.Next(sizes.Length)];
                    string category = categories[random.Next(categories.Length)];
                    int stock = random.Next(1, 100); // Any positive number
                    string description = "This is a high-quality " + name + " designed for comfort and style. Perfect for " + category + " who value quality and performance.";
                    string color = colors[random.Next(colors.Length)];

                    // Get the image file name
                    string imageFileName = images[i];
                    string imagePath = Path.Combine(uploadsFolder, imageFileName);

                    // Relative path for the image to be stored in the database
                    string relativeImagePath = $"/images/products/{imageFileName}";

                    string query = $@"
                        INSERT INTO Products (ID, Name, Price, Size, Category, Stock, Description, Image, Color) 
                        VALUES ('{id}', '{name}', {price}, '{size}', '{category}', {stock}, '{description}', '{relativeImagePath}', '{color}')";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}