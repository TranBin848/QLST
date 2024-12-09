using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using QuanLySoThu.Charts;
using QuanLySoThu.Models;
namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for BaoCaoThongKe.xaml
    /// </summary>
    public partial class BaoCaoThongKe : INotifyPropertyChanged
    {
        private string _tongSoLuongConVat;
        private string _soThuKhoeManh;
        private string _soConDuc;
        private string _soConCai;

        public string TongSoLuongConVat
        {
            get => _tongSoLuongConVat;
            set
            {
                _tongSoLuongConVat = value;
                OnPropertyChanged(); // Thông báo rằng giá trị đã thay đổi
            }
        }

        public string SoThuKhoeManh
        {
            get => _soThuKhoeManh;
            set
            {
                _soThuKhoeManh = value;
                OnPropertyChanged(); // Thông báo rằng giá trị đã thay đổi
            }
        }
        

        public string SoConDuc
        {
            get => _soConDuc;
            set
            {
                _soConDuc = value;
                OnPropertyChanged(); // Thông báo rằng giá trị đã thay đổi
            }
        }
        public string SoConCai
        {
            get => _soConCai;
            set
            {
                _soConCai = value;
                OnPropertyChanged(); // Thông báo rằng giá trị đã thay đổi
            }
        }

        private void LoadTongSoLuongConVat()
        {
            using (var context = new QLSTContext())
            {
                // Đếm số lượng con vật không bị xóa
                int soLuong = context.CONVAT.Count(c => !c.IsDeleted);
                TongSoLuongConVat = soLuong.ToString(); // Gán giá trị dưới dạng chuỗi
                

                int soKhoeManh = context.CONVAT.Count(c => !c.IsDeleted && c.TrangThaiSucKHoe == "Khỏe");
                SoThuKhoeManh = soKhoeManh.ToString();

                int soconduc = context.CONVAT.Count(c => !c.IsDeleted && c.GioiTinh == "Đực");
                SoConDuc = soconduc.ToString();

                int soconcai = context.CONVAT.Count(c => !c.IsDeleted && c.GioiTinh == "Cái");
                SoConCai = soconcai.ToString();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SeriesCollection PieSeriesCollection { get; set; } = new SeriesCollection();
        public SeriesCollection RowChuongSeriesCollection { get; set; } // Biểu đồ số thú theo chuồng
        public SeriesCollection RowLoaiSeriesCollection { get; set; }   // Biểu đồ số thú theo loài
        public List<string> LabelsChuong { get; set; }                  // Nhãn cho biểu đồ chuồng
        public List<string> LabelsLoai { get; set; }                    // Nhãn cho biểu đồ loài
        public Func<double, string> Formatter { get; set; }             // Định dạng số liệu

        public BaoCaoThongKe()
        {
            InitializeComponent();
            LoadTongSoLuongConVat();
            RowChuongSeriesCollection = new SeriesCollection();
            RowLoaiSeriesCollection = new SeriesCollection();
            LabelsChuong = new List<string>();
            LabelsLoai = new List<string>();
            Formatter = value => value.ToString("N0");
            PieSeriesCollection = new SeriesCollection();
            // Gọi hàm load dữ liệu cho biểu đồ
            LoadRowChuongChartData();
            LoadRowLoaiChartData();
            LoadPieChartData();

            // Gán DataContext để binding
            DataContext = this;
        }
        public void LoadRowChuongChartData()
        {
            using (var context = new QLSTContext())
            {
                // Lấy danh sách chuồng từ DB
                var danhSachChuong = context.CHUONG
                    .Where(c => !c.IsDeleted) // Bỏ qua các bản ghi đã bị xóa
                    .ToList();

                // Tạo danh sách chuồng và số lượng con vật
                var thongKeChuong = danhSachChuong
                    .GroupBy(c => c.TenChuong)
                    .Select(g => new
                    {
                        TenChuong = g.Key,
                        SoLuong = g.Sum(c => c.SoLuongConVat),
                        IsFull = g.All(c => c.SoLuongConVat == c.SucChua)
                    }).ToList();

                // Cập nhật Labels
                LabelsChuong.Clear();
                LabelsChuong.AddRange(thongKeChuong.Select(x => x.TenChuong));

                
                RowChuongSeriesCollection.Clear();
                RowChuongSeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        
                        Values = new ChartValues<int>(thongKeChuong.Select(x => x.SoLuong)),
                        Fill = null, // Sẽ được tùy chỉnh màu theo điều kiện
                        DataLabels = true,
                        //LabelPoint = point => thongKeChuong[(int)point.Key].IsFull ? "Chuồng đầy" : "Chuồng chưa đầy"
                    }
                };
            }
        }

        public void LoadRowLoaiChartData()
        {
            using (var context = new QLSTContext())
            {
                // Lấy dữ liệu từ DB, bao gồm thông tin của bảng CONVAT và LOAI
                var danhSachConVat = context.CONVAT
                    .Include(cv => cv.IDLoaiNavigation) // Đảm bảo load dữ liệu bảng LOAI liên quan
                    .Where(cv => !cv.IsDeleted) // Bỏ qua các bản ghi đã bị xóa
                    .ToList();

                // Nhóm con vật theo loài và đếm số lượng
                var thongKeLoai = danhSachConVat
                    .GroupBy(cv => cv.IDLoaiNavigation.TenLoai)
                    .Select(group => new
                    {
                        TenLoai = group.Key, // Tên loài
                        SoLuong = group.Count() // Số lượng con vật thuộc loài
                    }).ToList();

                // Cập nhật Labels và SeriesCollection
                LabelsLoai.Clear();
                LabelsLoai.AddRange(thongKeLoai.Select(x => x.TenLoai));

                RowLoaiSeriesCollection.Clear();
                RowLoaiSeriesCollection.Add(new ColumnSeries
                {
                    Values = new ChartValues<int>(thongKeLoai.Select(x => x.SoLuong)),
                    Fill = null, // Sẽ được tùy chỉnh màu theo điều kiện
                    DataLabels = true,
                });
            }
        }

        private void LoadPieChartData()
        {
            // Lấy danh sách con vật từ database (giả sử bạn dùng Entity Framework)
            using (var context = new QLSTContext())
            {
                // Lấy dữ liệu từ bảng CONVAT
                var animals = context.CONVAT.Where(cv => !cv.IsDeleted).ToList();

                // Đếm số lượng con vật theo trạng thái sức khỏe
                var healthStatuses = animals.GroupBy(cv => cv.TrangThaiSucKHoe)
                                             .Select(group => new
                                             {
                                                 TrangThai = group.Key,
                                                 SoLuong = group.Count()
                                             }).ToList();

                // Khởi tạo SeriesCollection cho PieChart
                PieSeriesCollection = new SeriesCollection();

                foreach (var status in healthStatuses)
                {
                    PieSeriesCollection.Add(new PieSeries
                    {
                        Title = status.TrangThai,
                        Values = new ChartValues<int> { status.SoLuong },
                        DataLabels = true // Hiển thị nhãn số liệu
                    });
                }
            }
        }

        

        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            btnPDF.Visibility = Visibility.Collapsed;
            string filePath = "BaoCaoThongKe.pdf";

            try
            {
                // Lấy UserControl từ Frame (nếu cần)
                UserControl control = this.Parent as UserControl ?? this;

                // Đảm bảo UserControl được render đầy đủ
                control.UpdateLayout();
                
                // Xuất PDF
                UserControlToImage.SaveToPdf(control, filePath);
                

                MessageBox.Show($"Xuất PDF thành công: {filePath}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Mở file PDF ngay sau khi tạo
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
