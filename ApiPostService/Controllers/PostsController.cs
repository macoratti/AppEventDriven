using ApiPostService.Context;
using ApiPostService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPostService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly PostServiceContext _context;
    public PostsController(PostServiceContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPost()
    {
        return await _context.Posts.Include(x => x.User).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Post>> PostPost(Post post)
    {
        if (post is null)
            return BadRequest();

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPost", new { id = post.PostId }, post);
    }
}
