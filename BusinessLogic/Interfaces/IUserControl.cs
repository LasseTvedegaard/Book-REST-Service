using Model;

public interface IUserControl
{
    Task<bool> Create(User entity);
    Task<User> Get(string id);
    Task<bool> Update(User entity);
    Task<User?> GetByEmail(string email);
    Task<(User user, string token)> LoginAsync(string email);

    string GenerateJwtForUser(User user);
}
