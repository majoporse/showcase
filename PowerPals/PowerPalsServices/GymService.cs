using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using powerPalsDb.Models;
using powerPalsDb.Data;
using Microsoft.EntityFrameworkCore;

namespace PowerPalsServices;

public class GymService
{
    private IDbContextFactory<PowerPalsContext> _contextFactory;

    public GymService(IDbContextFactory<PowerPalsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddGymAsync(Gym gym)
    {
        using var db = _contextFactory.CreateDbContext();
        await db.Gyms.AddAsync(gym);
        await db.SaveChangesAsync();
    }

    public async Task<List<Gym>> GetGymsAsync()
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Gyms.ToListAsync();
    }

    public async Task<List<Review>> GetReviewsAsync(int gymId)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Reviews.Include(r => r.User).Where(r => r.Gym.Id == gymId).ToListAsync();
    }

    public async Task UpdateGymAsync(Gym gym)
    {
        using var db = _contextFactory.CreateDbContext();
        db.Gyms.Update(gym);
        await db.SaveChangesAsync();
    }

    public async Task DeleteGymAsync(int id)
    {
        using var db = _contextFactory.CreateDbContext();
        var gym = await db.Gyms.FindAsync(id);
        if (gym != null)
        {
            db.Gyms.Remove(gym);
            db.Reviews.RemoveRange(db.Reviews.Where(r => r.GymId == id));
            db.GymGoers.RemoveRange(db.GymGoers.Where(gg => gg.GymId == id));
            await db.SaveChangesAsync();
        }
    }

    public async Task<Gym?> GetGymByIdAsync(int id)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Gyms.FindAsync(id);
    }

    public async Task<List<Gym>> GetGymsByNameAsync(string name)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Gyms.Where(g => g.Name == name).ToListAsync();
    }

    public async Task<List<Gym>> GetGymsByUserAsync(int userId)
	{
		using var db = _contextFactory.CreateDbContext();
		return await db.GymGoers
            .Join(
                db.Gyms,
                gg => gg.GymId,
                g => g.Id,
                (gymGoer, gym) => new { gymGoer, gym })
            .Where(g => g.gymGoer.UserId == userId)
            .Select(g => g.gym)
            .ToListAsync();
	}

    public async Task<List<User>> GetUsersByGymAsync(int gymId)
	{
		using var db = _contextFactory.CreateDbContext();
        return await db.GymGoers
			.Join(
				db.Users,
				gg => gg.UserId,
				u => u.Id,
				(gg, u) => new { gg, u })
			.Where(gg => gg.gg.GymId == gymId)
			.Select(gg => gg.u)
			.ToListAsync();
	}

	public async Task AddGymGoerAsync(int userId, int gymId)
	{
		using var db = _contextFactory.CreateDbContext();
		var gymGoer = new GymGoer { UserId = userId, GymId = gymId };
		await db.GymGoers.AddAsync(gymGoer);
		await db.SaveChangesAsync();
	}

	public async Task RemoveGymGoerAsync(int UserId, int GymId)
	{
		using var db = _contextFactory.CreateDbContext();
		var gymGoer = await db.GymGoers
			.Where(gg => gg.UserId == UserId && gg.GymId == GymId)
			.FirstOrDefaultAsync();
		if (gymGoer != null)
		{
			db.GymGoers.Remove(gymGoer);
			await db.SaveChangesAsync();
		}
	}

    public async Task<List<Gym>> GetGymsByOwner(int userId)
    {
        using var db = _contextFactory.CreateDbContext();
		return await db.Gyms.Where(g => g.UserId == userId).ToListAsync();
	}
}
