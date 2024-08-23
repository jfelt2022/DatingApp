using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public async Task<MemberDto?> GetMemberAsync(string userName)
    {

        return await context.Users
            .Where(x => x.NormalizedUserName == userName.ToUpper())
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto?>> GetMembersAsync()
    {
        return await context.Users
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    async Task<AppUser?> IUserRepository.GetUserByIdAsync(int id)
    {
        return await context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
    }

    async Task<AppUser?> IUserRepository.GetUserByUserNameAsync(string userName)
    {
        return await context.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => (x.UserName + "").ToLower() == (userName + "").ToLower());
    }

    async Task<IEnumerable<AppUser>> IUserRepository.GetUsersAsync()
    {
        return await context.Users
            .Include(x => x.Photos)
            .ToListAsync();
    }

    

    async Task<bool> IUserRepository.SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    void IUserRepository.Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

}