﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Senparc.Xncf.DynamicData.Models;

#nullable disable

namespace Senparc.Xncf.DynamicData.Domain.Migrations.SqlServer
{
    [DbContext(typeof(DynamicDataSenparcEntities_SqlServer))]
    partial class DynamicDataSenparcEntities_SqlServerModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Senparc.Xncf.DynamicData.Color", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("AdditionNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AdminRemark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("Blue")
                        .HasColumnType("int");

                    b.Property<bool>("Flag")
                        .HasColumnType("bit");

                    b.Property<int>("Green")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Red")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Senparc_DynamicData_Color");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.ColumnMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("AdminRemark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("ColumnName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ColumnType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DefaultValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Flag")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNullable")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("TableMetadataId")
                        .HasColumnType("int");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TableMetadataId");

                    b.ToTable("Senparc_DynamicData_ColumnMetadata");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.TableData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("AdminRemark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("CellValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ColumnMetadataId")
                        .HasColumnType("int");

                    b.Property<bool>("Flag")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("TableId")
                        .HasColumnType("int");

                    b.Property<int?>("TableMetadataId")
                        .HasColumnType("int");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ColumnMetadataId");

                    b.HasIndex("TableMetadataId");

                    b.ToTable("Senparc_DynamicData_TableData");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.TableMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("AdminRemark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Flag")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remark")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Senparc_DynamicData_TableMetadata");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.ColumnMetadata", b =>
                {
                    b.HasOne("Senparc.Xncf.DynamicData.TableMetadata", "TableMetadata")
                        .WithMany("ColumnMetadatas")
                        .HasForeignKey("TableMetadataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TableMetadata");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.TableData", b =>
                {
                    b.HasOne("Senparc.Xncf.DynamicData.ColumnMetadata", "ColumnMetadata")
                        .WithMany("TableDatas")
                        .HasForeignKey("ColumnMetadataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Senparc.Xncf.DynamicData.TableMetadata", null)
                        .WithMany("TableDatas")
                        .HasForeignKey("TableMetadataId");

                    b.Navigation("ColumnMetadata");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.ColumnMetadata", b =>
                {
                    b.Navigation("TableDatas");
                });

            modelBuilder.Entity("Senparc.Xncf.DynamicData.TableMetadata", b =>
                {
                    b.Navigation("ColumnMetadatas");

                    b.Navigation("TableDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
