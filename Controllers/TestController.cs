using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<Test> _logger;

    public TestController(ILogger<Test> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTest")]
    public IEnumerable<Test> Get()
    {
        Console.WriteLine("TESTING API");
        return new Test[] { new Test(), new Test("DUMMY") };
    }

    [HttpPost(Name = "PostTest")]
    public string Post()
    {
        return "HELLO TEST";
    }
}