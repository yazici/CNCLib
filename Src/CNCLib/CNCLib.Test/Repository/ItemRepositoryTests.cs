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
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;

using FluentAssertions;

using Framework.Dependency;
using Framework.Repository;
using Framework.Test.Repository;
using Framework.Tools;

using Xunit;

namespace CNCLib.Test.Repository
{
    public class ItemRepositoryTests : RepositoryTests<CNCLibContext, Item, int, IItemRepository>
    {
        #region crt and overrides

        protected override GetTestDbContext<CNCLibContext, Item, int, IItemRepository> CreateTestDbContext()
        {
            var context = new CNCLibContext();
            var uow     = new UnitOfWork<CNCLibContext>(context);
            var rep     = new ItemRepository(context, UserContext);
            return new GetTestDbContext<CNCLibContext, Item, int, IItemRepository>(context, uow, rep);
        }

        protected override int GetEntityKey(Item entity)
        {
            return entity.ItemId;
        }

        protected override Item SetEntityKey(Item entity, int key)
        {
            entity.ItemId = key;
            return entity;
        }

        protected override bool CompareEntity(Item entity1, Item entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserId)
            //);
            return CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"ItemId" });
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = await GetAll();
            entities.Count().Should().BeGreaterThan(1);
            entities.Count(i => i.Name == "laser cut 160mg paper").Should().Be(1);
            entities.Count(i => i.Name == "laser cut hole 130mg black").Should().Be(1);
        }

        [Fact]
        public async Task GetOKTest()
        {
            var entity = await GetOK(1);
            entity.ItemId.Should().Be(1);
        }

        [Fact]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(2);
            entity.ItemId.Should().Be(2);
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await GetNotExist(2342341);
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(() => CreateItem(@"AddUpdateDeleteTest"), (entity) => entity.ClassName = "DummyClassUpdate");
        }

        [Fact]
        public async Task AddUpdateDeleteWithItemPropertiesTest()
        {
            await AddUpdateDelete(() => AddItemProperties(CreateItem(@"AddUpdateDeleteWithItemPropertiesTest")), (entity) =>
            {
                entity.ClassName = "DummyClassUpdate";
                entity.ItemProperties.Remove(entity.ItemProperties.First());
                entity.ItemProperties.Add(new ItemProperty()
                {
                    Name  = @"NewItemProperty",
                    Value = @"Hallo"
                });
            });
        }

        [Fact]
        public async Task AddRollbackTest()
        {
            await AddRollBack(() => CreateItem(@"AddRollbackTest"));
        }

        #endregion

        private static Item CreateItem(string name)
        {
            var e = new Item
            {
                Name           = name,
                ClassName      = "Dummy",
                ItemProperties = new List<ItemProperty>()
            };
            return e;
        }

        private static Item AddItemProperties(Item e)
        {
            e.ItemProperties = new List<ItemProperty>
            {
                new ItemProperty { Name = "Name1", Value = "Test1", Item = e },
                new ItemProperty { Name = "Name2", Value = "Test2", Item = e },
                new ItemProperty { Name = "Name3", Item  = e }
            };
            return e;
        }
    }
}