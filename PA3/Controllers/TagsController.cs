using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoteAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuoteAPI.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly QuoteDbContext _context;

        // Constructor for Dependency Injection
        public TagController(QuoteDbContext context)
        {
            _context = context;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            // Retrieve all tags from the database
            return await _context.Tags.ToListAsync();
        }

        // GET: api/tags/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            // Retrieve a single tag by ID
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // POST: api/tags
        [HttpPost]
        public async Task<ActionResult<Tag>> AddTag([FromBody] Tag tag)
        {
            // Prevent duplicate tags
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
            if (existingTag != null)
            {
                return Conflict(new { message = "Tag already exists" });
            }

            // Add new tag to the database
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }

        // PUT: api/tags/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] Tag tag)
        {
            // Ensure tag IDs match
            if (id != tag.Id)
            {
                return BadRequest();
            }

            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/tags/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            // Remove the tag from the QuoteTags join table
            var quoteTags = _context.QuoteTags.Where(qt => qt.TagId == id);
            _context.QuoteTags.RemoveRange(quoteTags);

            // Remove the tag from the Tags table
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}
