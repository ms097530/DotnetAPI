namespace DotnetAPI;

public partial class Users
{
    private int UserId;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Active { get; set; }

}