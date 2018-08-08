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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Contracts.Repository
{
    public enum EntityState
    {
        Detached,
        Unchanged,
        Deleted,
        Modified,
        Added,
    }

    public interface ICRUDRepository<TEntry, TKey>: IGetRepository<TEntry,TKey> where TEntry : class
    {
        Task Update(TKey key, TEntry values);               // shortcut to GetTracking and SetValueGraph

        void Add(TEntry entity);
        void AddRange(IEnumerable<TEntry> entities);
        void Delete(TEntry entity);
        void DeleteRange(IEnumerable<TEntry> entities);
        void SetValue(TEntry trackingentity, TEntry values);
        void SetValueGraph(TEntry trackingentity, TEntry values);

        void SetState(TEntry entity, EntityState state);
    }
}