namespace Task.Repositories
{
    public interface IRepository<Type>
    {
        public void Create(Type obj);
        public Type GetById(int id);
        public IEnumerable<Type> GetAll();
        public void Update(Type obj);
        public void Delete(Type obj);
        public void UploadToDatabase();
        public System.Threading.Tasks.Task UploadToDatabaseAsync();
    }
}
