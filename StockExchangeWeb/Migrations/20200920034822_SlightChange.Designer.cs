﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StockExchangeWeb.Data;

namespace StockExchangeWeb.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20200920034822_SlightChange")]
    partial class SlightChange
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0-rc.1.20451.13");

            modelBuilder.Entity("StockExchangeWeb.Models.OrderTrace.OrderTrace", b =>
                {
                    b.Property<string>("TraceId")
                        .HasColumnType("text");

                    b.Property<string>("OrderId")
                        .HasColumnType("text");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Timestamp")
                        .HasColumnType("text");

                    b.HasKey("TraceId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderTrace");
                });

            modelBuilder.Entity("StockExchangeWeb.Models.Orders.Order", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<decimal>("AskPrice")
                        .HasColumnType("numeric");

                    b.Property<bool>("BuyOrder")
                        .HasColumnType("boolean");

                    b.Property<decimal>("ExecutedPrice")
                        .HasColumnType("numeric");

                    b.Property<string>("OrderDeletionTime")
                        .HasColumnType("text");

                    b.Property<string>("OrderExecutionTime")
                        .HasColumnType("text");

                    b.Property<string>("OrderPutTime")
                        .HasColumnType("text");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("integer");

                    b.Property<int>("OrderType")
                        .HasColumnType("integer");

                    b.Property<string>("Ticker")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OrderStatus");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("StockExchangeWeb.Models.OrderTrace.OrderTrace", b =>
                {
                    b.HasOne("StockExchangeWeb.Models.Orders.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId");

                    b.Navigation("Order");
                });
#pragma warning restore 612, 618
        }
    }
}
