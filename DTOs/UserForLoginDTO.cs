namespace DotnetAPI.DTOs
{
    // used for logging in, user only needs to provide email and PW
    partial class UserForLoginDTO
    {
        string Email { get; set; } = "";
        string Password { get; set; } = "";
    }
}