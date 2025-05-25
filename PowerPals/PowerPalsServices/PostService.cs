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

public class PostService
{
    private IDbContextFactory<PowerPalsContext> _contextFactory;

    public PostService(IDbContextFactory<PowerPalsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task<Post?> GetPostByIdAsync(int id)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Posts.FindAsync(id);
    }

    public async Task UpdatePostAsync(Post post)
    {
        using var db = _contextFactory.CreateDbContext();
        db.Posts.Update(post);
        await db.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        using var db = _contextFactory.CreateDbContext();
        var post = await db.Posts.FindAsync(id);
        if (post != null)
        {
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<Post>> GetPostsByUserWithTagsAsync(int userId, List<string> tags)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Posts
                    .Include(p => p.Tags)
                    .Include(p => p.User)
                    .Where(p => p.User.Id == userId && p.Tags.Any(t => tags.Contains(t.Name)))
                    .ToListAsync();
    }

    public async Task<List<Post>> GetPostsOfFollowing(int userId)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Friendships
            .Where(f => f.User1Id == userId)
            .Join(
                db.Posts,
                f => f.User2Id,
                p => p.User.Id,
                (f, p) => p)
            .ToListAsync();
    }

    public async Task<List<Post>> GetPostsOfFollowingWithTags(int userId, List<string> tags)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Posts
            .Include(p => p.Tags)
            .Include(p => p.User)
            .Join(
                db.Friendships,
                p => p.UserId,
                f => f.User2Id,
                (p, f) => new { p, f })
			.Where(pf => pf.p.Tags.Any(t => tags.Contains(t.Name)) && pf.f.User1Id == userId)
            .Select(pf => pf.p)
			.ToListAsync();
	}

    public async Task CreatePostWithTags(Post post, List<string> tagNames)
    {
		using var db = _contextFactory.CreateDbContext();
		var tags = tagNames.Select(t => new Tag { Name = t }).ToList();
		post.Tags = tags;
		await db.Posts.AddAsync(post);
		await db.SaveChangesAsync();
	}
}
