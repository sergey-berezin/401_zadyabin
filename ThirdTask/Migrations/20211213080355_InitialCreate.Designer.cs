﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ThirdTask.Database;

namespace ThirdTask.Migrations
{
    [DbContext(typeof(ImageContext))]
    [Migration("20211213080355_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("ThirdTask.Database.AnalyzedImage", b =>
                {
                    b.Property<int>("AnalyzedImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Image")
                        .HasColumnType("BLOB");

                    b.Property<string>("ImageHash")
                        .HasColumnType("TEXT");

                    b.HasKey("AnalyzedImageId");

                    b.ToTable("AnalyzedImages");
                });

            modelBuilder.Entity("ThirdTask.Database.BoundingBox", b =>
                {
                    b.Property<int>("BoundingBoxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnalyzedImageId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Confidence")
                        .HasColumnType("REAL");

                    b.Property<string>("Label")
                        .HasColumnType("TEXT");

                    b.Property<float>("x1")
                        .HasColumnType("REAL");

                    b.Property<float>("x2")
                        .HasColumnType("REAL");

                    b.Property<float>("y1")
                        .HasColumnType("REAL");

                    b.Property<float>("y2")
                        .HasColumnType("REAL");

                    b.HasKey("BoundingBoxId");

                    b.HasIndex("AnalyzedImageId");

                    b.ToTable("BoundingBox");
                });

            modelBuilder.Entity("ThirdTask.Database.BoundingBox", b =>
                {
                    b.HasOne("ThirdTask.Database.AnalyzedImage", null)
                        .WithMany("BoundingBoxes")
                        .HasForeignKey("AnalyzedImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ThirdTask.Database.AnalyzedImage", b =>
                {
                    b.Navigation("BoundingBoxes");
                });
#pragma warning restore 612, 618
        }
    }
}
