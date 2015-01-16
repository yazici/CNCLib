﻿////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using System.Windows;
using Proxxon.Wpf;
using Framework.Tools;
using System.Linq.Expressions;
using Proxxon.Logic;
using Proxxon.GCode;


namespace Proxxon.Wpf.ViewModels
{
    public class MachineViewModel : BaseViewModel
    {
        public MachineViewModel()
		{
            AddNewMachine = false;
		}

        public void LoadMachine(int machineID)
        {
            AddNewMachine = machineID <= 0;
            if (!AddNewMachine)
            {
                _currentMachine = ObjectConverter.NewCloneProperties<Models.Machine, Proxxon.Logic.DTO.Machine>(new MachineControler().GetMachine(machineID));
            }

            OnPropertyChanged(() => MachineName);
            OnPropertyChanged(() => ComPort);
            OnPropertyChanged(() => BaudRate);
            OnPropertyChanged(() => CommandToUpper);
            OnPropertyChanged(() => SizeX);
            OnPropertyChanged(() => SizeY);
            OnPropertyChanged(() => SizeZ);
            OnPropertyChanged(() => BufferSize);
            OnPropertyChanged(() => Default);
        }
 
        #region Properties

		Models.Machine _currentMachine = new Models.Machine();

        public string MachineName
        {
            get { return _currentMachine.Name; }
            set { SetProperty(() => _currentMachine.Name == value, () => _currentMachine.Name = value); }
        }

        public string ComPort
        {
			get { return string.IsNullOrEmpty(_currentMachine.ComPort) ? "com4" : _currentMachine.ComPort; }
            set { SetProperty(() => _currentMachine.ComPort == value, () => _currentMachine.ComPort = value); }
        }

		public int BaudRate
		{
			get { return _currentMachine.BaudRate; }
            set { SetProperty(() => _currentMachine.BaudRate == value, () => _currentMachine.BaudRate = value); }
		}

		public bool CommandToUpper
		{
			get { return _currentMachine.CommandToUpper; }
			set { SetProperty(() => _currentMachine.CommandToUpper == value, () => _currentMachine.CommandToUpper = value); }
		}

		public int BufferSize
		{
			get { return _currentMachine.BufferSize; }
			set { SetProperty(() => _currentMachine.BufferSize == value, () => _currentMachine.BufferSize = value); }
		}

		public decimal SizeX
		{
			get { return _currentMachine.SizeX; }
			set { SetProperty(() => _currentMachine.SizeX == value, () => _currentMachine.SizeX = value); }
		}
		public decimal SizeY
		{
			get { return _currentMachine.SizeY; }
			set { SetProperty(() => _currentMachine.SizeY == value, () => _currentMachine.SizeY = value); }
		}
		public decimal SizeZ
		{
			get { return _currentMachine.SizeZ; }
            set { SetProperty(() => _currentMachine.SizeZ == value, () => _currentMachine.SizeZ = value); }
		}
        public bool Default
        {
            get { return _currentMachine.Default; }
            set { SetProperty(() => _currentMachine.Default == value, () => _currentMachine.Default = value); }
        }

		#endregion

        public bool AddNewMachine { get; set; }
        
        #region Operations

		public void SaveMachine()
		{
            int id;
            if (AddNewMachine)
            {
                id = new MachineControler().Add(_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            }
            else
            {
                id = _currentMachine.MachineID;
                new MachineControler().Update(_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            }
            LoadMachine(id);
        }
		public bool CanSaveMachine()
		{
			return true;
		}

        public void DeleteMachine()
        {
            new MachineControler().Delete(_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            ;
        }
        public bool CanDeleteMachine()
        {
            return !AddNewMachine;
        }

        #endregion

        #region Commands

		public ICommand SaveMachineCommand { get { return new DelegateCommand(SaveMachine, CanSaveMachine); } }
        public ICommand DeleteMachineCommand { get { return new DelegateCommand(DeleteMachine, CanDeleteMachine); } }

        #endregion
    }
}
