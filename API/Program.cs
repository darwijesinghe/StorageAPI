using API.Configuration;
using API.Services;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var config  = builder.Configuration;

// Add services to the container.

// Bind configuration
builder.Services
    .Configure<TableStorageOptions>(builder.Configuration.GetSection("AzureStorage"))
    .Configure<BlobStorageOptions>(builder.Configuration.GetSection("AzureStorage"))
    .Configure<QueueStorageOptions>(builder.Configuration.GetSection("AzureStorage"));

// Register table client
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<TableStorageOptions>>().Value;
    return new TableClient(options.ConnectionString, options.TableName);
});

// Register storage client
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<BlobStorageOptions>>().Value;
    return new BlobServiceClient(options.ConnectionString);
});

// Register queue client
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<QueueStorageOptions>>().Value;
    return new QueueClient(options.ConnectionString, options.QueueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.None });
});

// Register storage services
builder.Services.AddSingleton<TableStorageService>();
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<QueueStorageService>();


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
