using ApiUserService.Context;
using ApiUserService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ApiUserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserServiceContext _context;
    public UsersController(UserServiceContext context)
    {
        _context = context;
    }

    private void PublishToMessageQueue(string integrationEvent, string eventData)
    {
        // Ajustes a fazer : Reusar e fechar conexões e channel, e outro detalhes 
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        var body = Encoding.UTF8.GetBytes(eventData);

        channel.BasicPublish(exchange: "user",
                                         routingKey: integrationEvent,
                                         basicProperties: null,
                                         body: body);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUser()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (user == null || id <=0 )
            return BadRequest();

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var integrationEventData = JsonSerializer.Serialize(new
        {
            id = user.Id,
            newname = user.Name
        });

        PublishToMessageQueue("user.update", integrationEventData);

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        if(user is null)
            return BadRequest();

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var integrationEventData = JsonSerializer.Serialize(new
        {
            id = user.Id,
            name = user.Name
        });

        PublishToMessageQueue("user.add", integrationEventData);

        return CreatedAtAction("GetUser", new { id = user.Id }, user);
    }
}
