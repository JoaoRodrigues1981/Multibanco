# Multibanco v3

**Tecnologias:** C# / .NET 8 / Windows Forms / PostgreSQL / Npgsql  
**BD:** `Multibanco` | Host: localhost | User: postgres | Password: 1234  
**Entrega:** 17 Jun 2026
**Apresentação:** 18 Jun 2026

---

## Documentação

| Ficheiro | Conteúdo |
|----------|----------|
| [01-arquitetura](documentação/01-arquitetura.md) | As 3 camadas e classes por camada |
| [02-comunicacao-bd](documentação/02-comunicacao-bd.md) | ExecuteReader, ExecuteNonQuery, SQL Injection, padrões |
| [03-funcionalidades-cliente](documentação/03-funcionalidades-cliente.md) | Login, Levantamento, Depósito, Transferência, MBWay, Pagamentos, Histórico, Alterar PIN, Simulador, Relógio, Bloqueio |
| [04-funcionalidades-backoffice](documentação/04-funcionalidades-backoffice.md) | Login Admin, Listar, Inserir, Eliminar, MBWay toggle, Desbloquear |
| [05-conceitos-oop](documentação/05-conceitos-oop.md) | Encapsulamento, construtor, propriedades, DRY |
| [06-base-de-dados](documentação/06-base-de-dados.md) | Tabelas, relações, decisões de design |
| [07-dados-teste](documentação/07-dados-teste.md) | 10 contas + admin para testes |
| [08-testes-manuais](documentação/08-testes-manuais.md) | 82 testes manuais (T01–T82) |
| [09-perguntas-professor](documentação/09-perguntas-professor.md) | Respostas rápidas para a apresentação |

---

## Funcionalidades — visão geral

### Cliente
| # | Funcionalidade | Obrigatório |
|---|----------------|-------------|
| 3.1 | Login (4 campos: Banco + Cliente + Conta + PIN) | ✅ |
| 3.2 | Levantamento | ✅ |
| 3.3 | Depósito | ✅ |
| 3.4 | Transferência entre contas | ✅ |
| 3.5 | MBWay (janela dedicada, lista contactos, limite 300€) | ✅ |
| 3.6 | Pagamento de Serviços (pré-definidos) | ✅ |
| 3.7 | Histórico de Movimentos + filtro de datas | ✅ |
| 3.8 | Alterar PIN | ✅ |
| 3.9 | Simulador de Empréstimo (fórmula PMT) | Extra |
| 3.10 | Relógio em tempo real | Extra |
| 3.11 | Bloqueio de conta (3 tentativas) | Extra |

### BackOffice SIBS
| # | Funcionalidade | Obrigatório |
|---|----------------|-------------|
| 4.1 | Login Admin (sibs / 1111) | ✅ |
| 4.2 | Listar Clientes | ✅ |
| 4.3 | Inserir Cliente (saldo inicial 100€) | ✅ |
| 4.4 | Eliminar Cliente (bloqueado se saldo ≠ 0) | ✅ |
| 4.5 | Ativar / Desativar MBWay | Extra |
| 4.6 | Desbloquear Conta | Extra |

---

## Credenciais rápidas para a apresentação

| Conta | Banco | Cliente | PIN | MBWay |
|-------|-------|---------|-----|-------|
| 123456 | CGD | João Silva | 1234 | Não |
| 654321 | BPI | Maria Santos | 4321 | **Sim** |
| Admin: sibs | — | — | 1111 | — |
