// <auto-generated />
using System;
using EVoucherAndStoreAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EVoucherAndStoreAPI.Migrations
{
    [DbContext(typeof(MySqlContext))]
    [Migration("20220910115406_initial8")]
    partial class initial8
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.17");

            modelBuilder.Entity("EVoucherAndStoreAPI.DataAccess.Models.EVouchers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("Amount")
                        .HasColumnType("double");

                    b.Property<string>("AvailablePaymentMethods")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("DiscountPaymentMethodId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("GiftPerUserLimit")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("MaxVoucherLimit")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("DiscountPaymentMethodId");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("EVouchers");
                });

            modelBuilder.Entity("EVoucherAndStoreAPI.DataAccess.Models.PaymenMethods", b =>
                {
                    b.Property<int>("PaymentMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("PaymentMethodName")
                        .HasColumnType("varchar(255)");

                    b.HasKey("PaymentMethodId");

                    b.HasIndex("PaymentMethodName")
                        .IsUnique();

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("EVoucherAndStoreAPI.DataAccess.Models.EVouchers", b =>
                {
                    b.HasOne("EVoucherAndStoreAPI.DataAccess.Models.PaymenMethods", "Paymentmethods")
                        .WithMany("EVouchers")
                        .HasForeignKey("DiscountPaymentMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Paymentmethods");
                });

            modelBuilder.Entity("EVoucherAndStoreAPI.DataAccess.Models.PaymenMethods", b =>
                {
                    b.Navigation("EVouchers");
                });
#pragma warning restore 612, 618
        }
    }
}
