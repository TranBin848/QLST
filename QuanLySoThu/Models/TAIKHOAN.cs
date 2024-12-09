using System;
using System.Collections.Generic;

namespace QuanLySoThu.Models;

public partial class TAIKHOAN
{
    public int ID { get; set; }

    public string? MaTaiKhoan { get; set; }

    public string TenTaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string Hoten { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<ACTIVE_SESSION> ACTIVE_SESSION { get; } = new List<ACTIVE_SESSION>();
}
