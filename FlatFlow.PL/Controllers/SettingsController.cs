using Demo.BLL.Interfaces;
using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models.Identity;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Controllers
{
    [Authorize]
    public class SettingsController(IUnitOfWork _unitOfWork,
        UserManager<User> _userManager,
        SignInManager<User> _signInManager,
        AppDbContext _context) : Controller
    {
        #region Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Get some stats for dashboard
            var apartmentsCount = await _context.Apartments.CountAsync(a => a.UserId == user.Id);
            var clientsCount = await _context.Clients.CountAsync(c => c.UserId == user.Id);
            var groupsCount = await _context.FacebookGroups.CountAsync(g => g.UserId == user.Id);

            var totalRevenue = await _context.Clients
                .Where(c => c.UserId == user.Id && c.Commission.HasValue)
                .SumAsync(c => c.Commission ?? 0);

            ViewBag.ApartmentsCount = apartmentsCount;
            ViewBag.ClientsCount = clientsCount;
            ViewBag.GroupsCount = groupsCount;
            ViewBag.totalRevenue = totalRevenue;

            return View(user);
        }

        #endregion

        #region Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(User model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name is required.");
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        #endregion

        #region ChangePassword

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password and confirmation password do not match.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        #endregion

        #region Account
        public async Task<IActionResult> Account()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            // Get some stats for dashboard
            var apartmentsCount = await _context.Apartments.CountAsync(a => a.UserId == user.Id);
            var clientsCount = await _context.Clients.CountAsync(c => c.UserId == user.Id);
            var groupsCount = await _context.FacebookGroups.CountAsync(g => g.UserId == user.Id);

            var totalRevenue = await _context.Clients
                .Where(c => c.UserId == user.Id && c.Commission.HasValue)
                .SumAsync(c => c.Commission ?? 0);

            ViewBag.ApartmentsCount = apartmentsCount;
            ViewBag.ClientsCount = clientsCount;
            ViewBag.GroupsCount = groupsCount;
            ViewBag.totalRevenue = totalRevenue;

            return View(user);
        }

        #endregion

        #region DeleteAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                // Start a database transaction to ensure all operations succeed or fail together
                using var transaction = await _context.Database.BeginTransactionAsync();

                var userClients = await _context.Clients
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();
                _context.Clients.RemoveRange(userClients);

                var userGroups = await _context.FacebookGroups
                    .Where(g => g.UserId == user.Id)
                    .ToListAsync();
                _context.FacebookGroups.RemoveRange(userGroups);

                var userApartments = await _context.Apartments
                    .Where(a => a.UserId == user.Id)
                    .ToListAsync();
                _context.Apartments.RemoveRange(userApartments);

                await _unitOfWork.CompleteAsync();

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Commit the transaction
                    await transaction.CommitAsync();

                    // Sign out the user to clear the authentication session
                    await _signInManager.SignOutAsync();

                    TempData["AccountDeleted"] = "Your account has been successfully deleted.";

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    // If user deletion failed, rollback the transaction
                    await transaction.RollbackAsync();

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return View("Account", user);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting your account. Please try again or contact support.");
                return View("Account", user);
            }
        }
        #endregion
    }
}
