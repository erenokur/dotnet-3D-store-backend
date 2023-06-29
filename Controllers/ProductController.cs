namespace dotnet_todo_backend.Controllers;


using System.Security.Claims;
using dotnet_3D_store_backend.Contexts;
using dotnet_3D_store_backend.interfaces;
using dotnet_3D_store_backend.Models;
using dotnet_3D_store_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using dotnet_3D_store_backend.Enumerators;

[ApiController]
[Route("[controller]")]

public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private ProductService _productService;
    private AppSettings _appSettings;

    public ProductController(ILogger<ProductController> logger, IOptions<AppSettings> appSettings, DatabaseContext dbContext)
    {
        _appSettings = appSettings.Value;
        _productService = new ProductService(dbContext, _appSettings);
        _logger = logger;

    }

    [Authorize(Policy = "SellerPolicy")]
    [HttpPost("addProduct")]
    public IActionResult AddProduct(AddProductRequest model)
    {
        var userId = Convert.ToInt32(HttpContext.User.FindFirst("id")?.Value);
        if (userId == 0)
        {
            return BadRequest();
        }
        model.SellerUserId = userId;
        var result = _productService.AddProduct(model);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }

    [Authorize(Policy = "SellerPolicy")]
    [HttpPost("removeProduct")]
    public IActionResult RemoveProduct(RemoveProductRequest model)
    {
        var result = _productService.DeleteProduct(model);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpGet("getSuggestedProducts")]
    public IActionResult GetSuggestedProducts(SuggestedProductsRequest model)
    {
        var products = _productService.GetSuggestedProductsAsync(model);
        return Ok(products);
    }

    [HttpGet("GetProductsByCategory")]
    public IActionResult GetProductsByCategory(ProductsByCategoryRequest model)
    {
        var products = _productService.GetProductsByCategory(model);
        return Ok(products);
    }

    [HttpGet("GetProductsBySearch")]
    public IActionResult GetProductsBySearch(ProductsBySearchRequest model)
    {
        var products = _productService.GetProductsBySearchAsync(model);
        return Ok(products);
    }
}