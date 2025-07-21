using FlatFlow.DAL.Models;
using FlatFlow.DAL.Repositories.Interfaces;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FlatFlow.PL.Helpers;

namespace FlatFlow.PL.Controllers
{
    public class ClientController(
        IApartmentRepo _apartmentRepo,
        IClientRepo _clientRepo) : Controller
    {
        #region Index Page
        public ActionResult Index(int page = 1, int pageSize = 9)
        {
            var viewModel = new ClientIndexViewModel();

            var currentUserId = UserHelper.GetCurrentUserId(User);

            var clients = _clientRepo.GetAllWithApartments().Where(c => c.UserId == currentUserId);

            viewModel.TotalItems = clients.Count();
            viewModel.CurrentPage = page;
            viewModel.PageSize = pageSize;

            var pagedClients = clients
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClientViewModel
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Status = c.Status,
                    Commission = c.Commission,
                    Note = c.Note,
                    Apartment = c.Apartment != null ? new ApartmentViewModel
                    {
                        Id = c.Apartment.Id,
                        Title = c.Apartment.Title,
                        Price = c.Apartment.Price
                    } : null
                }).ToList();

            viewModel.Clients = pagedClients;

            ViewBag.ClientStats = new
            {
                TotalClients = clients.Count(),
                ActiveClients = clients.Count(c => c.Status == "Active"),
                InactiveClients = clients.Count(c => c.Status == "Inactive"),
                TotalCommission = clients.Where(c => c.Commission.HasValue).Sum(c => c.Commission.Value)
            };

            return View(viewModel);
        }

        #endregion

        #region Add Client

        [HttpGet]
        public IActionResult Add()
        {
            var currentUserId = UserHelper.GetCurrentUserId(User);
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
                var currentUserId = UserHelper.GetCurrentUserId(User);

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

            var userId = UserHelper.GetCurrentUserId(User);
            model.Apartments = _apartmentRepo.GetApartmentsByUserId(userId).ToList();
            return View(model);
        }

        #endregion

        #region Client Details
        [HttpGet]
        public IActionResult Details(int id)
        {
            var currentUserId = UserHelper.GetCurrentUserId(User);
            var client = _clientRepo.GetAllWithApartments()
                .FirstOrDefault(c => c.Id == id && c.UserId == currentUserId);

            if (client == null)
            {
                TempData["ClientError"] = "Client not found or you don't have permission to view it!";
                return RedirectToAction("Index");
            }
            return View(client);
        }

        #endregion

        #region Update Client
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var currentUserId = UserHelper.GetCurrentUserId(User);
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
                var currentUserId = UserHelper.GetCurrentUserId(User);
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

            var userId = UserHelper.GetCurrentUserId(User);
            model.Apartments = _apartmentRepo.GetApartmentsByUserId(userId).ToList();
            return View(model);
        }

        #endregion

        #region Delete Client
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var currentUserId = UserHelper.GetCurrentUserId(User);
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
            var currentUserId = UserHelper.GetCurrentUserId(User);
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

        #endregion

    }
}