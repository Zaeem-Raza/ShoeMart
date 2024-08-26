using ShoeMart.Models.Entities;
using ShoeMart.Models.Interfaces;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
namespace ShoeMart.Models.Repositories
{
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        
        private string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ShoeMart;Integrated Security=True;";
        public Users Login(Users u)
        {
            Users user = new Users {};
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                try
                {
                    connect.Open();

                    // Query to check if the email and password match
                    string query = "select * from Users where Email=@Email and Password=@Password";
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        // prevent SQL injection
                        cmd.Parameters.AddWithValue("@Email", u.Email);
                        cmd.Parameters.AddWithValue("@Password", u.PasswordHash); // hashing --

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while(reader.Read())
                                {
                                    user.Id = Convert.ToString(reader["Id"]);
                                    user.UserName = Convert.ToString(reader["Name"]);
                                    user.Role = Convert.ToString(reader["Role"]);
                                }   
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // not found
                    Console.WriteLine(ex.Message);
                   
                }
                finally
                {

                    connect.Close();
                }
            }
            return user;
        }


        public (bool result, string message) SignUp(Users user)
        {
            bool result = false;
            string message = String.Empty;

            // Checking whether the user has provided all the fields
            if (user.Id == null || user.UserName == null || user.Email == null || user.PasswordHash == null || user.PhoneNumber == null || user.Address == null)
            {
                result = false;
                message = "Input fields cannot be null";
                return (result, message);
            }


            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                try
                {
                    connect.Open();

                    // Email must be unique
                    string select = "select * from Users where Email=@Email";
                    using (SqlCommand selectCommand = new SqlCommand(select, connect))
                    {
                        selectCommand.Parameters.AddWithValue("@Email", user.Email);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // Email already exists
                                result = false;
                                message = "User with this email already exists";
                                connect.Close();
                                return (result, message);
                            }
                        }
                    }

                    // Registering the user
                    string query = "insert into Users (Id, Name, Email, Password, Role, Address, PhoneNo) Values(@Id, @Name, @Email, @Password, @Role, @Address, @PhoneNo)";
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@Id", user.Id);
                        cmd.Parameters.AddWithValue("@Name", user.UserName);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", user.PasswordHash);  // Hashing --
                        cmd.Parameters.AddWithValue("@Role", user.Role);
                        cmd.Parameters.AddWithValue("@Address", user.Address);
                        cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNumber);

                        int count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            result = true;
                            message = "use registered successfully";
                            connect.Close();
                            return (result, message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = false;
                    message = "Something went wrong!!!";
                }
                finally
                {
                    connect.Close();
                }
            }
            return (result, message);
        }

        public (bool result, string imagePath) ValidateFileAndSaveToFolder(IFormFile file)
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
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return (true, uniqueFileName);
        }

        public string GetUserProfileImage(string Id)
        {
            string image = string.Empty;
            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                try
                {
                    connect.Open();

                    string select = "select * from Users where Id=@Id";
                    using (SqlCommand selectCommand = new SqlCommand(select, connect))
                    {
                        selectCommand.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    image = Convert.ToString(reader["Image"]);
                                }
                                return image;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }
            return image;
        }

    }
}
