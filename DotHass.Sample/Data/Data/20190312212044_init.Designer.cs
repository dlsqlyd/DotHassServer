﻿// <auto-generated />
using DotHass.Sample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DotHass.Sample.Data.Data
{
    [DbContext(typeof(GameDataDbContext))]
    [Migration("20190312212044_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DotHass.Sample.Model.Data.GameRole", b =>
                {
                    b.Property<long>("Id");

                    b.Property<int>("Experience");

                    b.Property<int>("Gold");

                    b.Property<string>("HeadId");

                    b.Property<int>("Money");

                    b.Property<string>("RoleName");

                    b.HasKey("Id");

                    b.ToTable("GameRole");
                });
#pragma warning restore 612, 618
        }
    }
}
