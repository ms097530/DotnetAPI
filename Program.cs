using DotnetAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        const string ROOT = "http://localhost";
        // localhost ports for popular frontend frameworks
        corsBuilder.WithOrigins(ROOT + "4200", ROOT + "3000", ROOT + "8000")
            // requests from these origins can use any HTTP method
            .AllowAnyMethod()
            // allow custom headers
            .AllowAnyHeader()
            // to take in cookies and stuff for auth
            .AllowCredentials();
    });
    options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        const string ROOT = "https://";
        // localhost ports for popular frontend frameworks
        corsBuilder.WithOrigins(ROOT + "myproductionsite.com")
            // requests from these origins can use any HTTP method
            .AllowAnyMethod()
            // allow custom headers
            .AllowAnyHeader()
            // to take in cookies and stuff for auth
            .AllowCredentials();
    });
});

// * <Interface, Class>
// * makes so we can access IUserRepository inside controller
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // use one of registered cors policies from above
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    // use one of registered cors policies from above
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
