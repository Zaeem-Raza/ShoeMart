using ShoeMart.Models.Entities;

namespace ShoeMart.Models.Interfaces
{
    public interface IProductRepository : IRepository<Products>
    {
        // functions to be decided
        public (bool result, string imagePath) ValidateFileAndSaveToProductsFolder(IFormFile file);

        public void LoadProducts();

    }
}
