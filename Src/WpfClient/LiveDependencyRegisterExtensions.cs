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

using CNCLib.Service.Abstraction;
using CNCLib.WpfClient.Services;

using Framework.Dependency;
using Framework.Dependency.Abstraction;
using Framework.Pattern;

namespace CNCLib.WpfClient
{
    public static class LiveDependencyRegisterExtensions
    {
        public static IDependencyContainer RegisterCNCLibWpf(this IDependencyContainer container)
        {
            container.RegisterInstance(new Global())
            .RegisterType<IJoystickService, JoystickService>()
            .RegisterType<IFactory<IMachineService>, FactoryResolve<IMachineService>>()
            .RegisterType<IFactory<ILoadOptionsService>, FactoryResolve<ILoadOptionsService>>()
            .RegisterType<IFactory<IJoystickService>, FactoryResolve<IJoystickService>>()
            .RegisterType<IFactory<IUserService>, FactoryResolve<IUserService>>()
            .RegisterTypesByName(
                n => n.EndsWith("ViewModel"),
                DependencyLivetime.Transient,
                typeof(ViewModels.MachineViewModel).Assembly,
                typeof(GCode.GUI.ViewModels.LoadOptionViewModel).Assembly);

            return container;
        }
    }
}