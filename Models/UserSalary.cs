// ? good practice to use sub-namespaces so things can be loaded separately/modularly
namespace DotnetAPI.Models;

public partial class UserSalary
{
    public int UserId { get; set; }

    public decimal Salary { get; set; }
}