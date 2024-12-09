using System;
using System.Collections.Generic;

namespace QuanLySoThu.Models;

public partial class CONVAT
{
    public int ID { get; set; }

    public string? MaConVat { get; set; }

    public string TenConVat { get; set; } = null!;

    public string GioiTinh { get; set; } = null!;

    public DateTime NgaySinh { get; set; }

    public DateTime NgayNhap { get; set; }

    public string TrangThaiSucKHoe { get; set; } = null!;

    public int IDChuong { get; set; }

    public int IDLoai { get; set; }

    public bool IsDeleted { get; set; }

    public string? AnhConVat { get; set; }

    public virtual CHUONG IDChuongNavigation { get; set; } = null!;

    public virtual LOAI IDLoaiNavigation { get; set; } = null!;
}
