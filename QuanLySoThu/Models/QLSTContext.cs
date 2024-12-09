using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLySoThu.Models;

public partial class QLSTContext : DbContext
{
    public QLSTContext()
    {
    }

    public QLSTContext(DbContextOptions<QLSTContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ACTIVE_SESSION> ACTIVE_SESSION { get; set; }

    public virtual DbSet<CHUONG> CHUONG { get; set; }

    public virtual DbSet<CONVAT> CONVAT { get; set; }

    public virtual DbSet<LOAI> LOAI { get; set; }

    public virtual DbSet<TAIKHOAN> TAIKHOAN { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=BJNB0\\SQLEXPRESS;Initial Catalog=QLST;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ACTIVE_SESSION>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ACTIVESESSION");

            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.SessionToken).HasMaxLength(255);

            entity.HasOne(d => d.IDTaiKhoanNavigation).WithMany(p => p.ACTIVE_SESSION)
                .HasForeignKey(d => d.IDTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACTIVESESSION_TAIKHOAN");
        });

        modelBuilder.Entity<CHUONG>(entity =>
        {
            entity.HasIndex(e => e.TenChuong, "UQ__CHUONG__E05612E0BBF66642").IsUnique();

            entity.Property(e => e.MaChuong)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasComputedColumnSql("('CH'+right('00000'+CONVERT([varchar](5),[ID]),(5)))", true);
            entity.Property(e => e.TenChuong).HasMaxLength(100);
        });

        modelBuilder.Entity<CONVAT>(entity =>
        {
            entity.HasIndex(e => e.TenConVat, "UQ__CONVAT__A80568372DA6DB3D").IsUnique();

            entity.Property(e => e.AnhConVat).HasMaxLength(200);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.MaConVat)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasComputedColumnSql("('CV'+right('00000'+CONVERT([varchar](5),[ID]),(5)))", true);
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.TenConVat).HasMaxLength(100);
            entity.Property(e => e.TrangThaiSucKHoe).HasMaxLength(100);

            entity.HasOne(d => d.IDChuongNavigation).WithMany(p => p.CONVAT)
                .HasForeignKey(d => d.IDChuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CONVAT_CHUONG");

            entity.HasOne(d => d.IDLoaiNavigation).WithMany(p => p.CONVAT)
                .HasForeignKey(d => d.IDLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CONVAT_LOAI");
        });

        modelBuilder.Entity<LOAI>(entity =>
        {
            entity.HasIndex(e => e.TenLoai, "UQ__LOAI__E29B104295979BBA").IsUnique();

            entity.Property(e => e.AnhLoai).HasMaxLength(100);
            entity.Property(e => e.KichThuoc).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LoaiDongVat).HasMaxLength(200);
            entity.Property(e => e.MTSTuNhien).HasMaxLength(100);
            entity.Property(e => e.MaLoai)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComputedColumnSql("('L'+right('00000'+CONVERT([varchar](5),[ID]),(5)))", true);
            entity.Property(e => e.MoTaNgoaiHinh).HasMaxLength(1000);
            entity.Property(e => e.PhanLoai).HasMaxLength(100);
            entity.Property(e => e.TenLoai).HasMaxLength(100);
            entity.Property(e => e.ThoiQuenAnUong).HasMaxLength(1000);
            entity.Property(e => e.TuoiTho).HasColumnType("decimal(5, 1)");
        });

        modelBuilder.Entity<TAIKHOAN>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Email").IsUnique();

            entity.HasIndex(e => e.TenTaiKhoan, "UQ_TenTaiKhoan").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Hoten).HasMaxLength(100);
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasComputedColumnSql("('TK'+right('00000'+CONVERT([varchar](5),[ID]),(5)))", true);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenTaiKhoan).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
