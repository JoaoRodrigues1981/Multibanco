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
2. Removemos validações de campo = 0 dos metodos dos forms, e colocamos apenas no cControlo (regra de negocio)
3. 

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

1. Valida o **formato** do campo de valor com o método auxiliar `fValidarValor` — verifica apenas se o campo contém um número parseável (`decimal.TryParse`). A regra de negócio (valor > 0) fica no `cControlo.fLevantar`.
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

---
#### Camada 1 — Apresentação (`Form1.cs` → `btnDepositar_Click`)
Idêntico ao Levantamento: valida o **formato** do valor com `fValidarValor` (só verifica se é número), cria `cControlo`, chama `fDepositar`, e passa o resultado a `fMostrarResultado`.

---
#### Camada 2 — Controlo (`cControlo.cs` → `fDepositar`)
Recebe `contaId` e `valor`. Diferenças face ao Levantamento:

1. Verifica `valor > 0` — se não, `operacao=false` e para.
2. Obtém o saldo atual (`fObterSaldo`).
3. **Não verifica saldo suficiente** — pode sempre depositar.
4. Calcula `novoSaldo = saldoAtual + valor` (soma em vez de subtrair).
5. Chama `fAtualizarSaldo` e `fInserirMovimento` com tipo `"D"`.

---
#### Camada 3 — Dados (`cMovimento.cs`)
Mesmos métodos do Levantamento: `fObterSaldo`, `fAtualizarSaldo`, `fInserirMovimento`. Só o tipo muda: `"D"`.

---
#### Fluxo resumido
```
Form1.btnDepositar_Click
  │
  ├─ fValidarValor(txtValor)
  │    inválido → erro — para aqui
  │
  └─ cControlo.fDepositar(contaId, valor)
        ├─ verificar valor > 0
        ├─ cMovimento.fObterSaldo(contaId)        → Q1: SELECT Saldo
        ├─ novoSaldo = saldoAtual + valor          (sem verificar saldo)
        ├─ cMovimento.fAtualizarSaldo(novoSaldo)  → Q2: UPDATE Saldo
        └─ cMovimento.fInserirMovimento("D", ...) → Q3: INSERT Movimentos
  │
  └─ fMostrarResultado(oCtrl)
        ├─ false → MessageBox erro
        └─ true  → atualiza saldo no ecrã + refresca histórico
```

---
### 3.6 Transferência

**Ficheiros:** `Form1.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `Form1.cs`

---
#### Camada 1 — Apresentação (`Form1.cs` → `btnTransferir_Click`)
O utilizador introduz o valor e o número de conta destino (o número visível, ex: 654321).
1. Valida o **formato** do valor com `fValidarValor` — apenas se é número parseável. Se não, para.
2. Valida o **formato** da conta destino com `fValidarContaDestino` — apenas se é inteiro positivo válido. Se não, para.
3. Cria `cControlo` e chama `fTransferir` passando `contaId` da sessão, o número de conta destino e o valor.
4. Passa o resultado a `fMostrarResultado`.

---
#### Camada 2 — Controlo (`cControlo.cs` → `fTransferir`)
Recebe `contaOrigem` (Id interno), `contaNumDest` (número visível) e `valor`.
1. Verifica `valor > 0`. Se não, para.
2. Chama `fObterContaDestino` — converte o número visível no `Id` interno da conta destino e obtém o seu saldo atual. Se a conta não existir, para.
3. Verifica que `contaOrigem ≠ idDestino` — não pode transferir para a própria conta.
4. Obtém o saldo da conta origem (`fObterSaldo`).
5. Verifica saldo suficiente.
6. Calcula os dois novos saldos: origem perde, destino ganha.
7. Atualiza os **dois** saldos (`fAtualizarSaldo` × 2).
8. Regista **2 movimentos** tipo `"T"` — um em cada conta (`fInserirMovimento` × 2).

---
#### Camada 3 — Dados (`cMovimento.cs`)
**`fObterContaDestino`** — recebe o número visível. Executa `SELECT Id, Saldo FROM Credenciais WHERE Conta = @Conta`. Devolve `idDestino` e `saldoDestino` via parâmetros `out`.
Os restantes métodos (`fObterSaldo`, `fAtualizarSaldo`, `fInserirMovimento`) são os mesmos do Levantamento.

---
#### Fluxo resumido

```
Form1.btnTransferir_Click
  │
  ├─ fValidarValor(txtValor)          → inválido: para
  ├─ fValidarContaDestino(txtDest)    → inválido: para
  │
  └─ cControlo.fTransferir(contaOrigem, contaNumDest, valor)
        ├─ verificar valor > 0
        ├─ fObterContaDestino(contaNumDest) → SELECT Id, Saldo destino
        │    não encontrada → para
        ├─ verificar contaOrigem ≠ idDestino
        ├─ fObterSaldo(contaOrigem)         → SELECT Saldo origem
        ├─ verificar saldo suficiente
        ├─ fAtualizarSaldo(contaOrigem, novoSaldoOrig) → UPDATE
        ├─ fAtualizarSaldo(idDestino,   novoSaldoDest) → UPDATE
        ├─ fInserirMovimento(contaOrigem, "T", ...)    → INSERT
        └─ fInserirMovimento(idDestino,   "T", ...)    → INSERT
  │
  └─ fMostrarResultado(oCtrl)
        ├─ false → MessageBox erro
        └─ true  → atualiza saldo no ecrã + refresca histórico
```

---

### 3.7 MBWay

**Ficheiros:** `Form1.cs` → `frmMBWay.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `frmMBWay.cs` → `Form1.cs`

---
#### Camada 1 — Apresentação (`Form1.cs` → `btnMBWay_Click`)
No `frmMultibanco_Load`, verifica se a conta tem MBWay ativo (`fVerificarMBWay`). 
- Se não tiver, o botão fica desativado — o utilizador nem chega a abrir a janela.
- Se tive, ao clicar "MBWay", abre `frmMBWay` com `ShowDialog`. Quando fecha com `DialogResult.OK`, lê `frm.NovoSaldo` e refresca o ecrã.

---
#### Camada 1b — Apresentação (`frmMBWay.cs`)
No construtor, chama `fListarContasMBWay` — carrega da BD todas as contas com MBWay ativo, excluindo a própria. Preenche a lista com "Cliente — Telefone". O número de conta destino fica guardado internamente.

Ao clicar "Enviar":
1. Verifica se há destinatário selecionado.
2. Valida o **formato** do valor — verifica apenas se o campo é um número parseável (`decimal.TryParse`). Se não for, mostra erro e para. As regras de negócio (valor > 0, máximo 300€) não são validadas aqui — ficam no `cControlo.fMBWay` para se aplicarem independentemente de qual formulário chamar a operação.
3. Cria `cControlo` e chama `fMBWay` com `contaId`, número de conta destino e valor.
4. Se `operacao=true`, guarda o novo saldo em `NovoSaldo`, fecha com `DialogResult.OK`.

---
#### Camada 2 — Controlo (`cControlo.cs`)
**`fVerificarMBWay`** — recebe `contaId`. Consulta a BD e define `mbwayAtivo` (bool público).

**`fListarContasMBWay`** — recebe `contaId`. Carrega contas com MBWay ativo excluindo a própria. Resultado em `listaContasMBWay`.

**`fMBWay`** — recebe `contaOrigem`, `contaNumDest` e `valor`. É aqui que ficam **todas as regras de negócio da operação**: valor > 0 e máximo 300€. O formulário só valida formato (campo é número?); as regras ficam nesta camada para se aplicarem independentemente do formulário que chamar `fMBWay`. Internamente igual à Transferência mas verifica que a conta destino tem MBWay ativo. Tipo de movimento: `"M"`.

---
#### Camada 3 — Dados (`cMovimento.cs`)
Os mesmos métodos da Transferência. A query de `fListarContasMBWay` filtra `WHERE MBWay = TRUE AND Id != @ContaOrigem`.

---
#### Fluxo resumido

```
Form1.frmMultibanco_Load
  └─ fVerificarMBWay(contaId) → se mbwayAtivo=false: btnMBWay.Enabled=false

Form1.btnMBWay_Click
  └─ abre frmMBWay(contaId)
        │  construtor: fListarContasMBWay → lista destinatários
        │
        └─ btnEnviar_Click
              ├─ validar destinatário selecionado
              ├─ validar valor > 0 e ≤ 300€   (local, sem BD)
              └─ cControlo.fMBWay(contaOrigem, contaNumDest, valor)
                    ├─ verificar MBWay ativo no destino
                    ├─ verificar saldo suficiente
                    ├─ fAtualizarSaldo × 2 + fInserirMovimento("M") × 2
                    └─ devolve operacao, saldoAtualizado
              │
              ├─ false → MessageBox erro
              └─ true  → NovoSaldo = saldoAtualizado; DialogResult.OK
  │
  └─ Form1 lê frm.NovoSaldo → fRefrescarEcra
```

---

### 3.8 Pagamento de Serviços

**Ficheiros:** `Form1.cs` → `frmPagamentosServicos.cs` → `cControlo.cs` → `cMovimento.cs` → BD → `cMovimento.cs` → `cControlo.cs` → `frmPagamentosServicos.cs` → `Form1.cs`

---
#### Camada 1 — Apresentação (`Form1.cs` → `btnPagamentos_Click`)
Abre `frmPagamentosServicos` com `ShowDialog`. Quando fecha com `DialogResult.OK`, lê `frm.NovoSaldo` e refresca o ecrã.

---
#### Camada 1b — Apresentação (`frmPagamentosServicos.cs`)
No construtor, chama `fListarServicos` — carrega da tabela `Servicos` a lista de serviços pré-definidos. Preenche a listbox com o nome de cada serviço. Ao mudar a seleção, atualiza o label da entidade automaticamente.

Ao clicar "Pagar":
1. Verifica se há serviço selecionado (estado de UI).
2. Valida o **formato** do valor — verifica apenas se é número parseável (`decimal.TryParse`). As regras de negócio (valor > 0 e referência com 9 dígitos) ficam no `cControlo.fPagamento`.
3. Constrói a descrição: `"Nome | Ent X Ref 123456789"`.
4. Chama `cControlo.fPagamento` com `contaId`, valor, descrição e referência.
5. Se `operacao=true`, guarda `NovoSaldo` e fecha com `DialogResult.OK`.

---
#### Camada 2 — Controlo (`cControlo.cs`)
**`fListarServicos`** — carrega `SELECT * FROM Servicos`. Resultado em `listaServicos`.

**`fPagamento`** — recebe `contaId`, `valor`, `descricao` e `referencia`. É aqui que ficam **todas as regras de negócio**: referência com 9 dígitos e valor > 0. O formulário só valida formato. Depois funciona como o Levantamento: verifica saldo, debita e regista movimento com tipo `"P"`.

---
#### Camada 3 — Dados (`cMovimento.cs`)
Mesmos métodos do Levantamento. Tipo `"P"`, descrição com nome do serviço, entidade e referência.

---
#### Fluxo resumido

```
Form1.btnPagamentos_Click
  └─ abre frmPagamentosServicos(contaId)
        │  construtor: fListarServicos → preenche listbox
        │
        └─ btnPagar_Click
              ├─ validar serviço selecionado
              ├─ validar referência (9 dígitos)
              ├─ validar valor > 0
              └─ cControlo.fPagamento(contaId, valor, descricao)
                    ├─ fObterSaldo       → Q1: SELECT Saldo
                    ├─ verificar saldo suficiente
                    ├─ fAtualizarSaldo   → Q2: UPDATE Saldo
                    └─ fInserirMovimento("P", descricao) → Q3: INSERT
              │
              ├─ false → MessageBox erro
              └─ true  → NovoSaldo = saldoAtualizado; DialogResult.OK
  │
  └─ Form1 lê frm.NovoSaldo → fRefrescarEcra
```

---

### 3.9 Simulador de Empréstimo

**Ficheiros:** `frmMultibanco.cs` (apenas Camada 1 — sem BD)

O utilizador introduz capital, taxa anual e prazo em meses. O cálculo é feito localmente com a fórmula PMT — sem ir à BD, sem `cControlo`, sem `cMovimento`.

```
prestacao = capital × (taxa / 12) / (1 - (1 + taxa/12)^(-meses))
```

---
### 3.10 Relógio em tempo real

**Ficheiros:** `Form1.cs` (apenas Camada 1 — sem BD)

Um `Timer` configurado com `Interval = 1000` (1 segundo) corre desde que o `frmMultibanco` abre. A cada tick (`tmrRelogio_Tick`) atualiza um label com `DateTime.Now` no formato `dd/MM/yyyy HH:mm:ss`. Não envolve nenhuma outra camada.

---
### 3.11 Bloqueio de conta

Não é uma funcionalidade com botão próprio — é um comportamento automático despoletado pelo Login (ver 3.1 Q4).

- Após 3 tentativas falhadas, a coluna `Bloqueada` passa a `TRUE` na BD via `CASE WHEN` — numa única query, sem ler e voltar a escrever.
- Persiste ao fechar a app — está na BD, não em memória.
- Bloqueio por conta, não por utilizador — a conta 123456 bloqueada não afeta a 123457.
- Login bem-sucedido reseta `Tentativas=0` automaticamente (Q3 do Login).
- Mensagem distinta consoante o motivo: "Credenciais inválidas" vs "Conta bloqueada após 3 tentativas".
- Desbloqueio: só o admin sibs via BackOffice (ver 4.6).
