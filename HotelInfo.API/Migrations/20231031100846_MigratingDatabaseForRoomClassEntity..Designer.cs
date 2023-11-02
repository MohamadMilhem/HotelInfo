﻿// <auto-generated />
using System;
using HotelInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelInfo.API.Migrations
{
    [DbContext(typeof(HotelInfoContext))]
    [Migration("20231031100846_MigratingDatabaseForRoomClassEntity.")]
    partial class MigratingDatabaseForRoomClassEntity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HotelHotelAmenity", b =>
                {
                    b.Property<int>("HotelAmenitiesId")
                        .HasColumnType("int");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.HasKey("HotelAmenitiesId", "HotelId");

                    b.HasIndex("HotelId");

                    b.ToTable("HotelHotelAmenity");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ThumbnailImageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Hotel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("HotelType")
                        .HasColumnType("int");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("StarRating")
                        .HasColumnType("int");

                    b.Property<int>("ThumbnailImageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Hotels");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.HotelAmenity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("HotelAmenities");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CityId")
                        .HasColumnType("int");

                    b.Property<int?>("HotelId")
                        .HasColumnType("int");

                    b.Property<int?>("RoomClassId")
                        .HasColumnType("int");

                    b.Property<int?>("RoomId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("HotelId");

                    b.HasIndex("RoomClassId");

                    b.HasIndex("RoomId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<int?>("RoomClassId")
                        .HasColumnType("int");

                    b.Property<string>("RoomNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("HotelId");

                    b.HasIndex("RoomClassId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.RoomAmenity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("RoomAmenities");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.RoomClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("StandardCost")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("RoomClasses");
                });

            modelBuilder.Entity("RoomAmenityRoomClass", b =>
                {
                    b.Property<int>("RoomAmenitiesId")
                        .HasColumnType("int");

                    b.Property<int>("RoomClassId")
                        .HasColumnType("int");

                    b.HasKey("RoomAmenitiesId", "RoomClassId");

                    b.HasIndex("RoomClassId");

                    b.ToTable("RoomAmenityRoomClass");
                });

            modelBuilder.Entity("RoomRoomAmenity", b =>
                {
                    b.Property<int>("RoomAmenitiesId")
                        .HasColumnType("int");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("RoomAmenitiesId", "RoomId");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomRoomAmenity");
                });

            modelBuilder.Entity("HotelHotelAmenity", b =>
                {
                    b.HasOne("HotelInfo.API.Entites.HotelAmenity", null)
                        .WithMany()
                        .HasForeignKey("HotelAmenitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelInfo.API.Entites.Hotel", null)
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Hotel", b =>
                {
                    b.HasOne("HotelInfo.API.Entites.City", "City")
                        .WithMany("Hotels")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Photo", b =>
                {
                    b.HasOne("HotelInfo.API.Entites.City", "City")
                        .WithMany("Photos")
                        .HasForeignKey("CityId");

                    b.HasOne("HotelInfo.API.Entites.Hotel", "Hotel")
                        .WithMany("Photos")
                        .HasForeignKey("HotelId");

                    b.HasOne("HotelInfo.API.Entites.RoomClass", "RoomClass")
                        .WithMany("Photos")
                        .HasForeignKey("RoomClassId");

                    b.HasOne("HotelInfo.API.Entites.Room", "Room")
                        .WithMany("Photos")
                        .HasForeignKey("RoomId");

                    b.Navigation("City");

                    b.Navigation("Hotel");

                    b.Navigation("Room");

                    b.Navigation("RoomClass");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Room", b =>
                {
                    b.HasOne("HotelInfo.API.Entites.Hotel", "Hotel")
                        .WithMany("Rooms")
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelInfo.API.Entites.RoomClass", "RoomClass")
                        .WithMany("Rooms")
                        .HasForeignKey("RoomClassId");

                    b.Navigation("Hotel");

                    b.Navigation("RoomClass");
                });

            modelBuilder.Entity("RoomAmenityRoomClass", b =>
                {
                    b.HasOne("HotelInfo.API.Entites.RoomAmenity", null)
                        .WithMany()
                        .HasForeignKey("RoomAmenitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelInfo.API.Entites.RoomClass", null)
                        .WithMany()
                        .HasForeignKey("RoomClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RoomRoomAmenity", b =>
                {
                    b.HasOne("HotelInfo.API.Entites.RoomAmenity", null)
                        .WithMany()
                        .HasForeignKey("RoomAmenitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelInfo.API.Entites.Room", null)
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HotelInfo.API.Entites.City", b =>
                {
                    b.Navigation("Hotels");

                    b.Navigation("Photos");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Hotel", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.Room", b =>
                {
                    b.Navigation("Photos");
                });

            modelBuilder.Entity("HotelInfo.API.Entites.RoomClass", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
