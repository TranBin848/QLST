using System;
using System.Collections.Generic;

namespace QuanLySoThu.Models;

public partial class LOAI
{
    public int ID { get; set; }

    public string? MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public string? MoTaNgoaiHinh { get; set; }

    public decimal? KichThuoc { get; set; }

    public string? MTSTuNhien { get; set; }

    public decimal? TuoiTho { get; set; }

    public string? ThoiQuenAnUong { get; set; }

    public string? AnhLoai { get; set; }

    public bool IsDeleted { get; set; }

    public string LoaiDongVat { get; set; } = null!;

    public string PhanLoai { get; set; } = null!;

    public virtual ICollection<CONVAT> CONVAT { get; } = new List<CONVAT>();
}
