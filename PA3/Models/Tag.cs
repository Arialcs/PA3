using PA3.Models;
using System.ComponentModel.DataAnnotations;

namespace QuoteAPI.Models;

using System.Collections.Generic;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<QuoteTag> QuoteTags { get; set; } = new List<QuoteTag>();
}

