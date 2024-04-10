using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;


namespace BusinessLogic {
    public class GenreControl : ICRUD<Genre> {
        private readonly ICRUDAccess<Genre> _genreAccess;

        public GenreControl(ICRUDAccess<Genre> genreAccess) { 
            _genreAccess = genreAccess;
        }
        public async Task<int> Create(Genre entity) {
            int insertedId = -1;
            insertedId = await _genreAccess.Create(entity);
            return insertedId;
        }

        public async Task<bool> Delete(int id) {
            bool isDeleted;
            isDeleted = await _genreAccess.Delete(id);
            return isDeleted;
        }

        public async Task<List<Genre>> GetAll() {
            List<Genre> foundGenre;
            foundGenre = await _genreAccess.GetAll();
            return foundGenre;
        }

        public Task<Genre> GetValue(int id) {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(int id, Genre entity) {
            bool isUpdated;
            isUpdated = await _genreAccess.Update(id, entity);
            return isUpdated;
        }
    }
}

