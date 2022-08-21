using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Caching
{
    public class ProductServiceWithCaching : IProductService
    {
        private const string CacheProductKey = "productsCashe";
        private readonly IMapper _mapper;
        private readonly IProductRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceWithCaching(IMapper mapper, IProductRepository repository, IMemoryCache memoryCahche, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _memoryCache = memoryCahche;
            _unitOfWork = unitOfWork;

            if(!_memoryCache.TryGetValue(CacheProductKey, out _))
            {
                memoryCahche.Set(CacheProductKey, _repository.GetAll().ToList());
            }
        }

        public async Task<Product> AddAsync(Product entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entity;
        }

        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();

            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_memoryCache.Get<IEnumerable<Product>>(CacheProductKey));
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _memoryCache.Get<List<Product>>(CacheProductKey)
                .FirstOrDefault(x => x.Id == id);

            if(product == null)
            {
                throw new NotFoundException($"{typeof(Product).Name}({id}) not found");
            }

            return Task.FromResult(product);
        }
            
      

        public async Task<CustomResponseDTO<List<ProductWithCategoryDTO>>> GetProductsListWithCategory()
        {
            var products = await _repository.GetProductsWithCategory();

            var productsWithCategory = _mapper.Map<List<ProductWithCategoryDTO>>(products);

            return CustomResponseDTO<List<ProductWithCategoryDTO>>.Success(200, productsWithCategory);
            
        }

        public async Task RemoveAsync(Product entity)
        {
             _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            return _memoryCache.Get<List<Product>>(CacheProductKey)
                .Where(expression.Compile())
                .AsQueryable();
        }

        public async Task CacheAllProductsAsync()
        {
            _memoryCache.Set(CacheProductKey, await _repository.GetAll().ToListAsync());
;        }
    }
}
