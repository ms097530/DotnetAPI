namespace DotnetAPI.DTOs
{
    // when registering, user will need to provide email, PW, and confirm that PW
    partial class UserForRegistrationDTO
    {
        string Email { get; set; } = "";
        string Password { get; set; } = "";
        string PasswordConfirm { get; set; } = "";
    }
}