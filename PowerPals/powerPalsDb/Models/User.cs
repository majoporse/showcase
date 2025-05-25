using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace powerPalsDb.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Bio { get; set; } = null!;
    public string ProfilePicUrl { get; set; } = null!;
    public ICollection<Friendship> FriendshipsInitiated { get; set; } = null!;
    public ICollection<Friendship> FriendshipsReceived { get; set; } = null!;
    public ICollection<GymGoer> GymGoers { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = null!;
    public ICollection<Gym> Gyms { get; set; } = null!;
}
