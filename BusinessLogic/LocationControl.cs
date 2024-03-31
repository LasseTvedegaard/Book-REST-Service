using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;

namespace BusinessLogic {
    public class LocationControl : ICRUD<Location> {
        private readonly ICRUDAccess<Location> _locationAccess;

        public LocationControl(ICRUDAccess<Location> locationAccess) {
            _locationAccess = locationAccess;
        }
        public async Task<int> Create(Location entity) {
            int insertedId = -1;
            insertedId = await _locationAccess.Create(entity);
            return insertedId;
        }

        public async Task<bool> Delete(int id) {
            bool isDeleted;
            isDeleted = await _locationAccess.Delete(id);
            return isDeleted;
        }

        public async Task<List<Location>> GetAll() {
            List<Location> foundLocation;
            foundLocation = await _locationAccess.GetAll();
            return foundLocation;
        }

        public Task<Location> GetValue(int id) {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(int id, Location entity) {
            bool isUpdated;
            isUpdated = await _locationAccess.Update(id, entity);
            return isUpdated;
        }
    }
}
