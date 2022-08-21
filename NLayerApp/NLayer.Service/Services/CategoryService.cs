using AutoMapper;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Services
{
    public class CategoryService : Service<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IGenericRepository<Category> repository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository, IMapper mapper) : base(repository, unitOfWork)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }


        public async Task<CustomResponseDTO<CategoryWithProductsDTO>> GetSingleCategoryByIdWithProductsAsync(int id)
        {
            var category = await _categoryRepository.GetSingleCategoryByIdWithProductsAsync(id);
                                                      
            var categoryDTO = _mapper.Map<CategoryWithProductsDTO>(category);

            return CustomResponseDTO<CategoryWithProductsDTO>.Success(200, categoryDTO);
        }
    }
}
