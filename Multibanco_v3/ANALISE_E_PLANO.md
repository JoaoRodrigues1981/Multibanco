# Plano de Refactor — Redundâncias

> Ponto de situação: o ponto 1 (duplicação de `fObterSaldo`) já foi feito.
> Retomar a partir do ponto 2.

---

## ✅ FEITO — Ponto 1: `cAdmin.fObterSaldo` duplicado

- `_03Dados/cAdmin.cs` — método `fObterSaldo` removido; comentário adicionado a explicar que não existe intencionalmente
- `_02Controlo/cControlo.cs` — `fEliminarCliente` passou a usar `new cMovimento().fObterSaldo(id)` em vez de `oAdmin.fObterSaldo(id)`

---

## ⬜ Ponto 2: Handlers vazios em `frmAutenticacao`

### `_01Apresentacao/frmAutenticacao.cs`
Remover estes dois métodos completos (não fazem nada):

```csharp
// Evento que corre quando o formulário carrega (abre) — vazio por agora
private void frmAutenticacao_Load(object sender, EventArgs e)
{
}

// Evento KeyUp no campo Conta — vazio (reservado para uso futuro)
private void txtConta_KeyUp(object sender, KeyEventArgs e)
{
}
```

### `_01Apresentacao/frmAutenticacao.Designer.cs`
Remover os dois wirings correspondentes (dentro de `InitializeComponent`):

```csharp
Load += frmAutenticacao_Load;          // linha ~138
txtConta.KeyUp += txtConta_KeyUp;      // linha ~80
```

---

## ⬜ Ponto 3: Handlers vazios em `frmUpdate`

### `_01Apresentacao/frmUpdate.cs`
Remover estes três métodos completos (não fazem nada):

```csharp
private void txtNovoPin_TextChanged(object sender, EventArgs e)
{
}

private void txtConfPin_TextChanged(object sender, EventArgs e)
{
}

private void txtConfPin_TextChanged_1(object sender, EventArgs e)
{
}
```

### `_01Apresentacao/frmUpdate.Designer.cs`
Remover os dois wirings correspondentes (dentro de `InitializeComponent`):

```csharp
txtNovoPin.TextChanged += txtNovoPin_TextChanged;       // linha ~50
txtConfPin.TextChanged += txtConfPin_TextChanged_1;     // linha ~101
```

> Nota: `txtConfPin_TextChanged` não tem wiring no Designer — só existe no .cs. Remover apenas o método.

---

## ⬜ Ponto 4: Handler vazio em `frmMultibanco` (Form1)

### `_01Apresentacao/Form1.cs`
Remover este método completo:

```csharp
// Evento de clique no label do título — vazio, não faz nada
private void label1_Click(object sender, EventArgs e)
{
}
```

### `_01Apresentacao/Form1.Designer.cs`
Remover o wiring correspondente (dentro de `InitializeComponent`, no bloco do `label1`):

```csharp
label1.Click += label1_Click;    // linha ~74
```

---

## ⬜ Ponto 5 (decisão do utilizador): `frmPagamento.cs` — código morto

`frmPagamento.cs` e `frmPagamento.Designer.cs` existem mas **nunca são chamados** da UI.
Foi substituído por `frmPagamentosServicos`. Duas opções:

- **Eliminar** — limpa o projeto, sem código morto
- **Manter** — sem impacto funcional, mas confunde quem lê o projeto

**Aguarda decisão do utilizador antes de agir.**

---

# Multibanco — Análise e Plano de Trabalho

> Atualizado: 2026-06-08
> Entrega: 17/Junho 23:59 | Apresentação: 18/Junho

---

## 1. O que está feito

| Componente | Estado | Notas |
|---|---|---|
| Arquitetura 3 camadas | ✅ | Pastas `_01`, `_02`, `_03` criadas e a funcionar |
| `criar_bd.sql` — 3 tabelas | ✅ | `Admins`, `Credenciais` (+ Saldo, MBWay), `Movimentos` com dados de teste |
| `cConexao.cs` | ✅ | Ligação PostgreSQL funcional; duplicado `desconectar` removido; `Search Path=public` na connection string |
| `cLogin.cs` | ✅ | `fObterCredenciais`, `fValidarAdmin`, `fGravarNovoPin` |
| `cAdmin.cs` | ✅ | `fListarCredenciais`, `fInserirCredencial`, `fObterSaldo`, `fEliminarCredencial`, `fAlternarMBWay` |
| `cMovimento.cs` | ✅ | `fObterSaldo`, `fAtualizarSaldo`, `fObterMBWay`, `fObterContaDestino`, `fInserirMovimento`, `fListarMovimentos` |
| `cControlo.cs` | ✅ | Todos os métodos: login, admin, PIN, CRUD backoffice, operações bancárias, helpers DRY |
| `frmAutenticacao` | ✅ | Login cliente + sibs, Alterar PIN, bug `oCtrl` corrigido |
| `frmUpdate` | ✅ | Alterar PIN com encapsulamento (getters/setters) |
| `frmAdmin` | ✅ | Listar, inserir (100€), eliminar (bloqueia se saldo≠0), toggle MBWay |
| `frmMultibanco` | ✅ | Levantar, Depositar, Transferir, MBWay, Pagamentos, filtro de datas, Simulador |
| `frmPagamentosServicos` | ✅ | 9 serviços pré-definidos (EDP, Água, Gás, NOS, MEO, Vodafone, Carregamentos); entidade automática |
| `frmSimuladorEmprestimo` | ✅ | Simulador de empréstimo — Indexado vs Taxa Fixa, fórmula PMT, sem BD |

---

## 2. Funcionalidades — obrigatório vs extra

### Obrigatório (enunciado)

| # | Funcionalidade | Estado |
|---|---|---|
| 1 | Saldo real + movimentos ao entrar | ✅ |
| 2 | Levantamento (validar saldo suficiente) | ✅ |
| 3 | Depósito | ✅ |
| 4 | Transferência entre contas | ✅ |
| 5 | MBWay (verificar conta aderente antes de enviar) | ✅ |
| 6 | Pagamentos de serviços pré-definidos | ✅ `frmPagamentosServicos` — 9 serviços, entidade automática |
| 7 | Histórico com filtro por intervalo de datas | ✅ DateTimePicker De/Até + Filtrar/Todos |
| 8 | Registo imediato de movimento na BD e na UI | ✅ |

### Extra (criatividade — 25% da nota)

| # | Funcionalidade | Estado |
|---|---|---|
| E1 | MBWay toggle pelo sibs no BackOffice | ✅ |
| E2 | Simulador de empréstimo (Indexado vs Taxa Fixa, fórmula PMT) | ✅ |

---

## 3. Estrutura de ficheiros — estado final

```
Multibanco_v3/
├── Program.cs                              ✅
├── criar_bd.sql                            ✅ 3 tabelas + dados de teste
│
├── _01Apresentacao/
│   ├── frmAutenticacao.cs                  ✅
│   ├── frmUpdate.cs                        ✅ Alterar PIN
│   ├── Form1.cs  (frmMultibanco)           ✅ Ecrã principal — todas as operações
│   ├── frmAdmin.cs                         ✅ BackOffice completo
│   ├── frmPagamento.cs                     ✅ Pagamento de serviços
│   └── frmSimuladorEmprestimo.cs           ✅ Simulador de empréstimo (extra)
│
├── _02Controlo/
│   └── cControlo.cs                        ✅ Todos os métodos
│
└── _03Dados/
    ├── cConexao.cs                         ✅
    ├── cLogin.cs                           ✅
    ├── cAdmin.cs                           ✅
    └── cMovimento.cs                       ✅

```

## 4. O que falta antes da entrega

- [ ] Testar casos limite: saldo insuficiente, contas inválidas, MBWay para conta não aderente
- [ ] Rever relatório (`RELATORIO.md`) para entrega
- [ ] Preparar apresentação oral (18/Junho)
- [ ] Criar `.zip` do projeto para entrega
- [ ] Dump da BD PostgreSQL com dados de teste

## 5. Entrega

- **Código-fonte:** `.zip` completo do projeto `Multibanco_v3/`
- **Base de dados:** script `criar_bd.sql` (já inclui dados de teste)
- **Data:** 17/Junho 23:59
- **Apresentação:** 18/Junho (4h de aula)