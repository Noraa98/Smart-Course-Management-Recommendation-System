using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Classes
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<T> _repository;

        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Repository<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }

       
    }
}