using ApiPostService.Context;
using ApiPostService.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Sqlite");
builder.Services.AddDbContext<PostServiceContext>(options =>
        options.UseSqlite(connectionString));

var app = builder.Build();

CreateDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

//ListenForIntegrationEvents();
// Injeta o contexto do banco de dados
ListenForIntegrationEvents(app.Services);

app.MapControllers();
app.Run();

static void CreateDatabase(WebApplication app)
{
    var serviceScope = app.Services.CreateScope();
    var dataContext = serviceScope.ServiceProvider.GetService<PostServiceContext>();
    dataContext?.Database.EnsureCreated();
}
static void ListenForIntegrationEvents(IServiceProvider serviceProvider)
{
    var factory = new ConnectionFactory();
    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();
    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServiceProvider = scope.ServiceProvider;
            var dbContext = scopedServiceProvider.
                            GetRequiredService<PostServiceContext>();

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            var data = JObject.Parse(message);
            var type = ea.RoutingKey;

            if (type == "user.add")
            {
                dbContext.Users.Add(new User()
                {
                    Id = data["id"].Value<int>(),
                    Name = data["name"].Value<string>()
                });
                dbContext.SaveChanges();
            }
            else if (type == "user.update")
            {
                var user = dbContext.Users.FirstOrDefault(a => 
                                     a.Id == data["id"].Value<int>());

                user.Name = data["newname"].Value<string>();
                dbContext.SaveChanges();
            }
        }
    };

    channel.BasicConsume(queue: "user.postservice", 
                         autoAck: true, consumer: consumer);
}
