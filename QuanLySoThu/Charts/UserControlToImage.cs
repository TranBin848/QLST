using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySoThu.Charts
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;

    public static class UserControlToImage
    {
        public static BitmapSource Capture(UserControl control)
        {
            // Kiểm tra kích thước hợp lệ
            Size size = new Size(control.ActualWidth, control.ActualHeight);
            if (size.Width <= 0 || size.Height <= 0)
            {
                throw new InvalidOperationException("UserControl không có kích thước hợp lệ để chụp.");
            }

            // Sử dụng VisualBrush để render
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(control);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), size));
            }

            // Render thành Bitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);

            return renderBitmap;
        }

        public static void SaveToPdf(UserControl control, string filePath)
        {
            BitmapSource bitmap = Capture(control);
            
            // Chuyển BitmapSource thành MemoryStream bằng PngBitmapEncoder
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            
            // Sử dụng MemoryStream để lưu hình ảnh
            byte[] imageData;
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                imageData = stream.ToArray(); // Lưu dữ liệu hình ảnh vào mảng byte
            }
            
            // Tạo PDF bằng PDFsharp
            PdfDocument pdf = new PdfDocument();
            PdfPage page = pdf.AddPage();
            page.Width = bitmap.PixelWidth * 72.0 / 96.0;
            page.Height = bitmap.PixelHeight * 72.0 / 96.0;
            
            

            // Sử dụng XImage từ mảng byte
            try
            {
                using (MemoryStream imageStream = new MemoryStream(imageData))
                {
                    // Kiểm tra nếu dữ liệu trong imageStream có hợp lệ
                    imageStream.Seek(0, SeekOrigin.Begin); // Đặt lại vị trí của stream

                    // Tạo XImage từ stream
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream))
                    {
                        // Chuyển đổi System.Drawing.Image thành XImage để vẽ lên PDF
                        using (var ms = new MemoryStream())
                        {
                            // Lưu ảnh vào MemoryStream dưới dạng PNG
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                            // Tạo XImage từ MemoryStream
                            ms.Seek(0, SeekOrigin.Begin);
                            XImage xImage = XImage.FromStream(ms);

                            // Tạo trang PDF với kích thước tương ứng với ảnh
                            
                            page.Width = xImage.PixelWidth * 72.0 / 96.0;  // Tính toán lại kích thước trang PDF
                            page.Height = xImage.PixelHeight * 72.0 / 96.0;

                            XGraphics gfx = XGraphics.FromPdfPage(page);

                            // Vẽ ảnh lên PDF
                            gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);

                            // Lưu file PDF
                            pdf.Save(filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo XImage từ stream: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            

        }
    }

}
