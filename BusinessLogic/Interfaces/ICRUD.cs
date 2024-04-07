namespace BusinessLogic.Interfaces {
    public interface ICRUD<T> {

        Task<int> Create(T entity);
        //Task<T> GetValue(int id);
        Task<List<T>> GetAll();
        //Task<bool> Update(int id, T entity);
        //Task<bool> Delete(int id);
    }
}
