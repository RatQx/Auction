﻿// <auto-generated />
using System;
using Aukcionas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Aukcionas.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240217182104_updatetypesv2")]
    partial class updatetypesv2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Aukcionas.Models.Auction", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("SavedFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SavedUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("auction_biders_list")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("auction_end_time")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("auction_ended")
                        .HasColumnType("bit");

                    b.Property<int?>("auction_likes")
                        .HasColumnType("int");

                    b.Property<DateTime>("auction_start_time")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("auction_stopped")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("auction_time")
                        .HasColumnType("datetime2");

                    b.Property<string>("auction_winner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("auction_won")
                        .HasColumnType("bit");

                    b.Property<int>("bid_ammount")
                        .HasColumnType("int");

                    b.Property<string>("bidding_amount_history")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("bidding_times_history")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("buy_now_price")
                        .HasColumnType("float");

                    b.Property<string>("category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("city")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("condition")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("country")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("item_build_year")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<double>("item_mass")
                        .HasColumnType("float");

                    b.Property<string>("material")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("min_buy_price")
                        .HasColumnType("float");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<double>("starting_price")
                        .HasColumnType("float");

                    b.Property<string>("username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Auctions");
                });
#pragma warning restore 612, 618
        }
    }
}
