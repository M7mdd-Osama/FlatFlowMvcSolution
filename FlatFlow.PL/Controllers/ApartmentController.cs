using Demo.BLL.Interfaces;
using FlatFlow.BLL.DTOs;
using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Models.Shared;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlatFlow.PL.Controllers
{
    [Authorize]
    public class ApartmentController(
        IUnitOfWork _unitOfWork,
        IWebHostEnvironment _webHostEnvironment,
        IHttpContextAccessor _httpContextAccessor,
        AppDbContext _dbContext) : Controller
    {

        #region Index Page with Pagination
        public IActionResult Index(string searchTerm, bool? isRentedFilter, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 9)
        {
            var apartments = string.IsNullOrWhiteSpace(searchTerm)
                ? _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                : _unitOfWork.ApartmentRepo.Search(searchTerm);

            // Filter by rental status
            if (isRentedFilter.HasValue)
                apartments = apartments.Where(a => a.IsRented == isRentedFilter).ToList();

            // Filter by minimum price
            if (minPrice.HasValue)
                apartments = apartments.Where(a => a.Price >= minPrice.Value).ToList();

            // Filter by maximum price
            if (maxPrice.HasValue)
                apartments = apartments.Where(a => a.Price <= maxPrice.Value).ToList();

            // Get the current user's apartments only
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                apartments = apartments.Where(a => a.UserId == userId).ToList();
            }

            // Calculate pagination
            var totalItems = apartments.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var skip = (page - 1) * pageSize;
            var paginatedApartments = apartments.Skip(skip).Take(pageSize).ToList();

            var viewModel = new ApartmentIndexViewModel
            {
                Apartments = paginatedApartments.Select(a =>
                {
                    var firstImage = a.ApartmentImages.FirstOrDefault();
                    return new ApartmentCardViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Price = a.Price,
                        Location = a.Location,
                        IsRented = a.IsRented,
                        ImageUrl = firstImage?.Url ?? "/images/default.jpg",
                        IsVideo = firstImage?.IsVideo ?? false
                    };
                }).ToList(),

                // Pagination properties
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages,

                // Statistics (for all items, not just current page)
                TotalApartments = apartments.Count(),
                AvailableCount = apartments.Count(a => !a.IsRented),
                RentedCount = apartments.Count(a => a.IsRented),

                // Search filters
                SearchTerm = searchTerm,
                IsRentedFilter = isRentedFilter,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return View(viewModel);
        }
        #endregion

        #region Apartment Details
        public async Task<IActionResult> Details(int id)
        {
            var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
            {
                TempData["ApartmentError"] = "Apartment not found.";
                return RedirectToAction("Index");
            }

            // Check if the apartment belongs to the current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (apartment.UserId != userId)
            {
                TempData["ApartmentError"] = "You don't have permission to view this apartment.";
                return RedirectToAction("Index");
            }

            var viewModel = new ApartmentDetailsWithGroupsViewModel
            {
                Id = apartment.Id,
                Title = apartment.Title,
                Price = apartment.Price,
                Location = apartment.Location,
                Description = apartment.Description,
                IsRented = apartment.IsRented,
                Images = apartment.ApartmentImages.Select(img => img.Url).ToList(),
                Clients = apartment.Clients.Select(c => new ClientViewModel
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Commission = c.Commission ?? 0
                }).ToList()
            };

            var apartmentGroups = await _dbContext.FacebookGroups
                .Where(g => g.UserId == userId)
                .Select(g => new ApartmentGroupPostViewModel
                {
                    ApartmentId = id,
                    GroupId = g.Id,
                    GroupName = g.GroupName,
                    GroupLink = g.GroupLink,
                    IsPosted = g.ApartmentGroupPosts.Any(p => p.ApartmentId == id && p.IsPosted)
                })
                .OrderBy(g => g.GroupName)
                .ToListAsync();

            viewModel.FacebookGroups = apartmentGroups;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostStatus([FromBody] TogglePostStatusRequest request)
        {
            try
            {
                // Validate the request
                if (request == null || request.ApartmentId <= 0 || request.GroupId <= 0)
                {
                    return Json(new { success = false, message = "Invalid request data." });
                }

                // Get current user ID
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                // Verify that the apartment belongs to the current user
                var apartment = await _dbContext.Apartments
                    .FirstOrDefaultAsync(a => a.Id == request.ApartmentId && a.UserId == userId);

                if (apartment == null)
                {
                    return Json(new { success = false, message = "Apartment not found or you don't have permission to modify it." });
                }

                // Verify that the Facebook group belongs to the current user
                var group = await _dbContext.FacebookGroups
                    .FirstOrDefaultAsync(g => g.Id == request.GroupId && g.UserId == userId);

                if (group == null)
                {
                    return Json(new { success = false, message = "Facebook group not found or you don't have access to it." });
                }

                // Find existing post or create new one
                var existingPost = await _dbContext.ApartmentGroupPosts
                    .FirstOrDefaultAsync(p => p.ApartmentId == request.ApartmentId && p.GroupId == request.GroupId);

                if (existingPost != null)
                {
                    // Update existing post status
                    existingPost.IsPosted = request.IsPosted;
                }
                else
                {
                    // Create new post record
                    _dbContext.ApartmentGroupPosts.Add(new ApartmentGroupPost
                    {
                        ApartmentId = request.ApartmentId,
                        GroupId = request.GroupId,
                        IsPosted = request.IsPosted
                    });
                }
                await _unitOfWork.CompleteAsync();

                // Return success response
                var statusMessage = request.IsPosted ? "marked as posted" : "marked as not posted";
                return Json(new
                {
                    success = true,
                    message = $"Post status successfully {statusMessage} for group '{group.GroupName}'!"
                });
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to use a proper logging framework)
                // _logger.LogError(ex, "Error occurred while toggling post status");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the post status. Please try again."
                });
            }
        }

        #endregion

        #region Add Apartment

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AddApartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get current user ID
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "User not found. Please log in again.");
                    return View(model);
                }

                var apartment = new Apartment
                {
                    Title = model.Title,
                    Price = model.Price,
                    Description = model.Description,
                    Location = model.Location,
                    IsRented = false,
                    UserId = userId,
                    ApartmentImages = new List<ApartmentImage>()
                };

                // Handle media uploads (images and videos) - OPTIONAL NOW
                if (model.Images != null && model.Images.Any() && model.Images.First().Length > 0)
                {
                    foreach (var file in model.Images)
                    {
                        // Skip empty files
                        if (file.Length == 0) continue;

                        // Validate file type
                        var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var allowedVideoExtensions = new[] { ".mp4", ".mov", ".avi" };
                        var allAllowedExtensions = allowedImageExtensions.Concat(allowedVideoExtensions).ToArray();

                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allAllowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Images", "Only JPG, JPEG, PNG, MP4, MOV, AVI files are allowed");
                            return View(model);
                        }

                        // Validate file size (100MB for all files)
                        var maxSize = 100 * 1024 * 1024; // 100MB for all files
                        if (file.Length > maxSize)
                        {
                            ModelState.AddModelError("Images", "File size cannot exceed 100MB");
                            return View(model);
                        }

                        // Generate unique file name
                        var fileName = Guid.NewGuid().ToString() + extension;

                        // Determine folder based on file type
                        var folderName = allowedVideoExtensions.Contains(extension) ? "videos" : "images";
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "apartments", folderName);

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        apartment.ApartmentImages.Add(new ApartmentImage
                        {
                            Url = $"/uploads/apartments/{folderName}/{fileName}",
                            IsVideo = allowedVideoExtensions.Contains(extension) // Add this property if needed
                        });
                    }
                }

                _unitOfWork.ApartmentRepo.Add(apartment);
                TempData["ApartmentSuccess"] = "Apartment added successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        #endregion

        #region Update Apartment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditApartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                    .FirstOrDefault(a => a.Id == model.Id);

                if (apartment == null)
                {
                    TempData["ApartmentError"] = "Apartment not found.";
                    return RedirectToAction("Index");
                }

                // Check user permission
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (apartment.UserId != userId)
                {
                    TempData["ApartmentError"] = "You don't have permission to edit this apartment.";
                    return RedirectToAction("Index");
                }

                // Update apartment details
                apartment.Title = model.Title;
                apartment.Price = model.Price;
                apartment.Description = model.Description;
                apartment.Location = model.Location;
                apartment.IsRented = model.IsRented;

                // Handle image deletions
                if (!string.IsNullOrEmpty(model.ImagesToDelete))
                {
                    var imageUrlsToDelete = model.ImagesToDelete.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var imagesToRemove = new List<ApartmentImage>();

                    foreach (var imageUrl in imageUrlsToDelete)
                    {
                        var imageToDelete = apartment.ApartmentImages.FirstOrDefault(img => img.Url == imageUrl.Trim());
                        if (imageToDelete != null)
                        {
                            // Delete physical file
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.Trim().TrimStart('/'));
                            if (System.IO.File.Exists(filePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to delete file {filePath}: {ex.Message}");
                                }
                            }
                            imagesToRemove.Add(imageToDelete);
                        }
                    }

                    // Remove images from database using repository method
                    if (imagesToRemove.Any())
                    {
                        _unitOfWork.ApartmentRepo.RemoveImages(imagesToRemove);

                        // Remove from apartment collection as well
                        foreach (var img in imagesToRemove)
                        {
                            apartment.ApartmentImages.Remove(img);
                        }
                    }
                }

                // Handle new uploads (same as before)
                if (model.Images != null && model.Images.Any())
                {
                    foreach (var file in model.Images)
                    {
                        // ... validation code (same as before)
                        var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var allowedVideoExtensions = new[] { ".mp4", ".mov", ".avi" };
                        var allAllowedExtensions = allowedImageExtensions.Concat(allowedVideoExtensions).ToArray();

                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allAllowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Images", "Only JPG, JPEG, PNG, MP4, MOV, AVI files are allowed");
                            model.ExistingImages = apartment.ApartmentImages.Select(img => img.Url).ToList();
                            return View(model);
                        }

                        var maxSize = 100 * 1024 * 1024;
                        if (file.Length > maxSize)
                        {
                            ModelState.AddModelError("Images", "File size cannot exceed 100MB");
                            model.ExistingImages = apartment.ApartmentImages.Select(img => img.Url).ToList();
                            return View(model);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var folderName = allowedVideoExtensions.Contains(extension) ? "videos" : "images";
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "apartments", folderName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        apartment.ApartmentImages.Add(new ApartmentImage
                        {
                            Url = $"/uploads/apartments/{folderName}/{fileName}",
                            ApartmentId = apartment.Id,
                            IsVideo = allowedVideoExtensions.Contains(extension)
                        });
                    }
                }

                try
                {
                    _unitOfWork.ApartmentRepo.Update(apartment);
                    TempData["ApartmentSuccess"] = "Apartment updated successfully!";
                    return RedirectToAction("Details", new { id = apartment.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the apartment. Please try again.");
                    Console.WriteLine($"Error updating apartment: {ex.Message}");
                }
            }

            // Redisplay form if validation failed
            var existingApartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                .FirstOrDefault(a => a.Id == model.Id);
            if (existingApartment != null)
            {
                model.ExistingImages = existingApartment.ApartmentImages.Select(img => img.Url).ToList();
            }

            return View(model);
        }

        // GET: Edit Apartment
        public IActionResult Edit(int id)
        {
            var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
            {
                TempData["ApartmentError"] = "Apartment not found.";
                return RedirectToAction("Index");
            }

            // Check if the apartment belongs to the current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (apartment.UserId != userId)
            {
                TempData["ApartmentError"] = "You don't have permission to edit this apartment.";
                return RedirectToAction("Index");
            }

            var viewModel = new EditApartmentViewModel
            {
                Id = apartment.Id,
                Title = apartment.Title,
                Price = apartment.Price,
                Description = apartment.Description,
                Location = apartment.Location,
                IsRented = apartment.IsRented,
                ExistingImages = apartment.ApartmentImages.Select(img => img.Url).ToList()
            };

            return View(viewModel);
        }

        #endregion

        #region Delete Apartment

        // Delete Image
        [HttpPost]
        public IActionResult DeleteImage(int apartmentId, string imageUrl)
        {
            try
            {
                var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                    .FirstOrDefault(a => a.Id == apartmentId);

                if (apartment == null)
                {
                    return Json(new { success = false, message = "Apartment not found" });
                }

                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (apartment.UserId != userId)
                {
                    return Json(new { success = false, message = "Permission denied" });
                }

                var imageToDelete = apartment.ApartmentImages.FirstOrDefault(img => img.Url == imageUrl);
                if (imageToDelete != null)
                {
                    // Delete file from disk
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Get the image ID for direct deletion
                    var imageId = imageToDelete.Id;

                    // Remove from collection
                    apartment.ApartmentImages.Remove(imageToDelete);

                    // Update the apartment
                    _unitOfWork.ApartmentRepo.Update(apartment);

                    return Json(new { success = true, message = "Image deleted successfully" });
                }

                return Json(new { success = false, message = "Image not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Delete Apartment
        public IActionResult Delete(int id)
        {
            var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
            {
                TempData["ApartmentError"] = "Apartment not found.";
                return RedirectToAction("Index");
            }

            // Check if the apartment belongs to the current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (apartment.UserId != userId)
            {
                TempData["ApartmentError"] = "You don't have permission to delete this apartment.";
                return RedirectToAction("Index");
            }

            var viewModel = new ApartmentCardViewModel
            {
                Id = apartment.Id,
                Title = apartment.Title,
                Price = apartment.Price,
                Location = apartment.Location,
                IsRented = apartment.IsRented,
                ImageUrl = apartment.ApartmentImages.FirstOrDefault()?.Url ?? "/images/default.jpg"
            };

            return View(viewModel);
        }

        // POST: Delete Apartment
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
            {
                TempData["ApartmentError"] = "Apartment not found.";
                return RedirectToAction("Index");
            }

            // Check if the apartment belongs to the current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (apartment.UserId != userId)
            {
                TempData["ApartmentError"] = "You don't have permission to delete this apartment.";
                return RedirectToAction("Index");
            }

            // Check if apartment has clients
            if (apartment.Clients != null && apartment.Clients.Any())
            {
                TempData["ApartmentError"] = "Cannot delete apartment that has clients assigned to it.";
                return RedirectToAction("Index");
            }

            // Delete images from disk
            foreach (var image in apartment.ApartmentImages)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _unitOfWork.ApartmentRepo.Remove(apartment);
            TempData["ApartmentSuccess"] = "Apartment deleted successfully!";
            return RedirectToAction("Index");
        }

        #endregion

        #region Toggle Rent Status

        [HttpPost]
        public IActionResult ToggleRentStatus(int id)
        {
            var apartment = _unitOfWork.ApartmentRepo.GetWithImagesAndClients()
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
            {
                return Json(new { success = false, message = "Apartment not found" });
            }

            // Check if the apartment belongs to the current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (apartment.UserId != userId)
            {
                return Json(new { success = false, message = "Permission denied" });
            }

            apartment.IsRented = !apartment.IsRented;
            _unitOfWork.ApartmentRepo.Update(apartment);

            return Json(new
            {
                success = true,
                message = $"Apartment marked as {(apartment.IsRented ? "rented" : "available")}",
                isRented = apartment.IsRented
            });
        }

        #endregion  
    }
}