using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using auths.Models;
using auths.Database;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace auths.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeController> _logger;


        // Inject UserManager and SignInManager
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [Route("/")]
        [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                return View(model); // Return the view with validation errors
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Age = model.Age
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error creating user: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }





        [Authorize]
        [Route("/addproduct")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }
        [Authorize]
        [Route("/addproduct")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized();
            }

            _logger.LogInformation($"User authenticated: {User.Identity.IsAuthenticated}, User name: {User.Identity.Name}");

            // Remove validation for RegisteredId since it's assigned manually in the controller
            ModelState.Remove("RegisteredId");

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                _logger.LogInformation($"Retrieved User ID: {userId}");


                var prod = new Product
                {
                    ProductName = model.ProductName,
                    Price = model.Price,
                    Description = model.Description,
                    RegisteredId = userId // Store the current user's ID
                };

                _context.Products.Add(prod);
                await _context.SaveChangesAsync();

                return View();
            }
            else
            {
                _logger.LogWarning("Model state is invalid.");
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                    }
                }
            }

            return View(model);
        }


        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user exists
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // If the user does not exist, add a specific error message
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                // Attempt password sign-in
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("AddProduct", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return RedirectToAction("Lockout");
                }
                else
                {
                    // If the password is incorrect, add a generic error message
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }

            return View(model);
        }

        [Route("/logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Home");
        }



        // List all products
        [Route("/allproducts")]
        [HttpGet]
        public async Task<IActionResult> AllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // Get product for editing

        [Route("updateproducts/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateProducts(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Post the updated product details
        [Route("updateproducts/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateProducts(Product product)
        {
            // Remove validation for RegisteredId since it's assigned manually in the controller
            ModelState.Remove("RegisteredId");
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Find the existing product by ID
            var existingProduct = await _context.Products.FindAsync(product.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update properties of the existing product
            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;

            // Save changes to the database
            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            // Redirect to the AllProducts page
            return RedirectToAction("AllProducts");
        }

        // Delete a product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllProducts");
        }
    }
}
