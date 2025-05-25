using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using powerPalsDb.Models;
using powerPalsDb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace PowerPalsServices;

public class ReviewService
{
    private IDbContextFactory<PowerPalsContext> _contextFactory;

    public ReviewService(IDbContextFactory<PowerPalsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddReviewAsync(Review review)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            db.Reviews.Add(review);
            await db.SaveChangesAsync();
        }
    }

    public async Task UpdateReviewAsync(Review review)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            db.Reviews.Update(review);
            await db.SaveChangesAsync();
        }
    }

    public async Task DeleteReviewAsync(Review review)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
        }
    }

    public async Task<Review?> GetReviewByIdAsync(int id)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.Reviews.FindAsync(id);
        }
    }

    public async Task<List<Review>> GetReviewsByUserAsync(int userId)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.Reviews.Where(r => r.Gym.Id == userId).ToListAsync();
        }
    }

}
