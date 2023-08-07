using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

// ? good practice to use sub-namespaces so things can be loaded separately/modularly
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

    [HttpGet]
    public IEnumerable<Test> Get()
    {
        Console.WriteLine("TESTING API");
        _logger.Log(new LogLevel(), "Hello Test");
        return new Test[] { new Test(), new Test("DUMMY") };
    }

    [HttpPost]
    public ContentResult Post()
    {
        return Content("<h1>HELLO POST TEST</h1>");
    }

    [HttpPost("{id}")]
    public ContentResult Post(string id)
    {
        Console.WriteLine(id);
        return Content("<h1>HELLO POST BY ID TEST</h1>");
    }

    [HttpGet("{id}")]
    public ContentResult GetById()
    {
        Console.WriteLine("GETTING BY ID");
        return Content("<h1>HELLO GET TEST BY ID</h1>");
    }

    [HttpPut("{id}")]
    public ContentResult Put()
    {
        return Content("<h1>PUTTING TEST</h1>");
    }

    [HttpDelete("{id}")]
    public ContentResult Delete()
    {
        return Content("<h1>DELETING TEST</h1>");
    }
}