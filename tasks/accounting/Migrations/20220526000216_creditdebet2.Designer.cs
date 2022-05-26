﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using accounting.Context;

#nullable disable

namespace accounting.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20220526000216_creditdebet2")]
    partial class creditdebet2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("accounting.Models.Account", b =>
                {
                    b.Property<Guid>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Balance")
                        .HasColumnType("integer");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("accounting.Models.TrackerTask", b =>
                {
                    b.Property<Guid>("PublicId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<int>("AssignCost")
                        .HasColumnType("integer");

                    b.Property<int>("CompleteCost")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("JiraId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PublicId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("accounting.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Credit")
                        .HasColumnType("integer");

                    b.Property<int>("Debit")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PublicId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("TaskId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
