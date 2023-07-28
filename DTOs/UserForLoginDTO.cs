namespace DotnetAPI.DTOs
{
    // used for logging in, user only needs to provide email and PW
    public partial class UserForLoginDTO
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}