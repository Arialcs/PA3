using PA3.Models;
using System.ComponentModel.DataAnnotations;

namespace QuoteAPI.Models;
using System.Collections.Generic;

public class Quote
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Author { get; set; } = "Unknown";
    public int Likes { get; set; } = 0;
    public List<QuoteTag> QuoteTags { get; set; } = new List<QuoteTag>();
}