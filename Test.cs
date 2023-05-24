namespace DotnetAPI;

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