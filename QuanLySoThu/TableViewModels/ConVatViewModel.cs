using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySoThu.TableViewModels
{
    public class ConVatViewModel : INotifyPropertyChanged
    {
        public string? MaConVat { get; set; }
        public string? TenConVat { get; set; }
        public string? GioiTinh { get; set; }
        public int IDLoai { get; set; }
        public string? Loai {  get; set; }   
        public string? Chuong { get; set; }
        public int IDChuong { get; set; }
        public string? SucKhoe { get; set; }
        public double Tuoi { get; set; } 
        public DateTime? NgaySinh { get; set; }
        public DateTime? NgayNhap { get; set; }        
        public string? AnhConVat { get; set; }
        public string BgColor { get; set; } = null!;
        public string Character => string.IsNullOrEmpty(TenConVat) ? "" : TenConVat[0].ToString().ToUpper();

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
