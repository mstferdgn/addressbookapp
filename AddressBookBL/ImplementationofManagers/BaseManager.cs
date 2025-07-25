using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.ResultModels;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AddressBookDL.ContextInfo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
namespace AddressBookBL.ImplementationofManagers
{
    public class BaseManager<T, TDTO, Tid> : IBaseManager<T, TDTO, Tid>
        where T : class, new()
    {

        protected AddressBookContext _context;
        private IMapper _mapper;
        private string[]? _joins;
        public BaseManager(AddressBookContext ctx, IMapper mapper)
        {
            _context = ctx;
            _mapper = mapper;
        }

        public BaseManager(AddressBookContext ctx, IMapper mapper, string[] joins)
        {
            _context = ctx;
            _mapper = mapper;
            this._joins = joins;
        }
        public IResult Add(TDTO entity)
        {
            try
            {
                Result result = new Result();

                T data = _mapper.Map<TDTO, T>(entity);
                _context.Add(data);
                if (_context.SaveChanges() > 0)
                {
                    result.IsSuccess = true;
                    result.Message = $"Ekleme başarılıdır!";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = $"EkleNEMEDİ!";
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IResult Delete(TDTO entity)
        {
            try
            {
                Result result = new Result();

                T data = _mapper.Map<TDTO, T>(entity);
                _context.Remove(data);
                if (_context.SaveChanges() > 0)
                {
                    result.IsSuccess = true;
                    result.Message = $"Silme başarılıdır!";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = $"SiliNEMEDİ!";
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IDataResult<ICollection<TDTO>> GetAll(string[]? joinTables)
        {
            try
            {
                var query = _context.Set<T>().AsQueryable();
                if (joinTables != null)
                {
                    foreach (var item in joinTables)
                    {
                        query = query.Include(item);
                    }
                }
                else if (_joins != null)
                {
                    foreach (var item in _joins)
                    {
                        query = query.Include(item);
                    }
                }

                DataResult<ICollection<TDTO>> result = new DataResult<ICollection<TDTO>>();
                result.Data = _mapper.Map<IQueryable<T>, ICollection<TDTO>>(query);
                result.IsSuccess = true;
                result.Message = $"{result.Data.Count} adet veri geldi";

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IDataResult<TDTO> GetbyCondition(Expression<Func<TDTO, bool>>? whereFilter = null, string[]? joinTables = null)
        {
            try
            {
                var query = _context.Set<T>().AsQueryable();
                if (joinTables != null)
                {
                    foreach (var item in joinTables)
                    {
                        query = query.Include(item);
                    }
                }
                else if (_joins != null)
                {
                    foreach (var item in _joins)
                    {
                        query = query.Include(item);
                    }
                }



                if (whereFilter != null)
                {
                    var filter = _mapper.Map<Expression<Func<TDTO, bool>>, Expression<Func<T, bool>>>(whereFilter);
                    query = query.Where(filter);
                }

                DataResult<TDTO> result = new DataResult<TDTO>();
                result.Data = _mapper.Map<T, TDTO>(query.FirstOrDefault());
                result.IsSuccess = true;
                result.Message = $"Veri geldi";

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IDataResult<TDTO> GetById(Tid id)
        {
            try
            {
                var entity = _context.Set<T>().Find(id);
                var data = _mapper.Map<T, TDTO>(entity);
                DataResult<TDTO> result = new DataResult<TDTO>(true, $"{id} idli data geldi", data);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IDataResult<ICollection<TDTO>> GetSomeAll(Expression<Func<TDTO, bool>>? whereFilter = null, string[]? joinTables = null)
        {
            try
            {
                var query = _context.Set<T>().AsQueryable();

                if (joinTables != null)
                {
                    foreach (var item in joinTables)
                    {
                        query = query.Include(item);
                    }
                }
                else if (_joins != null)
                {
                    foreach (var item in _joins)
                    {
                        query = query.Include(item);
                    }
                }


                if (whereFilter != null)
                {
                    var filter = _mapper.Map<Expression<Func<TDTO, bool>>, Expression<Func<T, bool>>>(whereFilter);
                    query = query.Where(filter);
                }



                DataResult<ICollection<TDTO>> result = new DataResult<ICollection<TDTO>>();
                result.Data = _mapper.Map<IQueryable<T>, ICollection<TDTO>>(query);
                result.IsSuccess = true;
                result.Message = $"{result.Data.Count} adet veri geldi";

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IResult Update(TDTO entity)
        {
            try
            {
                Result result = new Result();

                T data = _mapper.Map<TDTO, T>(entity);
                _context.Update(data);
                if (_context.SaveChanges() > 0)
                {
                    result.IsSuccess = true;
                    result.Message = $"Güncelleme başarılıdır!";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = $"GüncelNEMEDİ!";
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
