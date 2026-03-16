using System.ComponentModel.DataAnnotations;

namespace eCommerce.Models;

public class Product
{
	/// <summary>
	/// The unique identifier for the product.
	/// </summary>
	[Key]
	public int ProductId { get; set; }

	/// <summary>
	/// The user facing title of the product
	/// </summary>
	[StringLength(50, ErrorMessage = "Titles cannot be more than 50 characters")]
	public required string Title { get; set; }

	/// <summary>
	/// The current sales price of the product
	/// </summary>
	[Range(0, 10000)]
	[DataType(DataType.Currency)]
	public decimal Price { get; set; }
}

/// <summary>
/// Data used by the product list page.
/// </summary>
public class ProductListViewModel
{
	/// <summary>
	/// Products shown on the current page.
	/// </summary>
	public required IEnumerable<Product> Products { get; set; }

	/// <summary>
	/// Current page number.
	/// </summary>
	public int CurrentPage { get; set; }

	/// <summary>
	/// Total number of pages.
	/// </summary>
	public int TotalPages { get; set; }

	/// <summary>
	/// Number of items per page.
	/// </summary>
	public int PageSize { get; set; }

	/// <summary>
	/// Total number of items 
	/// </summary>
	public int TotalItems { get; set; }

	// Search / filter criteria
	/// <summary>
	/// Title text entered in search.
	/// </summary>
	public string? ProductTitleSearch { get; internal set; }

	/// <summary>
	/// Minimum price filter.
	/// </summary>
	public decimal? MinPrice { get; internal set; }

	/// <summary>
	/// Maximum price filter.
	/// </summary>
	public decimal? MaxPrice { get; internal set; }
}