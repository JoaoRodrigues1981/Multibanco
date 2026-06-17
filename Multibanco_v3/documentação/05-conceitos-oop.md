# 5. Conceitos OOP usados

## Encapsulamento

Os dados da sessão em `frmMultibanco` são **privados** — só o próprio formulário os acede:

```csharp
private int     _contaId;   // underscore = convenção para campo privado
private decimal _saldo;
private string  _banco;
private string  _cliente;
private int     _conta;
```

Se fossem `public`, qualquer classe poderia corromper os dados da sessão.

---

## Construtor com parâmetros

O `frmMultibanco` **só pode existir com os dados da conta** — obriga a que o login seja sempre feito primeiro:

```csharp
public frmMultibanco(int contaId, decimal saldo, string banco, string cliente, int conta)
{
    InitializeComponent();
    _contaId = contaId;
    _saldo   = saldo;
    // ...
}
```

`new frmMultibanco(contaId, saldo, banco, cliente, conta)` garante que nunca existe um ecrã principal sem sessão válida.

---

## Propriedade pública (get-only)

`frmMBWay` e `frmPagamentosServicos` passam o novo saldo ao formulário pai **sem expor o estado interno**:

```csharp
public decimal NovoSaldo { get; private set; }
// Form1 pode ler, mas só o frmMBWay pode escrever
```

---

## Separação de responsabilidades

| Classe | Responsabilidade |
|--------|-----------------|
| `cConexao` | Sabe ligar à BD |
| `cLogin` | Autenticar e gerir tentativas |
| `cAdmin` | Gerir clientes (CRUD) |
| `cMovimento` | Operações bancárias |
| `cMBWay` | Listar contas MBWay |
| `cServico` | Listar serviços |
| `cControlo` | Coordena tudo — nunca faz SQL |

---

## DRY — Don't Repeat Yourself

4 helpers que evitam código duplicado:

| Helper | Onde | Para quê |
|--------|------|----------|
| `fExecutarNonQuery` | `cAdmin` | Encapsula try/catch de todos os UPDATE/DELETE |
| `fSaldoSuficiente` | `cControlo` | Verifica saldo antes de Levantar/Transferir/MBWay/Pagar |
| `fMostrarResultado` | `Form1` | Mostra MessageBox e refresca ecrã após operação |
| `fMostrarResultadoEAtualizar` | `frmAdmin` | Mostra resultado e recarrega lista de clientes |
