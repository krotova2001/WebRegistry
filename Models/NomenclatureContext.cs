using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace WebRegistry.Models;

public partial class NomenclatureContext : DbContext
{
    public NomenclatureContext()
    {
    }

    public NomenclatureContext(DbContextOptions<NomenclatureContext> options)
        : base(options)
    {
        
    }
    public virtual DbSet<Izdelie> Izdelies { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Detail> Details { get; set; }
    public virtual DbSet<ExtCompInIzdelie> Extcomponents { get; set; }
    public virtual DbSet<User_roles> User_roles { get; set; }
    public virtual DbSet<Roles> Roles { get; set; }
    public virtual DbSet<Systema> Systemas { get; set; }
    public virtual DbSet<IzdelieSystema> IzdelieSystemas { get; set; }
    public virtual DbSet<ExternalComponent> ExternalComponents { get; set; }
    public virtual DbSet<RedBook> RedBookArchive { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<UserAction> UserActions { get; set; }
    public virtual DbSet<Status> Statuses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PRIMARY");
            entity.ToTable("category", tb => tb.HasComment("Категории изделий"));
            entity.HasIndex(e => e.IdCategory, "category_id_UNIQUE").IsUnique();
            entity.Property(e => e.Description).HasMaxLength(400);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Detail>(entity =>
        {
            entity.HasKey(e => e.IdLink).HasName("PRIMARY");

            entity.ToTable("details");

            entity.HasIndex(e => e.IdParent, "Common_link_idx");

            entity.HasIndex(e => e.IdPart, "Part_link_idx");

            entity.Property(e => e.Count).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(100);

            entity.HasOne(d => d.IdParentNavigation).WithMany(p => p.DetailIdParentNavigations)
                .HasForeignKey(d => d.IdParent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Common_link");

            entity.HasOne(d => d.IdPartNavigation).WithMany(p => p.DetailIdPartNavigations)
                .HasForeignKey(d => d.IdPart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Part_link");
        });

        modelBuilder.Entity<ExtCompInIzdelie>(entity =>
        {
            entity.HasKey(e => e.IdExtComponent).HasName("PRIMARY");

            entity.ToTable("extcompinizdelie");

            entity.HasIndex(e => e.IzdelieId, "Izdelie-ExtComponent_idx");
            entity.HasIndex(e => e.IdExtComponent, "ExtCompLink_idx");

            entity.Property(e => e.Count).HasMaxLength(50);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasComment("Наименование")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Note)
                .HasComment("Примечание")
                .HasColumnType("text")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ParentShifr)
                .HasMaxLength(100)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Shifr)
                .HasMaxLength(100)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Weight)
                .HasMaxLength(100)
                .HasComment("Масса 1 п.м., кг");

            entity.Property(e => e.IzdelieId);
            entity.Property(e => e.ExtCompId);

            entity.HasOne(d => d.IzdelieIdNavigation).WithMany(p => p.ExtcomponentsNavigations)
                .HasForeignKey(d => d.IzdelieId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ExtCompLink");

            entity.HasOne(e => e.ExternalComponentIdNavigation).WithMany(ex => ex.IzdelieNavigation)
            .HasForeignKey(e => e.ExtCompId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("IzdelieLink");
            
        });

        modelBuilder.Entity<Izdelie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("izdelie")
                .UseCollation("utf8mb4_ru_0900_as_cs");

            entity.HasIndex(e => e.Id, "Articul_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdCategory, "Izdelie-Category_idx");

            entity.Property(e => e.Id).HasComment("Артикул");
            entity.Property(e => e.Articul)
                .HasMaxLength(45)
                .HasComment("Артикул 1С");
            entity.Property(e => e.CircleDiametr)
                .HasMaxLength(30)
                .HasComment("Диаметр описанной окружности, мм");
            entity.Property(e => e.ContainExtComponent)
                .HasDefaultValueSql("'0'")
                .HasComment("Содержит ли покупные изделия");
            entity.Property(e => e.ContainDetails)
               .HasDefaultValueSql("'0'")
               .HasComment("Содержит ли составные изделия");
            entity.Property(e => e.CustomerShifr)
                .HasMaxLength(100)
                .HasComment("Шифр заказчика")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.DifficultyGroup)
                .HasMaxLength(100)
                .HasComment("Группа сложности")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.FilePath)
                .HasMaxLength(250)
                .HasComment("Абсолютный сетевой путь к файлу чертежа")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Gost)
                .HasMaxLength(100)
                .HasComment("ГОСТ")
                .HasColumnName("GOST")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.IdCategory).HasComment("ИД Категории");
            entity.Property(e => e.IsArchived).HasDefaultValueSql("'0'");
            entity.Property(e => e.IsReserved).HasDefaultValueSql("'0'");
            entity.Property(e => e.JX)
                .HasMaxLength(30)
                .HasColumnName("J(x)");
            entity.Property(e => e.JY)
                .HasMaxLength(30)
                .HasColumnName("J(y)");
            entity.Property(e => e.Master)
                .HasMaxLength(100)
                .HasComment("Мастер")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Nkontr)
                .HasMaxLength(100)
                .HasColumnName("NKontr")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.Perimeter)
                .HasMaxLength(30)
                .HasComment("Периметр внешний, мм");
            entity.Property(e => e.Prov)
                .HasMaxLength(100)
                .HasComment("Провер.")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Razrab)
                .HasMaxLength(100)
                .HasComment("Разраб.")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Shifr)
                .HasMaxLength(100)
                .HasComment("Шифр")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Square)
                .HasMaxLength(30)
                .HasComment("Площадь сечения, см2");
            entity.Property(e => e.Tkontr)
                .HasMaxLength(100)
                .HasColumnName("TKontr")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.Utverd)
                .HasMaxLength(100)
                .HasComment("Утвер.")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(e => e.WX)
                .HasMaxLength(30)
                .HasColumnName("W(x)");
            entity.Property(e => e.WY)
                .HasMaxLength(30)
                .HasColumnName("W(y)");
            entity.Property(e => e.Weight)
                .HasMaxLength(30)
                .HasComment("Масса 1 м длины, кг (Масса 1 п.м. общая)");
            entity.Property(e => e.WeightAl)
                .HasMaxLength(100)
                .HasDefaultValueSql("'0'")
                .HasComment("Масса AL 1 п.м., кг")
                .HasColumnName("WeightAL");
            entity.Property(e => e.CustomerName).HasMaxLength(100)
             .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Izdelies)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Izdelie-Category");
        });

        modelBuilder.Entity<User_roles>(entity =>
        {
            entity.HasKey(e => e.idUserRole).HasName("PRIMARY");
            entity.ToTable("user_roles");
            entity.HasIndex(e => e.idUserRole, "user_role_link_idx").IsUnique();
            entity.Property(e => e.Login).HasMaxLength(200);
            entity.Property(e => e.roleId);
            entity.Property(e => e.Fio);

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.User_Roles)
              .HasForeignKey(d => d.roleId)
              .OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("user_role_link");

        });
        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.iduser_roles).HasName("PRIMARY");
            entity.ToTable("roles");
            entity.Property(e => e.role).HasMaxLength(45);
            entity.Property(e => e.description).HasMaxLength(200);
        });

        modelBuilder.Entity<Systema>(entity =>
        {
            entity.HasKey(e => e.SystemId).HasName("PRIMARY");
            entity.ToTable("system");
            entity.Property(e=>e.SystemName).HasMaxLength(255);
            entity.Property(e => e.Fullname);
            entity.Property(e => e.Descrition).HasMaxLength(255);

        });

        modelBuilder.Entity<IzdelieSystema>(entity =>
        {
            entity.HasKey(e => new { e.systemId, e.izdelieId }).HasName("PRIMARY");
            entity.ToTable("izdeliesystema");
            entity.HasIndex(e => e.systemId, "sysId_idx");
            entity.HasIndex(e => e.izdelieId, "izdId_idx");
            entity.Property(e => e.systemId);
            entity.Property(e=>e.izdelieId);

            entity.HasOne(d => d.IdIzdelieNavigation).WithMany(i => i.SystemasNavigation)
            .HasForeignKey(e => e.izdelieId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("izdId");

            entity.HasOne(i => i.IdSystemaNavigation).WithMany(s => s.IzdelieNavigaion)
            .HasForeignKey(k => k.systemId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("sysId");

        });

        modelBuilder.Entity<ExternalComponent>(entity =>
        {
            entity.HasKey(e=>e.idExternalComponents).HasName("PRIMARY");
            entity.ToTable("externalcomponents");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Shifr).HasMaxLength(200);
            entity.Property(e => e.Weight);
            entity.Property(e => e.Note).HasMaxLength(400);
           
        });

        modelBuilder.Entity<RedBook>(entity =>
        {
            entity.HasKey(e => e.IdRedBook).HasName("PRIMARY");
            entity.ToTable("redbook");
            entity.Property(e => e.ShifrRedBook).HasMaxLength(200);
            entity.Property(e => e.Inventory).HasMaxLength(200);
            entity.Property(e => e.userPublishingDate);
            entity.Property(e => e.publishingDate);
            entity.Property(e => e.Note).HasMaxLength(450);
            entity.Property(e => e.IsArchived);
            entity.Property(e=>e.AuthorLogin);
            entity.Property(e => e.StatusId);
            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Books)
                .HasForeignKey(k => k.StatusId)
                .HasConstraintName("RedBook-Status")
                .OnDelete(DeleteBehavior.SetNull);

        });

        modelBuilder.Entity<Customer>(entity => {
            entity.HasKey(e => e.Idcustomer).HasName("PRIMARY");
            entity.ToTable("customer");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Note).HasMaxLength(450);
        });

        modelBuilder.Entity<UserAction>(entity =>
        {
            entity.HasKey(e => e.idUserAction).HasName("PRIMARY");
            entity.ToTable("useraction");
            entity.Property(e => e.userLogin).HasMaxLength(150);
            entity.Property(e => e.actionText).HasMaxLength(450);
            entity.Property(e => e.date);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.IdStatus).HasName("PRIMARY");
            entity.ToTable("status");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(450);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
