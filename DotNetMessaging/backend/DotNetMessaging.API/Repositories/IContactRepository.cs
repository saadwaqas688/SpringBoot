using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IContactRepository : IRepository<Contact>
{
    Task<IEnumerable<Contact>> GetUserContactsAsync(string userId);
    Task<Contact?> GetContactAsync(string userId, string contactUserId);
}

