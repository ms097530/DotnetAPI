namespace DotnetAPI.DTOs
{
    // when registering, user will need to provide email, PW, and confirm that PW
    public partial class UserForRegistrationDTO
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordConfirm { get; set; } = "";
    }
}