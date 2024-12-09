using System;
using System.Collections.Generic;

namespace QuanLySoThu.Models;

public partial class ACTIVE_SESSION
{
    public int ID { get; set; }

    public int IDTaiKhoan { get; set; }

    public string SessionToken { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public virtual TAIKHOAN IDTaiKhoanNavigation { get; set; } = null!;
}
