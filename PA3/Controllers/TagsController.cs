using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA3.Models;
using QuoteAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuoteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly QuoteDbContext _context;

        public TagController(QuoteDbContext context)
        {
            _context = context;
        }

        // GET: api/tag
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        // POST: api/tag
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] Tag tag)
        {
            if (string.IsNullOrEmpty(tag.Name))
                return BadRequest("Tag name is required.");

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllTags), new { id = tag.Id }, tag);
        }

        // POST: api/tag/{quoteId}/add
        [HttpPost("{quoteId}/add")]
        public async Task<IActionResult> AddTagToQuote(int quoteId, [FromBody] TagRequest tagRequest)
        {
            if (tagRequest == null || string.IsNullOrEmpty(tagRequest.Name))
                return BadRequest("Tag name is required.");

            // Find the quote by ID
            var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == quoteId);
            if (quote == null)
                return NotFound("Quote not found.");

            // Check if the tag exists
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagRequest.Name);
            if (tag == null)
            {
                // Create new tag if it doesn't exist
                tag = new Tag { Name = tagRequest.Name };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            // Add the tag to the quote via the QuoteTag relationship
            var quoteTag = await _context.QuoteTags
                .FirstOrDefaultAsync(qt => qt.QuoteId == quoteId && qt.TagId == tag.Id);

            if (quoteTag == null)
            {
                quoteTag = new QuoteTag
                {
                    QuoteId = quoteId,
                    TagId = tag.Id
                };
                _context.QuoteTags.Add(quoteTag);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Tag added to quote successfully." });
        }
    }
}
