using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.DTOs;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var checkEmail = await _userManager.FindByEmailAsync(loginDTO.Email);
            if(checkEmail is null)
            {
                ModelState.AddModelError("Error", "Email or Password is incorrect!");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(checkEmail.UserName, loginDTO.Password, loginDTO.RememberMe, true);

            if (result.Succeeded)
            {
                var controller = _contextAccessor.HttpContext.Request.Query["controller"];
                var action = _contextAccessor.HttpContext.Request.Query["action"];
                var id = _contextAccessor.HttpContext.Request.Query["id"];
                var seoUrl = _contextAccessor.HttpContext.Request.Query["seoUrl"];
                if (!string.IsNullOrEmpty(controller))
                {
                    return RedirectToAction(action, controller, new {Id = id, SeoUrl = seoUrl});
                }


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Error", "Email or Password is incorrect!");
                return View();
            }
        }


        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Task
        // Multi Threading
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var checkEmail = await _userManager.FindByEmailAsync(registerDTO.Email);
            if(checkEmail is not null)
            {
                ModelState.AddModelError("Error", "Email is already existed!");
                return View();
            }

            User newUser = new()
            {
                Email = registerDTO.Email,
                Firstname = registerDTO.Firstname,
                Lastname = registerDTO.Lastname,
                UserName = registerDTO.Email,
                AboutAuthor = string.Empty,
                PhotoUrl = "/"
            };

            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
