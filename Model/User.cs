using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model
{
    public class User
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Ugyldigt email-format")]
        public string Email { get; set; } = string.Empty;

        public User() { }

        public User(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
