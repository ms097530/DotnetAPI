// ? good practice to use sub-namespaces so things can be loaded separately/modularly
namespace DotnetAPI.Models;
public partial class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Active { get; set; }

}