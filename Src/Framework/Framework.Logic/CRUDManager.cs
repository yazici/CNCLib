﻿////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Framework.Contracts.Repository;

namespace Framework.Logic
{
    public abstract class CRUDManager<T, TKey, TEntity> : GetManager<T, TKey, TEntity> where T : class where TEntity : class
    {
        private readonly IMapper                        _mapper;
        private readonly ICRUDRepository<TEntity, TKey> _repository;
        private readonly IUnitOfWork                    _unitOfWork;

        protected CRUDManager(IUnitOfWork unitOfWork, ICRUDRepository<TEntity, TKey> repository, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
        }

        public async Task<TKey> Add(T value)
        {
            return (await Add(new List<T>() { value })).First();
        }

        public async Task<IEnumerable<TKey>> Add(IEnumerable<T> values)
        {
            var entities = _mapper.Map<IEnumerable<T>, IEnumerable<TEntity>>(values);
            using (var trans = _unitOfWork.BeginTransaction())
            {
                _repository.AddRange(entities);
                await trans.CommitTransactionAsync();
                return entities.Select(GetKey);
            }
        }

        public async Task Delete(T value)
        {
            await Delete(new[] { value });
        }

        public async Task Delete(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var entities = _mapper.Map<IEnumerable<T>, IEnumerable<TEntity>>(values);
                _repository.DeleteRange(entities);
                await trans.CommitTransactionAsync();
            }
        }

        public async Task Delete(TKey key)
        {
            await Delete(new[] { key });
        }

        public async Task Delete(IEnumerable<TKey> keys)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var entities = await _repository.GetTracking(keys);
                _repository.DeleteRange(entities);
                await trans.CommitTransactionAsync();
            }
        }

        public async Task Update(T value)
        {
            await Update(new[] { value });
        }

        public async Task Update(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var entities     = _mapper.Map<IEnumerable<T>, IEnumerable<TEntity>>(values);
                var entitiesInDb = await _repository.GetTracking(entities.Select(GetKey));

                var mergeJoin = entitiesInDb.Join(entities, GetKey, GetKey, (EntityInDb, Entity) => new { EntityInDb, Entity });

                if (entities.Count() != entitiesInDb.Count() || entities.Count() != mergeJoin.Count())
                {
                    throw new DBConcurrencyException();
                }

                foreach (var merged in mergeJoin)
                {
                    _repository.SetValueGraph(merged.EntityInDb, merged.Entity);
                }

                await trans.CommitTransactionAsync();
            }
        }
    }
}