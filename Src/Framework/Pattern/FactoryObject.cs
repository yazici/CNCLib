﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace Framework.Pattern
{
    using System;

    public sealed class ScopeDispose<T> : IScope<T>, IDisposable where T : class, IDisposable
    {
        private readonly T _instance;

        private bool _isDisposed;

        public ScopeDispose(T instance)
        {
            _instance = instance;
        }

        public T Instance
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("this", "Bad person.");
                }

                return _instance;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _instance.Dispose();
        }
    }

    public class FactoryNew<T> : IFactory<T> where T : class, IDisposable, new()
    {
        public FactoryNew()
        {
        }

        IScope<T> IFactory<T>.Create()
        {
            return new ScopeDispose<T>(new T());
        }
    }

    public class FactoryCreate<T> : IFactory<T> where T : class, IDisposable
    {
        public FactoryCreate(Func<T> createObjectFunc)
        {
            _createObjectFunc = createObjectFunc;
        }

        private Func<T> _createObjectFunc;

        IScope<T> IFactory<T>.Create()
        {
            return new ScopeDispose<T>(_createObjectFunc());
        }
    }
}