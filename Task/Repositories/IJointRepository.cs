namespace Task.Repositories
{
    public interface IJointRepository<Type>
    {
        public void Create(Type obj);
        public Type GetById(int id);
        public IEnumerable<Type> GetRangeById(int id);
        public Type GetById(int id, List<string> NavProps);
        public IEnumerable<Type> GetRangeById(int id, List<string> NavProps);
        public IEnumerable<Type> GetAll();
        public IEnumerable<Type> GetAll(List<string> NavProps);
        public void Update(Type obj);
        public void Delete(Type obj);
        public void DeleteRange(IEnumerable<Type> obj);
        public void UploadToDatabase();
        public System.Threading.Tasks.Task UploadToDatabaseAsync();
    }
}
