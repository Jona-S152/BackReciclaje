using BackReciclaje.Repository;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "*";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.Configure<BackReciclaje.Repository.ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
