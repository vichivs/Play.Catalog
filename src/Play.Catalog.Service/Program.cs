using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

public class Program
{
    private static ServiceSettings serviceSettings;
    public static void Main(string[] args)
    {
        // Register BSON serializers
        //RegisterBsonSerializers();

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

        /*// Register MongoDbSettings as a singleton
        builder.Services.AddSingleton(serviceProvider =>
        {
            var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionStrings);
            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        builder.Services.AddSingleton<IRepository<Item>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<Item>(database, "items");
        });*/

        builder.Services.AddMongo().AddMongoRepository<Item>("items");

        builder.Services.AddControllers(options => 
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });
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

        // Use top-level route registration
        app.MapControllers(); // This replaces UseRouting and UseEndpoints

        app.Run();
    }

    /*private static void RegisterBsonSerializers()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    }*/
}
