using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace powerPalsDb.Models;

public class Gym
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int UserId { get; set; }
	public User User { get; set; } = null!;
    public ICollection<GymGoer> GymGoers { get; set; } = null!;

}
