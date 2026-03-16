using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Controllers;

/// <summary>
/// Handles member registration, login, and logout.
/// </summary>
public class MemberController : Controller
{
	private readonly ProductDbContext _context;

	public MemberController(ProductDbContext context)
	{
		_context = context;
	}

	/// <summary>
	/// Shows the registration form.
	/// </summary>
	public IActionResult Register()
	{
		return View();
	}

	/// <summary>
	/// Creates a new member when the registration form is valid.
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> Register(RegistrationViewModel reg)
	{
		if (ModelState.IsValid)
		{
			// Check if username or email is already taken
			bool usernameTaken = await _context.Members
							.AnyAsync(m => m.Username == reg.Username);

			if (usernameTaken)
			{
				ModelState.AddModelError(nameof(Member.Username), "Username already taken");
				return View(reg);
			}

			// Stop duplicate emails.
			bool emailTaken = await _context.Members
							.AnyAsync(m => m.Email == reg.Email);

			if (emailTaken)
			{
				ModelState.AddModelError(nameof(Member.Email), "Email already taken");
				return View(reg);
			}

			if (usernameTaken || emailTaken)
			{
				return View(reg);
			}

			// Map registration input to a new Member entity.
			Member newMember = new()
			{
				Username = reg.Username,
				Email = reg.Email,
				Password = reg.Password,
				DateOfBirth = reg.DateOfBirth,
			};

			_context.Members.Add(newMember);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Home");
		}

		return View(reg);
	}

	/// <summary>
	/// Shows the login form.
	/// </summary>
	[HttpGet]
	public IActionResult Login()
	{
		return View(); 
	}

	/// <summary>
	/// Logs a member in by checking username/email + password.
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> Login(LoginViewModel login)
	{
		if (ModelState.IsValid)
		{
			// Check if UsernameOrEmail and Pasaword mathces a record in the database
			var loggedInMember = await _context.Members
							.Where(m => (m.Username == login.UsernameOrEmail || m.Email == login.UsernameOrEmail)
								&& m.Password == login.Password)
							.Select(m => new { m.Username, m.MemberId})
							.SingleOrDefaultAsync();

			if (loggedInMember == null)
			{
				ModelState.AddModelError(string.Empty, "Your provided credentials do not match any records in our database");
				return View(login);
			}

			// Log the user in
			HttpContext.Session.SetString("Username", loggedInMember.Username);
			HttpContext.Session.SetInt32("MemberId", loggedInMember.MemberId);

			return RedirectToAction("Index", "Home");
		}
		return View(login);
	}

	public IActionResult Logout()
	{
		// Destroy current session
		HttpContext.Session.Clear();
		return RedirectToAction("Index", "Home");
	}
}
