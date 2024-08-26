namespace ShoeMart.Models.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        TEntity GetById(string id);
        IEnumerable<TEntity> GetAll();
        void Update(TEntity entity);
        bool Delete(string id);

        IEnumerable<TEntity> GetFeaturedItems();

    }
}
