// ? good practice to use sub-namespaces so things can be loaded separately/modularly
namespace DotnetAPI.Models;

public class Test
{
    public Test()
    {
        Dummy = "is test";
    }
    public Test(string dummy)
    {
        Dummy = dummy;
    }
    public string Dummy { get; set; } = "";
}