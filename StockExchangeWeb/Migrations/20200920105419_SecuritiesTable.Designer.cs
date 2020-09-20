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
    [Migration("20200920105419_SecuritiesTable")]
    partial class SecuritiesTable
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

                    b.Property<bool>("LimitOrder")
                        .HasColumnType("boolean");

                    b.Property<string>("OrderDeletionTime")
                        .HasColumnType("text");

                    b.Property<string>("OrderExecutionTime")
                        .HasColumnType("text");

                    b.Property<string>("OrderPutTime")
                        .HasColumnType("text");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("integer");

                    b.Property<int>("OrderTimeInForce")
                        .HasColumnType("integer");

                    b.Property<string>("Ticker")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OrderStatus");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("StockExchangeWeb.Models.TradableSecurity", b =>
                {
                    b.Property<string>("Ticker")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("OutstandingAmount")
                        .HasColumnType("bigint");

                    b.Property<int>("SecurityType")
                        .HasColumnType("integer");

                    b.HasKey("Ticker");

                    b.ToTable("Security");
                });
#pragma warning restore 612, 618
        }
    }
}
