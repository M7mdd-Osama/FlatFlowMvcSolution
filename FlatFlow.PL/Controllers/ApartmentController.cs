using FlatFlow.DAL.Repositories.Interfaces;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.PL.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly IApartmentRepo _apartmentRepo;

        public ApartmentController(IApartmentRepo apartmentRepo)
        {
            _apartmentRepo = apartmentRepo;
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
                    ImageUrl = a.ApartmentImages.FirstOrDefault()?.Url
                }).ToList(),

                TotalApartments = apartments.Count(),
                TotalCommission = apartments.SelectMany(a => a.Clients).Sum(c => c.Commission ?? 0),
                SearchTerm = searchTerm,
                IsRentedFilter = isRentedFilter
            };

            return View(viewModel);
        }
    }
}
