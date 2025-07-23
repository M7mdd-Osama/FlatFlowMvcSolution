using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models.Shared;
using FlatFlow.DAL.Repositories.Interfaces;

namespace FlatFlow.DAL.Repositories.Classes
{
    public class ApartmentGroupPostRepo : IApartmentGroupPostRepo
    {
        private readonly AppDbContext _context;

        public ApartmentGroupPostRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApartmentGroupPost>> GetByApartmentIdAsync(int apartmentId)
        {
            return await _context.ApartmentGroupPosts
                .Where(agp => agp.ApartmentId == apartmentId)
                .Include(agp => agp.Group)
                .ToListAsync();
        }

        public async Task TogglePostStatusAsync(int apartmentId, int groupId, bool isPosted)
        {
            var existingPost = await _context.ApartmentGroupPosts
                .FirstOrDefaultAsync(p => p.ApartmentId == apartmentId && p.GroupId == groupId);

            if (existingPost != null)
            {
                existingPost.IsPosted = isPosted;
                _context.ApartmentGroupPosts.Update(existingPost);
            }
            else if (isPosted)
            {
                var newPost = new ApartmentGroupPost
                {
                    ApartmentId = apartmentId,
                    GroupId = groupId,
                    IsPosted = true,
                };

                await _context.ApartmentGroupPosts.AddAsync(newPost);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsPostedAsync(int apartmentId, int groupId)
        {
            var post = await _context.ApartmentGroupPosts
                .FirstOrDefaultAsync(agp => agp.ApartmentId == apartmentId && agp.GroupId == groupId);

            return post?.IsPosted ?? false;
        }
    }
}
