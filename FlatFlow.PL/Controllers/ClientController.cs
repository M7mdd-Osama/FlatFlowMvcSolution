using FlatFlow.DAL.Models;
using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Repositories.Interfaces;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.PL.Controllers
{
    public class ClientController : Controller
    {
        private readonly IGenericRepo<Client> _clientRepo;
        private readonly IGenericRepo<Apartment> _apartmentRepo;

        public ClientController(
            IGenericRepo<Client> clientRepo,
            IGenericRepo<Apartment> apartmentRepo)
        {
            _clientRepo = clientRepo;
            _apartmentRepo = apartmentRepo;
        }

        public IActionResult Index()
        {
            var clients = _clientRepo.GetAll();
            return View(clients);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var apartments = _apartmentRepo.GetAll();
            var viewModel = new AddClientViewModel
            {
                Apartments = apartments.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AddClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = new Client
                {
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Status = model.Status,
                    Note = model.Note,
                    Commission = model.Commission,
                    ApartmentId = model.ApartmentId,
                    UserId = 1 // Replace with actual user ID after implementing authentication
                };

                _clientRepo.Add(client);
                TempData["Success"] = "Client added successfully!";
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            model.Apartments = _apartmentRepo.GetAll().ToList();
            return View(model);
        }
    }
}