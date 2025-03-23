using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces {
    public interface IUserControl {
        Task<bool> Create(User entity);
        Task<User> Get(string id);
        Task<bool> Update(User entity);
        Task<User?> GetByEmail(string email);
        Task<(User user, string token)> LoginAsync(string email);

    }
}
