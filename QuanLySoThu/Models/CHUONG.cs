using System;
using System.Collections.Generic;

namespace QuanLySoThu.Models;

public partial class CHUONG
{
    public int ID { get; set; }

    public string? MaChuong { get; set; }

    public string TenChuong { get; set; } = null!;

    public int SucChua { get; set; }

    public int SoLuongConVat { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CONVAT> CONVAT { get; } = new List<CONVAT>();
}
