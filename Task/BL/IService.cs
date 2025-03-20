namespace Task.BL
{
    public interface IService<ViewModel>
    {
        public void Create(ViewModel Data);
        public void Update(ViewModel Data, int Id);
        public void Delete(int Id);
    }
}
