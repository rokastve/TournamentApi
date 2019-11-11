﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TournamentApi.Models;

namespace TournamentAPI.Migrations
{
    [DbContext(typeof(TournamentContext))]
    [Migration("20191111021206_useradded")]
    partial class useradded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("TournamentApi.Models.Player", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InGameName");

                    b.Property<string>("Region");

                    b.Property<long?>("TeamId");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("TournamentApi.Models.Team", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Region");

                    b.Property<long?>("TournamentId");

                    b.HasKey("Id");

                    b.HasIndex("TournamentId");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("TournamentApi.Models.Tournament", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Capacity");

                    b.HasKey("Id");

                    b.ToTable("Tournament");
                });

            modelBuilder.Entity("TournamentApi.Models.Player", b =>
                {
                    b.HasOne("TournamentApi.Models.Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("TournamentApi.Models.Team", b =>
                {
                    b.HasOne("TournamentApi.Models.Tournament")
                        .WithMany("participants")
                        .HasForeignKey("TournamentId");
                });
#pragma warning restore 612, 618
        }
    }
}
