using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/socialmediaposts")]
[Authorize]
public class SocialMediaPostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SocialMediaPostsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SocialMediaPost>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.SocialMediaPosts
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<SocialMediaPost>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SocialMediaPost>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SocialMediaPost>>> GetById(int id)
    {
        try
        {
            var item = await _db.SocialMediaPosts.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == id);
            if (item is null)
                return NotFound(ApiResponse<SocialMediaPost>.Fail("Not found"));

            return Ok(ApiResponse<SocialMediaPost>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SocialMediaPost>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SocialMediaPost>>> Create([FromBody] SocialMediaPost post)
    {
        try
        {
            _db.SocialMediaPosts.Add(post);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = post.PostId }, ApiResponse<SocialMediaPost>.Ok(post));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SocialMediaPost>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SocialMediaPost>>> Update(int id, [FromBody] SocialMediaPost post)
    {
        try
        {
            if (post.PostId != id)
                return BadRequest(ApiResponse<SocialMediaPost>.Fail("ID in route does not match body"));

            var existing = await _db.SocialMediaPosts.FirstOrDefaultAsync(x => x.PostId == id);
            if (existing is null)
                return NotFound(ApiResponse<SocialMediaPost>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(post);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<SocialMediaPost>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SocialMediaPost>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.SocialMediaPosts.FirstOrDefaultAsync(x => x.PostId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.SocialMediaPosts.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

