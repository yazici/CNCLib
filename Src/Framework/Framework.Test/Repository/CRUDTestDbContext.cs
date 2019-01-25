﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

namespace Framework.Test.Repository
{
    using Microsoft.EntityFrameworkCore;

    using Framework.Repository.Abstraction;

    public class CRUDTestDbContext<TDbContext, TEntity, TKey, TIRepository> : GetTestDbContext<TDbContext, TEntity, TKey, TIRepository>
        where TEntity : class where TIRepository : ICRUDRepository<TEntity, TKey> where TDbContext : DbContext
    {
        public CRUDTestDbContext(TDbContext dbContext, IUnitOfWork uow, TIRepository repository) : base(dbContext, uow, repository)
        {
        }
    }
}