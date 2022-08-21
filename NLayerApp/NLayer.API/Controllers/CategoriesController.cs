using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.API.Filters;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController( ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{cetagoryId}")]
        public async Task<IActionResult> GetSingleCategoryByIdWithProductsAsync(int cetagoryId)
        {
            return CreateActionResult(await _categoryService.GetSingleCategoryByIdWithProductsAsync(cetagoryId));
        }
    }
}
