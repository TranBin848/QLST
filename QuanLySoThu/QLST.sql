create database QLST
use QLST

create table CONVAT
(
	[ID] int identity(1, 1) constraint [PK_CONVAT] primary key,
	[MaConVat] as ('CV' + right('00000' + cast([ID] as varchar(5)), 5)) persisted,
	[TenConVat] nvarchar(100) unique not null,
	[GioiTinh] nvarchar(10) not null,
	[NgaySinh] datetime not null,
	[NgayNhap] datetime not null,	
	[TrangThaiSucKHoe] nvarchar(100) not null,
	[IDChuong] int not null,
	[IDLoai] int not null,
	[IsDeleted] bit not null,
)
ALTER TABLE CONVAT
ADD CONSTRAINT CK_NgayNhap_Greater_Than_NgaySinh
CHECK (NgayNhap > NgaySinh);
ALTER TABLE CONVAT
ADD CONSTRAINT FK_CONVAT_CHUONG
FOREIGN KEY (IDChuong) REFERENCES CHUONG(ID); 
ALTER TABLE CONVAT
ADD CONSTRAINT FK_CONVAT_LOAI
FOREIGN KEY (IDLoai) REFERENCES LOAI(ID); 
create table CHUONG
(
	[ID] int identity(1, 1) constraint [PK_CHUONG] primary key,
	[MaChuong] as ('CH' + right('00000' + cast([ID] as varchar(5)), 5)) persisted,
	[TenChuong] nvarchar(100) unique not null,
	[SucChua] INT NOT NULL,  -- Sức chứa của chuồng (số lượng con vật tối đa có thể chứa)
	[SoLuongConVat] INT NOT NULL DEFAULT 0,  -- Số lượng con vật trong chuồng, mặc định là 0
	[IsDeleted] bit not null,
)
ALTER TABLE CHUONG
ADD CONSTRAINT CK_SoLuongConVat_KhongVuotSucChua CHECK (SoLuongConVat <= SucChua);

create table LOAI
(
	[ID] int identity(1, 1) constraint [PK_LOAI] primary key,
	[MaLoai] as ('L' + right('00000' + cast([ID] as varchar(5)), 5)) persisted,
	[TenLoai] nvarchar(100) unique not null,
	[MoTaNgoaiHinh] nvarchar(1000) ,  
	[KichThuoc] decimal(10, 2) , 
	[MTSTuNhien] nvarchar(100) ,
	[TuoiTho] decimal(5, 1) ,
	[ThoiQuenAnUong] nvarchar(1000) ,
	[AnhLoai] nvarchar(100),
	[IsDeleted] bit not null default 0,
	[LoaiDongVat] nvarchar(200) not null,
	[PhanLoai] nvarchar(100) not null,
)
