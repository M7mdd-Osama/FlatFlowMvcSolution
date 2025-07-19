using FlatFlow.BLL.Services.Interfaces;
using FlatFlow.DAL.Models.Identity;
using FlatFlow.PL.Helpers;
using FlatFlow.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.PL.Controllers
{
    public class AccountController(
        UserManager<User> _userManager,
        SignInManager<User> _signInManager,
        ILogger<AccountController> _logger,
        IEmailSender _emailSender) : Controller
    {

        #region Login

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Apartment");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                User? user = null;

                if (IsValidEmail.IsValidEmailFunction(viewModel.UserNameOrEmail))
                {
                    user = await _userManager.FindByEmailAsync(viewModel.UserNameOrEmail);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(viewModel.UserNameOrEmail);
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                    return View(viewModel);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    viewModel.Password,
                    viewModel.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.UserName} logged in successfully");

                    if (viewModel.RememberMe)
                    {
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, isPersistent: true);

                        var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                        };

                        await _signInManager.SignInAsync(user, authProperties);
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Apartment");
                    }
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "User account locked out.");
                    return View(viewModel);
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "User account not allowed to sign in.");
                    return View(viewModel);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user: {UserName}", viewModel.UserNameOrEmail);
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View(viewModel);
            }
        }

        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(viewModel.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError(nameof(viewModel.Email), "Email is already registered.");
                    return View(viewModel);
                }

                var existingUserByUserName = await _userManager.FindByNameAsync(viewModel.UserName);
                if (existingUserByUserName != null)
                {
                    ModelState.AddModelError(nameof(viewModel.UserName), "Username is already taken.");
                    return View(viewModel);
                }

                var user = new User()
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"New user registered: {user.UserName}");
                    TempData["SuccessMessage"] = "Account created successfully! Please login.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user: {Email}", viewModel.Email);
                ModelState.AddModelError(string.Empty, "An error occurred while creating your account.");
                return View(viewModel);
            }
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"User {userName} logged out");
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region ForgotPassword

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = new Random().Next(100000, 999999).ToString();

                user.ResetPasswordCode = code;
                user.ResetPasswordCodeExpiry = DateTime.Now.AddMinutes(30);
                await _userManager.UpdateAsync(user);

                await _emailSender.SendEmailAsync(
                    viewModel.Email,
                    "Password Reset Code",
                    $"Your password reset code is: <strong>{code}</strong><br>This code will expire in 30 minutes.");

                return RedirectToAction("ResetPassword", new { email = viewModel.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword");
                ModelState.AddModelError(string.Empty, "An error occurred.");
                return View(viewModel);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region ResetPassword

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                if (string.IsNullOrEmpty(user.ResetPasswordCode) ||
                    user.ResetPasswordCode != viewModel.Code ||
                    user.ResetPasswordCodeExpiry == null ||
                    user.ResetPasswordCodeExpiry < DateTime.Now)
                {
                    ModelState.AddModelError("Code", "The verification code is invalid or has expired.");
                    return View(viewModel);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, viewModel.Password);

                if (result.Succeeded)
                {
                    user.ResetPasswordCode = null;
                    user.ResetPasswordCodeExpiry = null;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword");
                ModelState.AddModelError(string.Empty, "An error occurred while resetting your password.");
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendResetCode([FromBody] ResendCodeViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Ok();
                }

                var newCode = new Random().Next(100000, 999999).ToString();

                user.ResetPasswordCode = newCode;
                user.ResetPasswordCodeExpiry = DateTime.Now.AddMinutes(30);
                await _userManager.UpdateAsync(user);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "New Password Reset Code",
                    $"Your new password reset code is: <strong>{newCode}</strong><br>This code will expire in 30 minutes.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending reset code");
                return StatusCode(500, "Error resending code");
            }
        }

        #endregion

        #region Access Denied

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

    }
}