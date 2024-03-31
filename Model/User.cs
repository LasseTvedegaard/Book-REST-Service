namespace Model {
    public class User {
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set; }

        public User() { }

        public User(string userId, string email) {
            UserId = userId;
            Email = email;
            FirstName = null;
            LastName = null;
            Birthdate = null;
            Phone = null;
            Address = null;
            ZipCode = null;
            City = null;
        }
    }
}
