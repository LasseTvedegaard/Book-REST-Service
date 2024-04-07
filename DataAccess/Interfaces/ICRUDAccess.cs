namespace DataAccess.Interfaces {
    public interface ICRUDAccess<T> {

        Task<int> Create(T entity);
        //Task<T> Get(int id);
        Task<List<T>> GetAll();
        //Task<bool> Update(int id, T entity);
        //Task<bool> Delete(int id);

        ////For test tear down
        //Task<bool> DeleteAll();
    }
}
