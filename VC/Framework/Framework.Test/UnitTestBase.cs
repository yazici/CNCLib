﻿////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Tools;
using Framework.Test.Dependency;
using Framework.Tools.Dependency;

namespace Framework.Test
{
    /// <summary>
    /// Base class for *all* unit tests. 
    /// </summary>
    public abstract class UnitTestBase
    {
        private static bool _globalInitialisationRun;

        [TestInitialize]
        public void InitializeTest()
        {
            if (!_globalInitialisationRun)
            {
                Framework.Tools.Dependency.Dependency.Initialize(new UnitTestDependencyProvider());
                _globalInitialisationRun = true;
            }
            else
            {
                ReInitializeCoreDependencies();
            }
        }
        /// <summary>
        /// Resets the dependency container.
        /// </summary>
        private static void ReInitializeCoreDependencies()
        {
            Framework.Tools.Dependency.Dependency.Container.ResetContainer();
//            Framework.Tools.Dependency.Dependency.Container
//                .RegisterType<IUnitOfWorkWrapper, UnityUnitOfWorkWrapper>()
//                .RegisterType<IDataAccessComponentFactory, UnityDataAccessComponentFactory>();
        }
    }
}