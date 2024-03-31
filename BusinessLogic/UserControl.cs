using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic {
    public class UserControl : IUserControl {

        private readonly IUserAccess _userAccess;
        public UserControl(IUserAccess userAccess) { 
            _userAccess = userAccess;
        }
        public async Task<bool> Create(User entity) {
            return await _userAccess.Create(entity);
        }

        public async Task<User> Get(string id) {
            return await _userAccess.Get(id);
        }

        public async Task<bool> Update(User entity) {
            return await _userAccess.Update(entity);
        }
    }
}
