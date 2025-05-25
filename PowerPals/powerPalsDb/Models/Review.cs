using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace powerPalsDb.Models;

public class Review
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }

    public int stars { get; set; }

    public int UserId { get; set; }

    public DateTime? EditedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public int GymId { get; set; }
    public Gym Gym { get; set; } = null!;
}
