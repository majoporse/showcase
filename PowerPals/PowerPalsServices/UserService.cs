using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using powerPalsDb.Data;
using powerPalsDb.Models;

namespace PowerPalsServices;

public class UserService
{
    private IDbContextFactory<PowerPalsContext> _contextFactory;

    public UserService(IDbContextFactory<PowerPalsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public  async Task AddUserAsync(User user)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }
    }

    public  async Task UpdateUserAsync(User user)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }
    }

    public  async Task<User?> GetUserByIdAsync(int id)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.Users.FindAsync(id);
        }
    }

    public  async Task<List<User>> GetUsersByNameAsync(string name)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.Users.Where(u => u.Name == name).ToListAsync();
        }
    }

    public async Task<User?> GetUserByNameAsync(string name)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.Users.Where(u => u.Name == name).FirstOrDefaultAsync();
        }
    }

    public async Task<List<User>> GetUsersAsync()
    {
		using (var db = _contextFactory.CreateDbContext())
        {
			return await db.Users.ToListAsync();
		}
	}

    public async Task AddFollowAsync(int user1Id, int user2Id)
    {
		using (var db = _contextFactory.CreateDbContext())
        {
			var friendship = new Friendship
            {
				User1Id = user1Id,
				User2Id = user2Id
			};
			await db.Friendships.AddAsync(friendship);
			await db.SaveChangesAsync();
		}
	}

    public async Task RemoveFollowAsync(int user1Id, int user2Id)
	{
        using (var db = _contextFactory.CreateDbContext())
		{
            var follow = await db.Friendships
				.Where(f => f.User1Id == user1Id && f.User2Id == user2Id)
				.FirstOrDefaultAsync();
			if (follow != null)
			{
				db.Friendships.Remove(follow);
				await db.SaveChangesAsync();
			}
		}
	}

	public async Task<List<User>> GetFollowersAsync(int userId)
	{
		using (var db = _contextFactory.CreateDbContext())
		{
			return await db.Friendships
				.Join(db.Users,
						f => f.User1Id,
						u => u.Id,
						(f, u) => new { f, u })
				.Where(friendship => friendship.f.User2Id == userId)
				.Select(x => x.u).ToListAsync();
		}
	}

	public async Task<List<User>> GetFollowingAsync(int userId)
	{
		using (var db = _contextFactory.CreateDbContext())
		{
			return await db.Friendships
				.Join(db.Users,
					f => f.User2Id,
					u => u.Id,
					(f, u) => new { f, u })
				.Where(friendship => friendship.f.User1Id == userId)
				.Select(x => x.u).ToListAsync();
		}
	}

    public async Task RemoveUserAsync(int id)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            var user = await db.Users.FindAsync(id);
            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
            }
        }
    }
}



