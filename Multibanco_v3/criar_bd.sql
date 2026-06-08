-- =============================================================
-- Projeto Multibanco — Script de Base de Dados
-- BD: lei2526  |  PostgreSQL 18
-- =============================================================
-- COMO USAR:
--   1. Ligar ao psql como superuser:  psql -U postgres
--   2. Correr este script:            \i criar_bd.sql
-- =============================================================

GRANT ALL ON SCHEMA public TO postgres;
SET search_path TO public;

-- Apagar tabelas se já existirem (para poder recorrer o script do zero)
DROP TABLE IF EXISTS Movimentos;
DROP TABLE IF EXISTS Credenciais;
DROP TABLE IF EXISTS Admins;
DROP TABLE IF EXISTS Servicos;

-- =============================================================
-- TABELA: Admins
-- Credenciais do administrador "sibs".
-- Separada dos clientes — o admin não tem conta bancária.
-- =============================================================
CREATE TABLE Admins (
    Id       SERIAL      PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Pin      INTEGER     NOT NULL
);

-- =============================================================
-- TABELA: Credenciais
-- Cada linha é uma conta bancária (como estava em aula).
-- Colunas novas face à versão anterior: Saldo, MBWay.
-- Um cliente com várias contas tem várias linhas
-- (mesmo Banco + Cliente, diferente Conta).
-- =============================================================
CREATE TABLE Credenciais (
    Id      SERIAL         PRIMARY KEY,
    Banco   VARCHAR(50)    NOT NULL,
    Cliente VARCHAR(100)   NOT NULL,
    Conta   INTEGER        NOT NULL UNIQUE,
    Pin     INTEGER        NOT NULL,
    Saldo   DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    MBWay   BOOLEAN        NOT NULL DEFAULT FALSE
);

-- =============================================================
-- TABELA: Movimentos
-- Registo imutável de cada operação (só INSERT, nunca UPDATE/DELETE).
-- ContaId aponta para Credenciais.Id.
-- SaldoApos: saldo calculado no momento e guardado.
-- Tipo:
--   'D' — Depósito
--   'L' — Levantamento
--   'T' — Transferência
--   'P' — Pagamento de serviço
--   'M' — MBWay
-- =============================================================
CREATE TABLE Movimentos (
    Id        SERIAL         PRIMARY KEY,
    ContaId   INTEGER        NOT NULL REFERENCES Credenciais(Id),
    Tipo      CHAR(1)        NOT NULL CHECK (Tipo IN ('D','L','T','P','M')),
    Valor     DECIMAL(10,2)  NOT NULL,
    SaldoApos DECIMAL(10,2)  NOT NULL,
    DataHora  TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Descricao VARCHAR(200)
);

-- =============================================================
-- TABELA: Servicos
-- Lista de serviços pré-definidos para pagamentos no Multibanco.
-- Apenas leitura pela aplicação — gestão futura pelo sibs.
-- =============================================================
CREATE TABLE Servicos (
    Id       SERIAL       PRIMARY KEY,
    Nome     VARCHAR(100) NOT NULL,
    Entidade VARCHAR(5)   NOT NULL
);

-- =============================================================
-- DADOS DE TESTE
-- =============================================================

-- Serviços pré-definidos
INSERT INTO Servicos (Nome, Entidade) VALUES
    ('EDP — Electricidade',   '10001'),
    ('EPAL — Água',           '10002'),
    ('Galp — Gás',            '10003'),
    ('NOS',                   '10004'),
    ('MEO',                   '10005'),
    ('Vodafone',              '10006'),
    ('Carregamento NOS',      '20001'),
    ('Carregamento MEO',      '20002'),
    ('Carregamento Vodafone', '20003');

-- Admin sibs (PIN: 1111)
INSERT INTO Admins (Username, Pin) VALUES
    ('sibs', 1111);

-- Contas de clientes
-- João Silva tem 2 contas (para testar múltiplas contas por cliente)
-- Maria Santos tem MBWay ativo (para testar receção MBWay)
INSERT INTO Credenciais (Banco, Cliente, Conta, Pin, Saldo, MBWay) VALUES
    ('CGD', 'João Silva',   123456, 1234,  850.00, FALSE),  -- Id = 1
    ('CGD', 'João Silva',   123457, 1234,  200.00, FALSE),  -- Id = 2 (segunda conta)
    ('BPI', 'Maria Santos', 654321, 4321, 1500.00, TRUE),   -- Id = 3 (MBWay ativo)
    ('BCP', 'Rui Costa',    111222, 9999,  300.00, FALSE);  -- Id = 4

-- Movimentos de teste
INSERT INTO Movimentos (ContaId, Tipo, Valor, SaldoApos, DataHora, Descricao) VALUES
    (1, 'D', 500.00,  500.00,  '2026-04-01 09:00:00', 'Depósito inicial'),
    (1, 'D', 400.00,  900.00,  '2026-04-10 14:30:00', 'Depósito salário'),
    (1, 'L',  50.00,  850.00,  '2026-05-01 11:00:00', 'Levantamento'),
    (3, 'D', 1500.00, 1500.00, '2026-04-01 09:00:00', 'Depósito inicial'),
    (4, 'D',  300.00,  300.00, '2026-04-01 09:00:00', 'Depósito inicial');
