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
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools;
using Unity.Policy.Mapping;

namespace CNCLib.ServiceProxy.WebAPI
{
    public abstract class CRUDServiceBase<T,TKey> : ServiceBase
        where T : class
        where TKey : IComparable
    {
        protected abstract TKey GetKey(T value);

        public async Task<T> Get(TKey id)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Api + "/" + id);
                if (response.IsSuccessStatusCode)
                {
                    T value = await response.Content.ReadAsAsync<T>();
                    return value;
                }
            }
            return null;
        }

        public async Task<IEnumerable<T>> Get(IEnumerable<TKey> keys)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Api);
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<T> dtos = await response.Content.ReadAsAsync<IEnumerable<T>>();
                    return dtos;
                }
                return null;
            }
        }

        public async Task<TKey> Add(T value)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(Api, value);

                if (!response.IsSuccessStatusCode)
                    return default(TKey);

                return GetKey(await response.Content.ReadAsAsync<T>());
            }
        }

        public async Task<IEnumerable<TKey>> Add(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(T value)
        {
            await Delete(GetKey(value));
        }

        public async Task Delete(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(TKey key)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(Api + "/" + key);

                if (response.IsSuccessStatusCode)
                {
                    //return RedirectToAction("Index");
                }
                //return HttpNotFound();
            }
        }

        public async Task Delete(IEnumerable<TKey> keys)
        {
            throw new NotImplementedException();
        }

        public async Task Update(T value)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(Api + "/" + GetKey(value), value);
            }
        }

        public async Task Update(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

    }
}
