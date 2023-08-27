using AHAApi;
using AHAApi.Repository;
using AHAApi.Repository.Interfaces;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalClient", builder =>
    {
        builder.AllowAnyOrigin()  // Replace with the allowed origin(s)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

builder.Services.AddSingleton<IMongoDatabase>(provider =>
{
    var client = provider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(configuration.GetSection("CosmosDBSettings")["DatabaseName"]);
});
builder.Services.AddScoped<InterviewProfilesRepository>();
builder.Services.AddScoped<JobPostingRepository>();
// Register Cosmos DB connection
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    return new MongoClient(configuration.GetConnectionString("CosmosDBConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "AHAA-API");
    });
}

app.UseCors("AllowLocalClient");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
