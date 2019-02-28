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

using System;

using Framework.Test.Dependency;
using Framework.Tools.Abstraction;

namespace Framework.Test
{
    /// <summary>
    /// Base class for *all* unit tests. 
    /// </summary>
    public class UnitTestBase
    {
        private static object _lockObject = new object();

        protected UnitTestBase()
        {
            lock (_lockObject)
            {
                if (Framework.Dependency.Dependency.IsInitialized == false)
                {
                    Framework.Dependency.Dependency.Initialize(new UnitTestDependencyProvider());
                }
            }
        }

        protected virtual void InitializeDependencies()
        {
        }

        private CurrentDateTimeMock _currentDateTime;

        protected ICurrentDateTime CurrentDateTime => CurrentDateTimeX;

        protected CurrentDateTimeMock CurrentDateTimeX => _currentDateTime ?? (_currentDateTime = new CurrentDateTimeMock());
    }
}