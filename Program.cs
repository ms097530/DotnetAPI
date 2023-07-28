using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
// * need Microsoft.AspNetCore.Authentication.JwtBearer package
// * sets up application to receive JWT back from user to validate
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
            )),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

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

// * authentication and authorization
// ! AUTHENTICATION NEEDS TO COME BEFORE AUTHORIZATION
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
