# 6. Base de Dados

## Tabelas

### Credenciais — uma linha por conta bancária

```sql
Id         SERIAL PRIMARY KEY
Banco      VARCHAR(50) NOT NULL
Cliente    VARCHAR(100) NOT NULL
Conta      INTEGER UNIQUE NOT NULL    -- número visível (ex: 123456)
Pin        INTEGER NOT NULL
Saldo      DECIMAL(10,2) DEFAULT 0.00
MBWay      BOOLEAN DEFAULT FALSE
Telefone   VARCHAR(9)                 -- telemóvel para MBWay
Tentativas INTEGER DEFAULT 0         -- contador de falhas de login
Bloqueada  BOOLEAN DEFAULT FALSE     -- TRUE após 3 tentativas falhadas
```

### Movimentos — uma linha por operação

```sql
Id        SERIAL PRIMARY KEY
ContaId   INTEGER REFERENCES Credenciais(Id)
Tipo      CHAR(1)        -- 'D'=Depósito, 'L'=Levantamento, 'T'=Transferência, 'P'=Pagamento, 'M'=MBWay
Valor     DECIMAL(10,2)
SaldoApos DECIMAL(10,2) -- snapshot imutável do saldo após a operação
Descricao VARCHAR(200)
DataHora  TIMESTAMP DEFAULT NOW()
```

### Servicos — lista pré-definida

```sql
Id       SERIAL PRIMARY KEY
Nome     VARCHAR(100)
Entidade VARCHAR(5)
```

### Admins — credenciais do BackOffice

```sql
Id       SERIAL PRIMARY KEY
Username VARCHAR(50)   -- "sibs"
Pin      INTEGER
```

---

## Relação entre tabelas

```
Credenciais (Id) ←── ContaId ── Movimentos
```

`ContaId` em `Movimentos` é uma **chave estrangeira** — cada movimento pertence a uma conta.

---

## Decisões de design

| Decisão | Motivo |
|---------|--------|
| Movimentos só INSERT, nunca UPDATE/DELETE | Histórico imutável — auditoria |
| `SaldoApos` guardado em cada movimento | Snapshot: não depende de recalcular |
| `Tentativas` + `Bloqueada` na mesma tabela | Bloqueio por conta, não por utilizador |
| `Telefone` em `Credenciais` | Necessário para listar contactos MBWay |
