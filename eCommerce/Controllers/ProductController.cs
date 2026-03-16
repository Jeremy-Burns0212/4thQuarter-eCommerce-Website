using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Controllers;

public class ProductController : Controller
{
	private readonly ProductDbContext _context;

	public ProductController(ProductDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index(string? searchTerm, decimal? maxPrice, decimal? minPrice, int page = 1)
	{
		const int productsPerPage = 3;

		// Start creating query, doesn't run yet
		IQueryable<Product> query = _context.Products;

		bool isInvalidPriceRange = minPrice.HasValue
			&& maxPrice.HasValue
			&& minPrice.Value > maxPrice.Value;

		if (isInvalidPriceRange)
		{
			ViewData["FilterError"] = "Min Price cannot be greater than Max Price.";
		}

		// Apply filters
		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			query = query.Where(p => p.Title.Contains(searchTerm));
		}

		// Only apply price filters when range is valid
		if (!isInvalidPriceRange && minPrice.HasValue)
		{
			query = query.Where(p => p.Price >= minPrice.Value);
		}

		if (!isInvalidPriceRange && maxPrice.HasValue)
		{
			query = query.Where(p => p.Price <= maxPrice.Value);
		}

		int totalProducts = await query.CountAsync();
		int totalPagesNeeded = (int)Math.Ceiling(totalProducts / (double)productsPerPage);

		if (page < 1) page = 1;
		if (totalPagesNeeded > 0 && page > totalPagesNeeded) page = totalPagesNeeded;

		List<Product> products = await query
			.OrderBy(p => p.Title)
			.Skip((page - 1) * productsPerPage)
			.Take(productsPerPage)
			.ToListAsync();

		ProductListViewModel productListViewModel = new()
		{
			Products = products,
			CurrentPage = page,
			TotalPages = totalPagesNeeded,
			PageSize = productsPerPage,
			TotalItems = totalProducts,
			ProductTitleSearch = searchTerm,
			MinPrice = minPrice,
			MaxPrice = maxPrice
		};

		return View(productListViewModel);
	}

	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Create(Product p) 
	{ 
		if (ModelState.IsValid)
		{
			// Add to database
			_context.Products.Add(p);			//Add the product to the context.
			await _context.SaveChangesAsync(); //Save changes to the database.

			// TempData is used to pass data and will persist over a redirect.
			TempData["Message"] = $"{p.Title} was created successfully!";

			return RedirectToAction(nameof(Index));
		}
		return View(p); // If model state is invslid, return the view with the product data and validation errors.
	}
      
	[HttpGet]
	public IActionResult Edit(int id)
	{
		Product? product = _context.Products
			.Where(p => p.ProductId == id)
			.FirstOrDefault();

		if (product == null)
		{
			return NotFound();
		}

		return View(product);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(Product product)
	{
		if (ModelState.IsValid)
		{
			_context.Update(product); // Update the product in the context.
			await _context.SaveChangesAsync(); // Save changes to the database.

			TempData["Message"] = $"{product.Title} was updated successfully!";
			return RedirectToAction(nameof(Index));
		}

		return View(product);
	}

	[HttpGet]
	public async Task<IActionResult> Delete(int id)
	{
		Product? product = await _context.Products.FindAsync(id);

		if (product == null)
		{
			return NotFound();
		}

		return View(product);
	}

	[ActionName(nameof(Delete))]
	[HttpPost]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		Product? product = await _context.Products.FindAsync(id);

		if (product == null)
		{
			return RedirectToAction(nameof(Index));
		}

		_context.Remove(product);
		await _context.SaveChangesAsync();

		TempData["Message"] = $"{product.Title} was successfully deleted";
		return RedirectToAction(nameof(Index));
	}
}
