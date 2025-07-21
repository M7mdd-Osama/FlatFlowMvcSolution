using FlatFlow.DAL.Models;
using FlatFlow.DAL.Repositories.Interfaces;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlatFlow.PL.Controllers
{
    public class ClientController(
        IApartmentRepo _apartmentRepo,
        IClientRepo _clientRepo) : Controller
    {
        public IActionResult Index()
        {
            var currentUserId = GetCurrentUserId();
            var clients = _clientRepo.GetClientsByUserId(currentUserId);

            var clientStats = new
            {
                TotalClients = clients.Count(),
                ActiveClients = clients.Count(c => c.Status == "Active"),
                InactiveClients = clients.Count(c => c.Status == "Inactive"),
                TotalCommission = clients.Sum(c => c.Commission ?? 0)
            };

            ViewBag.ClientStats = clientStats;

            return View(clients);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var currentUserId = GetCurrentUserId();
            var apartments = _apartmentRepo.GetApartmentsByUserId(currentUserId).ToList();
            var viewModel = new AddClientViewModel
            {
                Apartments = apartments
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AddClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = GetCurrentUserId();

                if (model.ApartmentId.HasValue)
                {
                    var apartmentExists = _apartmentRepo.GetApartmentsByUserId(currentUserId)
                        .Any(a => a.Id == model.ApartmentId.Value);

                    if (!apartmentExists)
                    {
                        ModelState.AddModelError("ApartmentId", "Selected apartment is not valid or you don't have permission to access it.");
                        model.Apartments = _apartmentRepo.GetApartmentsByUserId(currentUserId).ToList();
                        return View(model);
                    }
                }

                var client = new Client
                {
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Status = model.Status,
                    Note = model.Note,
                    Commission = model.Commission,
                    ApartmentId = model.ApartmentId,
                    UserId = currentUserId
                };

                _clientRepo.Add(client);
                TempData["ClientSuccess"] = "Client added successfully!"; // غيرت هنا
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();
            model.Apartments = _apartmentRepo.GetApartmentsByUserId(userId).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var currentUserId = GetCurrentUserId();
            var client = _clientRepo.GetAll()
                .FirstOrDefault(c => c.Id == id && c.UserId == currentUserId);

            if (client == null)
            {
                TempData["ClientError"] = "Client not found or you don't have permission to view it!"; // غيرت هنا
                return RedirectToAction("Index");
            }
            return View(client);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var currentUserId = GetCurrentUserId();
            var client = _clientRepo.GetAll()
                .FirstOrDefault(c => c.Id == id && c.UserId == currentUserId);

            if (client == null)
            {
                TempData["ClientError"] = "Client not found or you don't have permission to edit it!"; // غيرت هنا
                return RedirectToAction("Index");
            }

            var apartments = _apartmentRepo.GetApartmentsByUserId(currentUserId).ToList();
            var viewModel = new EditClientViewModel
            {
                Id = client.Id,
                FullName = client.FullName,
                Phone = client.Phone,
                Status = client.Status,
                Note = client.Note,
                Commission = client.Commission,
                ApartmentId = client.ApartmentId,
                Apartments = apartments
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = GetCurrentUserId();
                var client = _clientRepo.GetAll()
                    .FirstOrDefault(c => c.Id == model.Id && c.UserId == currentUserId);

                if (client == null)
                {
                    TempData["ClientError"] = "Client not found or you don't have permission to edit it!"; // غيرت هنا
                    return RedirectToAction("Index");
                }

                if (model.ApartmentId.HasValue)
                {
                    var apartmentExists = _apartmentRepo.GetApartmentsByUserId(currentUserId)
                        .Any(a => a.Id == model.ApartmentId.Value);

                    if (!apartmentExists)
                    {
                        ModelState.AddModelError("ApartmentId", "Selected apartment is not valid or you don't have permission to access it.");
                        model.Apartments = _apartmentRepo.GetApartmentsByUserId(currentUserId).ToList();
                        return View(model);
                    }
                }

                client.FullName = model.FullName;
                client.Phone = model.Phone;
                client.Status = model.Status;
                client.Note = model.Note;
                client.Commission = model.Commission;
                client.ApartmentId = model.ApartmentId;

                _clientRepo.Update(client);
                TempData["ClientSuccess"] = "Client updated successfully!"; // غيرت هنا
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();
            model.Apartments = _apartmentRepo.GetApartmentsByUserId(userId).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            var client = _clientRepo.GetAll()
                .FirstOrDefault(c => c.Id == id && c.UserId == currentUserId);

            if (client == null)
            {
                TempData["ClientError"] = "Client not found or you don't have permission to delete it!";
                return RedirectToAction("Index");
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var currentUserId = GetCurrentUserId();
            var client = _clientRepo.GetAll()
                .FirstOrDefault(c => c.Id == id && c.UserId == currentUserId);

            if (client == null)
            {
                TempData["ClientError"] = "Client not found or you don't have permission to delete it!"; 
                return RedirectToAction("Index");
            }

            _clientRepo.Remove(client);
            TempData["ClientSuccess"] = "Client deleted successfully!"; 
            return RedirectToAction("Index");
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
            {
                return userIdClaim.Value;
            }

            throw new UnauthorizedAccessException("User not logged in");
        }
    }
}