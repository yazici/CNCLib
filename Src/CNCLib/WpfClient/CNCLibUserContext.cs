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
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;
using CNCLib.Shared;

using Framework.Dependency;

namespace CNCLib.WpfClient
{
    public class CNCLibUserContext : ICNCLibUserContextRW
    {
        public CNCLibUserContext()
        {
            //string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            UserName = Environment.UserName;
        }

        public string UserName { get; private set; }

        public int? UserId { get; private set; }

        public async Task InitUserContext()
        {
            try
            {
                using (var userService = Dependency.Resolve<IUserService>())
                {
                    var user = await userService.GetByName(UserName);
                    if (user == null)
                    {
                        user = new User()
                        {
                            UserName = this.UserName
                        };
                        user.UserId = await userService.Add(user);
                    }

                    UserId = user.UserId;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}