-- =============================================================
-- Projeto Multibanco — Script de Base de Dados (SQL Server)
-- Disciplina: Arquitetura e Estrutura de Dados — ISLA
-- Ano/Semestre: 2025/26 — 1º Ano / 1º Semestre
--
-- NOTA: O projeto foi desenvolvido com PostgreSQL (Npgsql).
-- Este script é a conversão equivalente para SQL Server,
-- entregue para efeitos de avaliação.
--
-- COMO USAR:
--   1. Abrir SQL Server Management Studio (SSMS)
--   2. Correr este script completo (F5)
-- =============================================================


-- =============================================================
-- CRIAR E SELECIONAR A BASE DE DADOS
-- =============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Multibanco')
    CREATE DATABASE Multibanco;
GO

USE Multibanco;
GO


-- =============================================================
-- APAGAR TABELAS SE JÁ EXISTIREM
-- A ordem importa: primeiro as tabelas com FK, depois as referenciadas
-- =============================================================

DROP TABLE IF EXISTS Movimentos;
DROP TABLE IF EXISTS Credenciais;
DROP TABLE IF EXISTS Admins;
DROP TABLE IF EXISTS Servicos;
GO


-- =============================================================
-- TABELA: Admins
-- Credenciais do administrador "sibs".
-- Separada dos clientes — o admin não tem conta bancária.
-- =============================================================

CREATE TABLE Admins (
    Id       INT           IDENTITY(1,1) PRIMARY KEY,  -- IDENTITY substitui o SERIAL do PostgreSQL
    Username NVARCHAR(50)  NOT NULL UNIQUE,
    Pin      INT           NOT NULL
);
GO


-- =============================================================
-- TABELA: Credenciais
-- Cada linha é uma conta bancária.
-- Um cliente com várias contas tem várias linhas
-- (mesmo Banco + Cliente, diferente Conta).
-- =============================================================

CREATE TABLE Credenciais (
    Id         INT            IDENTITY(1,1) PRIMARY KEY,
    Banco      NVARCHAR(50)   NOT NULL,
    Cliente    NVARCHAR(100)  NOT NULL,
    Conta      INT            NOT NULL UNIQUE,
    Pin        INT            NOT NULL,
    Saldo      DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    MBWay      BIT            NOT NULL DEFAULT 0,    -- BIT substitui BOOLEAN; 0=FALSE, 1=TRUE
    Telefone   NVARCHAR(9)    CHECK (Telefone IS NULL OR (LEN(Telefone) = 9 AND Telefone NOT LIKE '%[^0-9]%')),
    -- SQL Server não tem regex; NOT LIKE '%[^0-9]%' garante que só contém dígitos
    Tentativas INT            NOT NULL DEFAULT 0,
    Bloqueada  BIT            NOT NULL DEFAULT 0
);
GO


-- =============================================================
-- TABELA: Movimentos
-- Registo imutável de cada operação (só INSERT, nunca UPDATE).
-- ContaId aponta para Credenciais.Id (FK).
-- Tipo:
--   'D' — Depósito
--   'L' — Levantamento
--   'T' — Transferência
--   'P' — Pagamento de serviço
--   'M' — MBWay
-- =============================================================

CREATE TABLE Movimentos (
    Id        INT            IDENTITY(1,1) PRIMARY KEY,
    ContaId   INT            NOT NULL REFERENCES Credenciais(Id),
    Tipo      CHAR(1)        NOT NULL CHECK (Tipo IN ('D','L','T','P','M')),
    Valor     DECIMAL(10,2)  NOT NULL,
    SaldoApos DECIMAL(10,2)  NOT NULL,
    DataHora  DATETIME2      NOT NULL DEFAULT SYSDATETIME(),  -- SYSDATETIME() substitui CURRENT_TIMESTAMP
    Descricao NVARCHAR(200)
);
GO


-- =============================================================
-- TABELA: Servicos
-- Lista de serviços pré-definidos para pagamentos no Multibanco.
-- =============================================================

CREATE TABLE Servicos (
    Id       INT           IDENTITY(1,1) PRIMARY KEY,
    Nome     NVARCHAR(100) NOT NULL,
    Entidade NVARCHAR(5)   NOT NULL
);
GO


-- =============================================================
-- DADOS DE TESTE
-- =============================================================

-- Serviços pré-definidos
INSERT INTO Servicos (Nome, Entidade) VALUES
    ('EDP — Electricidade',    '10001'),
    ('EPAL — Água',            '10002'),
    ('Galp — Gás',             '10003'),
    ('NOS',                    '10004'),
    ('MEO',                    '10005'),
    ('Vodafone',               '10006'),
    ('Carregamento NOS',       '20001'),
    ('Carregamento MEO',       '20002'),
    ('Carregamento Vodafone',  '20003');

-- Admin sibs (PIN: 1111)
INSERT INTO Admins (Username, Pin) VALUES
    ('sibs', 1111);

-- Contas de clientes
-- João Silva tem 2 contas (para testar múltiplas contas por cliente)
-- Maria Santos e Ana Lima têm MBWay ativo (para testar envio/receção MBWay)
INSERT INTO Credenciais (Banco, Cliente, Conta, Pin, Saldo, MBWay, Telefone) VALUES
    ('CGD',        'João Silva',      123456, 1234,  850.00, 0, '912345001'),
    ('CGD',        'João Silva',      123457, 1234,  200.00, 0, '912345002'),
    ('BPI',        'Maria Santos',    654321, 4321, 1500.00, 1, '963215001'),
    ('BCP',        'Rui Costa',       111222, 9999,  300.00, 0, '935671001'),
    ('BES',        'Ana Lima',        777888, 5555,  500.00, 1, '916780001'),
    ('Santander',  'Carlos Mendes',   222333, 2233,  750.00, 1, '961234567'),
    ('Millennium', 'Sofia Pereira',   444555, 4455, 1200.00, 1, '936789012'),
    ('Novo Banco', 'Miguel Ferreira', 666777, 6677,  450.00, 0, '912398765'),
    ('Santander',  'Inês Oliveira',   888999, 8899, 2000.00, 1, '964321987'),
    ('CGD',        'Tiago Rodrigues', 321654, 3216,  100.00, 0, '935678901');

-- Movimentos de teste
INSERT INTO Movimentos (ContaId, Tipo, Valor, SaldoApos, DataHora, Descricao) VALUES
    (1, 'D', 500.00,  500.00,  '2026-04-01 09:00:00', 'Depósito inicial'),
    (1, 'D', 400.00,  900.00,  '2026-04-10 14:30:00', 'Depósito salário'),
    (1, 'L',  50.00,  850.00,  '2026-05-01 11:00:00', 'Levantamento'),
    (3, 'D', 1500.00, 1500.00, '2026-04-01 09:00:00', 'Depósito inicial'),
    (4, 'D',  300.00,  300.00, '2026-04-01 09:00:00', 'Depósito inicial'),
    (5, 'D',  500.00,  500.00, '2026-04-01 09:00:00', 'Depósito inicial');
GO


-- =============================================================
-- VERIFICAÇÃO FINAL
-- =============================================================

SELECT 'Admins'      AS Tabela, COUNT(*) AS Registos FROM Admins      UNION ALL
SELECT 'Credenciais' AS Tabela, COUNT(*) AS Registos FROM Credenciais  UNION ALL
SELECT 'Movimentos'  AS Tabela, COUNT(*) AS Registos FROM Movimentos   UNION ALL
SELECT 'Servicos'    AS Tabela, COUNT(*) AS Registos FROM Servicos;
GO
