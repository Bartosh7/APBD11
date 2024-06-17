﻿// <auto-generated />
using System;
using JWT.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JWT.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240617175702_creating table")]
    partial class creatingtable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.5.24306.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("JWT.Models.AppUser", b =>
                {
                    b.Property<int>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUser"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("email");

                    b.Property<string>("PasswordHashed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("password_hashed");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("salt");

                    b.HasKey("IdUser");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("JWT.Models.LogToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("toData");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("token");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("FK_userId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LogTokens");
                });

            modelBuilder.Entity("JWT.Models.LogToken", b =>
                {
                    b.HasOne("JWT.Models.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
