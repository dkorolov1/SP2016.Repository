using SP2016.Repository.Entities;

namespace SP2016.Repository
{
    public class EntityContainer<T> where T : IEntity, new()
    {
        public T Entity;
        public string FolderPath;
    }
}
