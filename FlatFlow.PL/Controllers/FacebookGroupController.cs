using Demo.BLL.Interfaces;
using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models;
using FlatFlow.DAL.Models.Shared;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlatFlow.PL.Controllers
{
    [Authorize]
    public class FacebookGroupController(AppDbContext _dbContext,IUnitOfWork _unitOfWork) : Controller
    {
        #region Index
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _dbContext.FacebookGroups
                .Include(g => g.ApartmentGroupPosts)
                .Where(g => g.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(g => g.GroupName.Contains(searchTerm));
                ViewData["SearchTerm"] = searchTerm;
            }

            var groups = await query.OrderBy(g => g.GroupName).ToListAsync();

            var viewModel = groups.Select(g => new FacebookGroupViewModel
            {
                Id = g.Id,
                GroupName = g.GroupName,
                GroupLink = g.GroupLink,
                PostsCount = g.ApartmentGroupPosts?.Count ?? 0,
            }).ToList();

            return View(viewModel);
        }

        #endregion

        #region Details
        public async Task<IActionResult> Details(int id)
        {
            var group = await _dbContext.FacebookGroups
                .Include(g => g.ApartmentGroupPosts)
                    .ThenInclude(p => p.Apartment)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var viewModel = new FacebookGroupDetailsViewModel
            {
                Id = group.Id,
                GroupName = group.GroupName,
                GroupLink = group.GroupLink,
                PostsCount = group.ApartmentGroupPosts?.Count ?? 0,
                ApartmentPosts = group.ApartmentGroupPosts?.ToList(),
            };

            return View(viewModel);
        }

        #endregion

        #region Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: FacebookGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(FacebookGroupCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var existingGroup = await _dbContext.FacebookGroups
                        .FirstOrDefaultAsync(g => g.GroupLink == model.GroupLink && g.UserId == userId);

                    if (existingGroup != null)
                    {
                        ModelState.AddModelError("GroupLink", "You have already added this Facebook group.");
                        return View(model);
                    }

                    var group = new FacebookGroup
                    {
                        GroupName = model.GroupName,
                        GroupLink = model.GroupLink,
                        UserId = userId
                    };

                    _dbContext.FacebookGroups.Add(group);
                    await _unitOfWork.CompleteAsync();

                    TempData["SuccessMessage"] = "Facebook group added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding Facebook group: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                }
            }

            return View(model);
        }

        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _dbContext.FacebookGroups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            var viewModel = new FacebookGroupEditViewModel
            {
                Id = group.Id,
                GroupName = group.GroupName,
                GroupLink = group.GroupLink
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacebookGroupEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var group = await _dbContext.FacebookGroups.FindAsync(id);
                    if (group == null)
                    {
                        return NotFound();
                    }

                    // Check if another group with same link exists
                    var existingGroup = await _dbContext.FacebookGroups
                        .FirstOrDefaultAsync(g => g.GroupLink == model.GroupLink && g.Id != id);

                    if (existingGroup != null)
                    {
                        ModelState.AddModelError("GroupLink", "Another Facebook group with this link already exists.");
                        return View(model);
                    }

                    group.GroupName = model.GroupName;
                    group.GroupLink = model.GroupLink;

                    _dbContext.Update(group);
                    await _unitOfWork.CompleteAsync();

                    TempData["SuccessMessage"] = "Facebook group updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacebookGroupExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Delete
        // GET: FacebookGroup/Delete/id
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _dbContext.FacebookGroups
                .Include(g => g.ApartmentGroupPosts)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var viewModel = new FacebookGroupDeleteViewModel
            {
                Id = group.Id,
                GroupName = group.GroupName,
                GroupLink = group.GroupLink,
                PostsCount = group.ApartmentGroupPosts?.Count ?? 0
            };

            return View(viewModel);
        }

        // POST: FacebookGroup/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _dbContext.FacebookGroups
                .Include(g => g.ApartmentGroupPosts)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            // Remove all related apartment posts first
            if (group.ApartmentGroupPosts != null && group.ApartmentGroupPosts.Any())
            {
                _dbContext.ApartmentGroupPosts.RemoveRange(group.ApartmentGroupPosts);
            }

            _dbContext.FacebookGroups.Remove(group);
            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = "Facebook group deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region AJAX: Toggle post status for apartment in specific group

        [HttpPost]
        public async Task<IActionResult> TogglePostStatus(int apartmentId, int groupId)
        {
            try
            {
                var apartment = await _dbContext.Apartments.FindAsync(apartmentId);
                if (apartment == null)
                {
                    return Json(new { success = false, message = "Apartment not found" });
                }

                var group = await _dbContext.FacebookGroups.FindAsync(groupId);
                if (group == null)
                {
                    return Json(new { success = false, message = "Group not found" });
                }

                var existingPost = await _dbContext.ApartmentGroupPosts
                    .FirstOrDefaultAsync(p => p.ApartmentId == apartmentId && p.GroupId == groupId);

                if (existingPost != null)
                {
                    existingPost.IsPosted = !existingPost.IsPosted;
                }
                else
                {
                    var newPost = new ApartmentGroupPost
                    {
                        ApartmentId = apartmentId,
                        GroupId = groupId,
                        IsPosted = true,
                    };
                    _dbContext.ApartmentGroupPosts.Add(newPost);
                }

                await _unitOfWork.CompleteAsync();

                return Json(new
                {
                    success = true,
                    message = "Status updated successfully",
                    isPosted = existingPost?.IsPosted ?? true
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the status" });
            }
        }

        #endregion

        #region AJAX: Get apartment groups with post status

        [HttpGet]
        public async Task<IActionResult> GetApartmentGroups(int apartmentId)
        {
            try
            {
                var groups = await _dbContext.FacebookGroups
                    .Select(g => new
                    {
                        g.Id,
                        g.GroupName,
                        g.GroupLink,
                        IsPosted = g.ApartmentGroupPosts
                            .Any(p => p.ApartmentId == apartmentId && p.IsPosted)
                    })
                    .OrderBy(g => g.GroupName)
                    .ToListAsync();

                return Json(new { success = true, groups = groups });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region AJAX: Get group statistics

        [HttpGet]
        public async Task<IActionResult> GetGroupStats(int groupId)
        {
            try
            {
                var stats = await _dbContext.ApartmentGroupPosts
                    .Where(p => p.GroupId == groupId)
                    .GroupBy(p => p.GroupId)
                    .Select(g => new
                    {
                        TotalPosts = g.Count(),
                        PostedCount = g.Count(p => p.IsPosted),
                        NotPostedCount = g.Count(p => !p.IsPosted),
                    })
                    .FirstOrDefaultAsync();

                return Json(new { success = true, stats = stats });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private bool FacebookGroupExists(int id)
        {
            return _dbContext.FacebookGroups.Any(e => e.Id == id);
        }

        #endregion

        #region AJAX: Delete Facebook Group

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                var group = await _dbContext.FacebookGroups
                    .Include(g => g.ApartmentGroupPosts)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (group == null)
                {
                    return Json(new { success = false, message = "Group not found." });
                }

                // Remove all related apartment posts first
                if (group.ApartmentGroupPosts != null && group.ApartmentGroupPosts.Any())
                {
                    _dbContext.ApartmentGroupPosts.RemoveRange(group.ApartmentGroupPosts);
                }

                _dbContext.FacebookGroups.Remove(group);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true, message = "Facebook group deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting group: " + ex.Message });
            }
        }

        #endregion    

    }
}