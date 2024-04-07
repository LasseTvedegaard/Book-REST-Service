using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;

namespace BusinessLogic {
    public class EmployeeControl : ICRUD<Employee> {
        private readonly ICRUDAccess<Employee> _employeeAccess;

        public EmployeeControl(ICRUDAccess<Employee> employeeAccess) {
            _employeeAccess = employeeAccess;
        }
        public async Task<int> Create(Employee entity) {
            int insertedId = -1;
            insertedId = await _employeeAccess.Create(entity);
            return insertedId;
        }

        //public async Task<bool> Delete(int id) {
        //    bool isDeleted;
        //    isDeleted = await _employeeAccess.Delete(id);
        //    return isDeleted;
        //}

        public async Task<List<Employee>> GetAll() {
            List<Employee> foundEmployees;
            foundEmployees = await _employeeAccess.GetAll();
            return foundEmployees;
        }


        //public Task<Employee> GetValue(int id) {
        //    throw new NotImplementedException();
        //}

        //public async Task<bool> Update(int EmployeeId, Employee employeeToUpdate) {
        //    bool isUpdated;
        //    isUpdated = await _employeeAccess.Update(EmployeeId, employeeToUpdate);
        //    return isUpdated;
        //}
    }
}
