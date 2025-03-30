using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoteAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/quotes")]
[ApiController]
public class QuotesController : ControllerBase
{
    private readonly QuoteDbContext _context;

    public QuotesController(QuoteDbContext context)
    {
        _context = context;
    }

    // GET: api/quotes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
    {
        return await _context.Quotes.Include(q => q.QuoteTags).ThenInclude(qt => qt.Tag).ToListAsync();
    }

    // GET: api/quotes/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Quote>> GetQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null) return NotFound();
        return quote;
    }

    // POST: api/quotes
    [HttpPost]
    public async Task<ActionResult<Quote>> AddQuote(Quote quote)
    {
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
    }

    // PUT: api/quotes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuote(int id, Quote quote)
    {
        if (id != quote.Id) return BadRequest("Quote ID mismatch.");

        var existingQuote = await _context.Quotes.FindAsync(id);
        if (existingQuote == null) return NotFound("Quote not found.");

       
        existingQuote.Text = quote.Text; // Update quote text
        existingQuote.Author = quote.Author; // Update author

        _context.Entry(existingQuote).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuoteExists(id)) return NotFound("Quote not found.");
            throw;
        }

        return NoContent(); // Return 204 No Content response on success
    }

    // POST: api/quotes/{id}/like
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null) return NotFound();
        quote.Likes++;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/quotes/top
    [HttpGet("top")]
    public async Task<ActionResult<IEnumerable<Quote>>> GetTopQuotes([FromQuery] int count = 10)
    {
        return await _context.Quotes.OrderByDescending(q => q.Likes).Take(count).ToListAsync();
    }
    // GET: api/quotes/most-liked
    [HttpGet("most-liked")]
    public async Task<ActionResult<IEnumerable<Quote>>> GetMostLikedQuotes([FromQuery] int count = 10)
    {
        // Fetch the top 'count' most liked quotes from the database
        var mostLikedQuotes = await _context.Quotes
            .OrderByDescending(q => q.Likes)  // Sort quotes by 'Likes' in descending order
            .Take(count)  // Take the specified number of top quotes
            .Include(q => q.QuoteTags)  // Optional: Include the related tags for each quote
            .ThenInclude(qt => qt.Tag)
            .ToListAsync();

        return Ok(mostLikedQuotes);  // Return the most liked quotes
    }


    // DELETE: api/quotes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null) return NotFound();

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();
        return NoContent(); // Returns 204 No Content response
    }

    // Helper method to check if a quote exists
    private bool QuoteExists(int id)
    {
        return _context.Quotes.Any(e => e.Id == id);
    }
}
