using System.Security.Claims;
using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Repositories.Interfaces;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.PL.Controllers
{
    [Authorize]
    public class ApartmentController : Controller
    {
        private readonly IApartmentRepo _apartmentRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApartmentController(
            IApartmentRepo apartmentRepo,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _apartmentRepo = apartmentRepo;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(string searchTerm, bool? isRentedFilter)
        {
            var apartments = string.IsNullOrWhiteSpace(searchTerm)
                ? _apartmentRepo.GetWithImagesAndClients()
                : _apartmentRepo.Search(searchTerm);

            if (isRentedFilter.HasValue)
                apartments = apartments.Where(a => a.IsRented == isRentedFilter).ToList();

            var viewModel = new ApartmentIndexViewModel
            {
                Apartments = apartments.Select(a => new ApartmentCardViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Price = a.Price,
                    Location = a.Location,
                    IsRented = a.IsRented,
                    ImageUrl = a.ApartmentImages.FirstOrDefault()?.Url ?? "/images/default.jpg"
                }).ToList(),

                TotalApartments = apartments.Count(),
                TotalCommission = apartments.SelectMany(a => a.Clients).Sum(c => c.Commission ?? 0),
                SearchTerm = searchTerm,
                IsRentedFilter = isRentedFilter
            };

            return View(viewModel);
        }

        // GET: Add Apartment
        public IActionResult Add()
        {
            return View();
        }

        // POST: Add Apartment
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

                // Handle image uploads
                if (model.Images != null && model.Images.Any())
                {
                    foreach (var image in model.Images)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Images", "Only JPG, JPEG, PNG files are allowed");
                            return View(model);
                        }

                        // Validate file size (5MB max)
                        if (image.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Images", "File size cannot exceed 5MB");
                            return View(model);
                        }

                        // Generate unique file name
                        var fileName = Guid.NewGuid().ToString() + extension;
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "apartments");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }

                        apartment.ApartmentImages.Add(new ApartmentImage
                        {
                            Url = $"/uploads/apartments/{fileName}"
                        });
                    }
                }

                _apartmentRepo.Add(apartment);
                TempData["Success"] = "Apartment added successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}