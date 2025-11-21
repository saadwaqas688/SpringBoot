using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
        return user;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Users.DeleteOneAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        return user != null;
    }
}

