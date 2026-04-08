-- Eğer varsa önce siler, sıfırdan temiz kurulum yapar:
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'EczaneDB')
DROP DATABASE EczaneDB;
GO

-- Veritabanını oluşturuyoruz
CREATE DATABASE EczaneDB;
GO

-- Veritabanını kullanacağımızı belirtiyoruz
USE EczaneDB;
GO

-- Tablomuzu oluşturuyoruz
CREATE TABLE Urunler (
    UrunID int IDENTITY(1,1) PRIMARY KEY,
    UrunAdi nvarchar(50),
    Kategori nvarchar(50),
    Miad date
);
GO

-- Örnek ilaçlar ekliyoruz
INSERT INTO Urunler (UrunAdi, Kategori, Miad) VALUES 
('Parol 500mg', 'Ağrı Kesici', '2026-08-15'),
('Benexol B12', 'Vitamin', '2025-11-20'),
('Majezik Sprey', 'Boğaz', '2024-12-01'),
('N95 Maske', 'Medikal', '2030-01-01'),
('Augmentin 1000mg', 'Antibiyotik', '2025-06-10');
GO