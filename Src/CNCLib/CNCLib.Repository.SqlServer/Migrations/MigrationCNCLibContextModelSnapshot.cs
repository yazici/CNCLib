﻿// <auto-generated />
using CNCLib.Repository.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace CNCLib.Repository.SqlServer.Migrations
{
    [DbContext(typeof(MigrationCNCLibContext))]
    partial class MigrationCNCLibContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.Configuration", b =>
                {
                    b.Property<string>("Group")
                        .HasMaxLength(256);

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<int?>("UserID");

                    b.Property<string>("Value")
                        .HasMaxLength(4000);

                    b.HasKey("Group", "Name");

                    b.HasIndex("UserID");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.Item", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int?>("UserID");

                    b.HasKey("ItemID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserID");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.ItemProperty", b =>
                {
                    b.Property<int>("ItemID");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("Value");

                    b.HasKey("ItemID", "Name");

                    b.ToTable("ItemProperty");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.Machine", b =>
                {
                    b.Property<int>("MachineID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Axis");

                    b.Property<int>("BaudRate");

                    b.Property<int>("BufferSize");

                    b.Property<string>("ComPort")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<int>("CommandSyntax");

                    b.Property<bool>("CommandToUpper");

                    b.Property<bool>("Coolant");

                    b.Property<bool>("Laser");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<bool>("NeedDtr");

                    b.Property<decimal>("ProbeDist");

                    b.Property<decimal>("ProbeDistUp");

                    b.Property<decimal>("ProbeFeed");

                    b.Property<decimal>("ProbeSizeX");

                    b.Property<decimal>("ProbeSizeY");

                    b.Property<decimal>("ProbeSizeZ");

                    b.Property<bool>("Rotate");

                    b.Property<bool>("SDSupport");

                    b.Property<decimal>("SizeA");

                    b.Property<decimal>("SizeB");

                    b.Property<decimal>("SizeC");

                    b.Property<decimal>("SizeX");

                    b.Property<decimal>("SizeY");

                    b.Property<decimal>("SizeZ");

                    b.Property<bool>("Spindle");

                    b.Property<int?>("UserID");

                    b.HasKey("MachineID");

                    b.HasIndex("UserID");

                    b.ToTable("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.MachineCommand", b =>
                {
                    b.Property<int>("MachineCommandID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("JoystickMessage")
                        .HasMaxLength(64);

                    b.Property<int>("MachineID");

                    b.Property<int?>("PosX");

                    b.Property<int?>("PosY");

                    b.HasKey("MachineCommandID");

                    b.HasIndex("MachineID");

                    b.ToTable("MachineCommand");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.MachineInitCommand", b =>
                {
                    b.Property<int>("MachineInitCommandID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int>("MachineID");

                    b.Property<int>("SeqNo");

                    b.HasKey("MachineInitCommandID");

                    b.HasIndex("MachineID");

                    b.ToTable("MachineInitCommand");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true);

                    b.Property<string>("UserPassword")
                        .HasMaxLength(255);

                    b.HasKey("UserID");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.Configuration", b =>
                {
                    b.HasOne("CNCLib.Repository.Contracts.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.Item", b =>
                {
                    b.HasOne("CNCLib.Repository.Contracts.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.ItemProperty", b =>
                {
                    b.HasOne("CNCLib.Repository.Contracts.Entities.Item", "Item")
                        .WithMany("ItemProperties")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.Machine", b =>
                {
                    b.HasOne("CNCLib.Repository.Contracts.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.MachineCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Contracts.Entities.Machine", "Machine")
                        .WithMany("MachineCommands")
                        .HasForeignKey("MachineID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CNCLib.Repository.Contracts.Entities.MachineInitCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Contracts.Entities.Machine", "Machine")
                        .WithMany("MachineInitCommands")
                        .HasForeignKey("MachineID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
