# 3. Funcionalidades Cliente

| #    | Funcionalidade          |
| ---- | ----------------------- |
| 3.1  | Login                   |
| 3.2  | Alterar PIN             |
| 3.3  | Histórico de Movimentos |
| 3.4  | Levantamento            |
| 3.5  | Depósito                |
| 3.6  | Transferência           |
| 3.7  | MBWay                   |
| 3.8  | Pagamento de Serviços   |
| 3.9  | Simulador de Empréstimo |
| 3.10 | Relógio em tempo real   |
| 3.11 | Bloqueio de conta       |

NOTAS:
1. Porque motivo temos return com `operacao` , se ela é definida como public? Não é redondante?
2. 

---
### 3.1 Login

**Ficheiros:** `frmAutenticacao.cs` → `cControlo.cs` → `cLogin.cs` → BD → `cLogin.cs` → `cControlo.cs` → `frmAutenticacao.cs`

---
#### Camada 1 — Apresentação (`frmAutenticacao.cs`)
O utilizador preenche 4 campos: **Banco**, **Cliente**, **Conta**, **PIN**.

**Validação em tempo real (eventos `KeyPress`):** acontece enquanto o utilizador escreve, independentemente do botão OK. Cada campo tem o seu evento que bloqueia a tecla antes de aparecer no campo — `txtBanco` e `txtCliente` só aceitam letras, `txtConta` e `txtPin` só aceitam dígitos.

**Ao clicar [OK] (`btnOK_Click`):** fluxo separado e independente da validação acima.

1. Cria uma instância nova de `cControlo` — estado limpo a cada tentativa.
2. Envia os 4 campos para `cControlo.fTestarCampos`. Se algum estiver vazio, recebe `operacao=false` e `mensagem` com o erro — mostra o aviso e para.
3. Se passou, envia os 4 campos para `cControlo.fValidarCredenciais`. Quando este termina, lê `operacao` e `existe`:
   - Se algum for `false` → mostra `mensagem` com o erro e para.
   - Se ambos forem `true` → login bem-sucedido. Abre `frmMultibanco` passando `contaId`, `saldo`, `banco`, `cliente` e `conta` pelo construtor. O ecrã principal não pode existir sem estes 5 valores.

---
#### Camada 2 — Controlo (`cControlo.cs`)
**`fTestarCampos`** — recebe os 4 campos. Verifica localmente se algum está vazio, sem ir à BD. Se sim, define `operacao=false` e `mensagem`. Devolve o controlo ao formulário.

**`fValidarCredenciais`** — recebe os 4 campos. Cria uma instância de `cLogin` (Camada 3) e chama `fObterCredenciais`, passando os mesmos 4 campos. Quando `fObterCredenciais` termina, copia todas as variáveis do `cLogin` para as suas próprias variáveis públicas (`operacao`, `existe`, `mensagem`, `contaId`, `saldo`, `banco`, `cliente`, `conta`). Copia sempre, se usar `if`. O formulário é que decide o que fazer consoante o valor de `operacao` que recebe do controlo.

---
#### Camada 3 — Dados (`cLogin.cs` → `fObterCredenciais`)
Recebe os 4 campos. Executa 4 queries em sequência:

**Q1 — Verificar bloqueio:** pergunta à BD se a conta está bloqueada. Se sim, define `operacao=false`, `existe=false`, `mensagem` com o motivo, e termina imediatamente — sem tentar as credenciais.

**Q2 — Validar credenciais:** pesquisa na tabela `Credenciais` uma linha onde os 4 campos coincidam. Se encontrar, preenche `contaId`, `saldo`, `banco`, `cliente`, `conta`. Se não encontrar, define `operacao=false`, `existe=false` e `mensagem` de erro.

**Q3 — Login OK:** se `operacao=true`, faz `UPDATE Tentativas=0` para anular tentativas anteriores falhadas.

**Q4 — Login falhado:** se `operacao=false`, faz `UPDATE Tentativas+1`. Usa `CASE WHEN` no próprio SQL para passar `Bloqueada=TRUE` se o contador chegar a 3 — tudo numa única query, sem ler e voltar a escrever.

Devolve `operacao` (bool) ao `cControlo`.

---
#### Fluxo resumido

```
frmAutenticacao.btnOK_Click
  │
  ├─ fTestarCampos(Banco, Cliente, Conta, PIN)
  │    operacao=false → erro "Preencha todos os campos." — para aqui
  │
  └─ fValidarCredenciais(Banco, Cliente, Conta, PIN)
        │
        └─ cLogin.fObterCredenciais(Banco, Cliente, Conta, PIN)
              ├─ Q1: conta bloqueada?    → se sim: operacao=false, termina
              ├─ Q2: credenciais válidas? → se não: operacao=false
              ├─ Q3: (se OK)  Tentativas=0
              └─ Q4: (se KO)  Tentativas+1, bloquear na 3ª
              └─ devolve operacao ao cControlo
        │
        └─ cControlo copia tudo → formulário lê operacao
              ├─ false → MessageBox mensagem
              └─ true  → new frmMultibanco(contaId, saldo, banco, cliente, conta)
```

---
### 3.2 Alterar PIN

**Ficheiros:** `frmAutenticacao.cs` → `cControlo.cs` → `cLogin.cs` → BD → `cLogin.cs` → `cControlo.cs` → `frmAutenticacao.cs`

---
#### Camada 1a — Apresentação (`frmAutenticacao.cs` → `btnAltPin_Click`)
O botão "Alterar PIN" está no mesmo ecrã de autenticação. O utilizador preenche os 4 campos normalmente (Banco, Cliente, Conta, PIN atual) e clica "Alterar PIN".

1. Cria uma instância nova de `cControlo` — estado limpo.
2. Envia os 4 campos para `cControlo.fTestarCampos`. Se algum estiver vazio, mostra erro e para.
3. Se passou, envia os 4 campos para `cControlo.fValidarCredenciais` - confirma que as credenciais são válidas antes de permitir alterar.
   - Se inválidas → mostra `mensagem` de erro e para. O `frmUpdate` não abre.
   - Se válidas → abre novo `frmUpdate` passando os 4 campos pelo construtor.

---
#### Camada 1b — Apresentação (`frmUpdate.cs`)
Abre com os campos pré-identificados. O utilizador introduz o **novo PIN** e confirma.

1. Envia os dois PINs para `cControlo.fValidarPinsIguais`. Se forem diferentes, mostra erro e para.
2. Se iguais, chama `cControlo.fAtualizarPin` com o PIN antigo e o novo.
3. Lê `operacao` — se `true`, mostra sucesso e fecha.

---
#### Camada 2 — Controlo (`cControlo.cs`)
**`fValidarPinsIguais`** — recebe os dois PINs. Compara localmente. Se diferentes, define `operacao=false` e `mensagem`. Sem ir à BD.

**`fAtualizarPin`** — recebe banco, cliente, conta, PIN antigo e PIN novo. Cria uma instância de `cLogin` e chama `fGravarNovoPin`. Copia `operacao` e `mensagem` do resultado.

---
#### Camada 3 — Dados (`cLogin.cs` → `fGravarNovoPin`)
Recebe os 5 valores. Executa um `UPDATE Credenciais SET Pin = @novoPin WHERE Banco=@Banco AND Cliente=@Cliente AND Conta=@Conta AND Pin=@oldPin`. O PIN antigo está no `WHERE` como verificação extra — se alguém chamar este método sem ter validado antes, o `UPDATE` não afeta nenhuma linha. Devolve `operacao` ao `cControlo`.

---
#### Fluxo resumido

```
frmAutenticacao.btnAltPin_Click
  │
  ├─ fTestarCampos(Banco, Cliente, Conta, PIN)
  │    operacao=false → erro — para aqui
  │
  └─ fValidarCredenciais(Banco, Cliente, Conta, PIN)
        operacao=false → erro — frmUpdate não abre
        operacao=true  → abre frmUpdate(Banco, Cliente, Conta, PIN)
              │
              ├─ fValidarPinsIguais(novoPin, confirmarPin)
              │    operacao=false → erro "PINs diferentes" — para aqui
              │
              └─ fAtualizarPin(Banco, Cliente, Conta, pinAntigo, novoPin)
                    │
                    └─ cLogin.fGravarNovoPin → UPDATE Pin WHERE Pin=@oldPin
                          operacao=true → sucesso, fecha frmUpdate
```

---

### 3.3 Histórico de Movimentos

**Ficheiros:** `Form1.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `Form1.cs`

---
#### Camada 1 — Apresentação (`Form1.cs` → `fCarregarMovimentos`)
Carregado automaticamente no `frmMultibanco_Load` é a primeira coisa que corre ao abrir o ecrã principal. O utilizador pode também filtrar por intervalo de datas e clicar "Filtrar", ou clicar "Ver Todos" para repor sem filtro.
- Sem filtro: chama `fCarregarMovimentos()` sem argumentos — `dataInicio` e `dataFim` ficam `null`.
- Com filtro: chama `fCarregarMovimentos(dtpInicio.Value, dtpFim.Value)` com as datas selecionadas.
- Recebe de `cControlo` uma lista (`listaMovimentos`) de linhas, cada uma com 5 campos: Tipo, Valor, SaldoApos, DataHora, Descrição.
- Antes de mostrar, converte o código de tipo para texto legível (`fTraduzirTipo`): `"D"`→Depósito, `"L"`→Levantamento, `"T"`→Transferência, `"P"`→Pagamento, `"M"`→MBWay.
- A coluna Descrição ajusta a largura ao conteúdo automaticamente (`AutoResizeColumn`).

---
#### Camada 2 — Controlo (`cControlo.cs` → `fListarMovimentos`)
Recebe `contaId`, `dataInicio` e `dataFim`. Cria uma instância de `cMovimento` e chama `fListarMovimentos` com os mesmos argumentos. Quando termina, copia `operacao`, `mensagem` e `listaMovimentos` para as suas variáveis públicas.

---
#### Camada 3 — Dados (`cMovimento.cs` → `fListarMovimentos`)
Recebe `contaId` e as duas datas (podem ser `null`). Executa um `SELECT` na tabela `Movimentos`:
- Se as datas forem `null` → sem filtro, devolve todos os movimentos da conta ordenados por data.
- Se as datas tiverem valor → aplica `WHERE DataHora BETWEEN @Inicio AND @Fim`.

Devolve `listaMovimentos` (lista de arrays de strings) ao `cControlo`.

---
#### Fluxo resumido

```
Form1.frmMultibanco_Load (ou btnFiltrar / btnVerTodos)
  │
  └─ fCarregarMovimentos(dataInicio?, dataFim?)
        │
        └─ cControlo.fListarMovimentos(contaId, dataInicio?, dataFim?)
              │
              └─ cMovimento.fListarMovimentos(contaId, dataInicio?, dataFim?)
                    └─ SELECT Movimentos [WHERE BETWEEN] ORDER BY DataHora
                    └─ devolve listaMovimentos ao cControlo
        │
        └─ Form1 itera listaMovimentos → preenche ListView
              └─ fTraduzirTipo converte "L"/"D"/... para texto legível
```

---
### 3.4 Levantamento

**Ficheiros:** `Form1.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `Form1.cs`

---
#### Camada 1 — Apresentação (`Form1.cs` → `btnLevantar_Click`)

O utilizador introduz o valor a levantar e clica "Levantar".

1. Valida o campo de valor localmente, ou seja, recorre a um método auxiliar (`fValidarValor`), e verifica se é numérico e maior que zero. Se não for, mostra erro e para.
2. Cria uma instância nova de `cControlo` e chama `fLevantar` passando `contaId` (da sessão) e o `valor`.
3. Quando termina, chama `fMostrarResultado` — se `operacao=false` mostra o erro; se `true` atualiza o saldo no ecrã e regista o movimento no histórico.

---
#### Camada 2 — Controlo (`cControlo.cs` → `fLevantar`)

Recebe `contaId` e `valor`. Cria uma instância de `cMovimento` (Camada 3) e:

1. Chama `fObterSaldo` para obter o saldo atual da conta. Se falhar (BD offline), define `operacao=false` e para.
2. Verifica se `valor > 0`. Se não, define `operacao=false` e para.
3. Verifica se o saldo é suficiente (`fSaldoSuficiente`). Se não, define `operacao=false` e para.
4. Calcula `novoSaldo = saldoAtual - valor`.
5. Chama `fAtualizarSaldo` para gravar o novo saldo na BD.
6. Chama `fInserirMovimento` para registar o movimento com tipo `"L"`.
7. Copia `operacao`, `mensagem` e `saldoAtualizado` do `cMovimento` para as suas variáveis públicas.

---
#### Camada 3 — Dados (`cMovimento.cs`)

**`fObterSaldo`** — recebe `contaId`. Executa `SELECT Saldo FROM Credenciais WHERE Id = @Id`. Devolve o saldo.

**`fAtualizarSaldo`** — recebe `contaId` e `novoSaldo`. Executa `UPDATE Credenciais SET Saldo = @Saldo WHERE Id = @Id`.

**`fInserirMovimento`** — recebe `contaId`, tipo `"L"`, valor, novoSaldo e descrição. Executa `INSERT INTO Movimentos (...)`.

---
#### Fluxo resumido

```
Form1.btnLevantar_Click
  │
  ├─ fValidarValor(txtValor)
  │    inválido → erro — para aqui
  │
  └─ cControlo.fLevantar(contaId, valor)
        │
        ├─ cMovimento.fObterSaldo(contaId)        → Q1: SELECT Saldo
        ├─ verificar valor > 0
        ├─ verificar saldo suficiente
        ├─ cMovimento.fAtualizarSaldo(novoSaldo)  → Q2: UPDATE Saldo
        └─ cMovimento.fInserirMovimento("L", ...) → Q3: INSERT Movimentos
        │
        └─ devolve operacao, mensagem, saldoAtualizado
  │
  └─ fMostrarResultado(oCtrl)
        ├─ false → MessageBox erro
        └─ true  → atualiza saldo no ecrã + refresca histórico
```

---

### 3.5 Depósito

**Ficheiros:** `Form1.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `Form1.cs`

Idêntico ao Levantamento, mas soma em vez de subtrair. Não há verificação de saldo — pode sempre depositar. Tipo de movimento: `"D"`.

---

### 3.6 Transferência

**Ficheiros:** `Form1.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `Form1.cs`

Envolve duas contas. O utilizador introduz o número de conta destino (visível).

1. Resolver conta destino — converter número visível em `Id` interno e obter o seu saldo.
2. Verificar que destino ≠ origem.
3. Verificar saldo suficiente.
4. Atualizar os dois saldos e registar 2 movimentos (tipo `"T"`) — um por cada conta.

---

### 3.7 MBWay

**Ficheiros:** `Form1.cs` → `frmMBWay.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `frmMBWay.cs` → `Form1.cs`

Ao abrir o ecrã principal, verifica se a conta tem MBWay ativo — se não tiver, o botão fica desativado.

Ao clicar "MBWay", abre `frmMBWay` com a lista de contas que têm MBWay ativo, excluindo a própria. O utilizador seleciona o destinatário e introduz o valor.

Regras adicionais face à Transferência:
- Limite de 300€ por transação.
- Conta destino também tem de ter MBWay ativo.

Tipo de movimento: `"M"`.

---

### 3.8 Pagamento de Serviços

**Ficheiros:** `Form1.cs` → `frmPagamentosServicos.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `frmPagamentosServicos.cs` → `Form1.cs`

Ao abrir `frmPagamentosServicos`, carrega a lista de serviços pré-definidos da tabela `Servicos`. O utilizador seleciona o serviço, introduz referência (9 dígitos) e valor. Funciona como um Levantamento mas com tipo `"P"` e associado a um serviço.

---

### 3.9 Simulador de Empréstimo

Sem acesso à BD — cálculo local com a fórmula PMT:

```
prestacao = capital × (taxa / 12) / (1 - (1 + taxa/12)^(-meses))
```

---

### 3.10 Relógio em tempo real

Timer configurado com intervalo de 1 segundo. A cada tick atualiza um label com `DateTime.Now` no formato `dd/MM/yyyy HH:mm:ss`.

---

### 3.11 Bloqueio de conta

- Automático via `CASE WHEN` no SQL após 3 tentativas falhadas (ver Q4 do Login).
- Guardado na BD — persiste ao fechar a app.
- Bloqueio por conta, não por utilizador.
- Login bem-sucedido reseta `Tentativas=0`.
- Mensagem distinta: "Credenciais inválidas" vs "Conta bloqueada após 3 tentativas".
- Desbloqueio: só o admin sibs (ver 4.6).
