namespace Task.BL
{
    public interface IService<ViewModel, BaseModel>
    {
        public System.Threading.Tasks.Task CreateAsync(ViewModel Data);
        public System.Threading.Tasks.Task Update(ViewModel Data, int Id);
        public System.Threading.Tasks.Task DeleteAsync(int Id);
        public IEnumerable<BaseModel> GetBySearch(string SearchTerm);
        public ViewModel PrepareCreateForm();
        public ViewModel PrepareEditForm(int Id);
    }
}
