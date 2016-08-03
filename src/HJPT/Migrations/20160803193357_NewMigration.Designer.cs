using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using HJPT.Model;

namespace HJPT.Migrations
{
    [DbContext(typeof(HJPTContext))]
    [Migration("20160803193357_NewMigration")]
    partial class NewMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HJPT.Model.User", b =>
                {
                    b.Property<int>("UID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("StuID");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("UID");

                    b.HasAlternateKey("Email");


                    b.HasAlternateKey("Username");

                    b.ToTable("Users");
                });
        }
    }
}
