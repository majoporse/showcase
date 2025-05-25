using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace powerPalsDb.Models;

public class Post
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime? EditedAt { get; set; }
    public DateTime Date { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
    public List<Tag> Tags { get; set; } = null;
}
