# 4. Funcionalidades BackOffice SIBS

| # | Funcionalidade |
|---|----------------|
| 4.1 | Login Admin |
| 4.2 | Listar Clientes |
| 4.3 | Inserir Cliente |
| 4.4 | Eliminar Cliente |
| 4.5 | Ativar / Desativar MBWay |
| 4.6 | Desbloquear Conta |

---

### 4.1 Login Admin

**Ficheiros:** `frmAutenticacao.cs` → `cControlo.cs` → `cLogin.cs` → BD (tabela Admins) → `cLogin.cs` → `cControlo.cs` → `frmAutenticacao.cs` → `frmAdmin.cs`

**Camada 1 — Apresentação (`frmAutenticacao.cs`):**
O mesmo ecrã de login serve clientes normais e o administrador. Quando o utilizador clica em "OK", o formulário verifica se o campo Cliente contém o valor `sibs` (sem distinção de maiúsculas). Se sim, segue o ramo admin: chama `fTestarCamposAdmin` passando apenas Cliente e PIN (Banco e Conta são ignorados — o admin não tem conta bancária). Se os campos estiverem preenchidos, chama `fValidarAdmin` com os mesmos valores. Quando `operacao` e `existe` voltam a `true`, o formulário esconde-se, abre o `frmAdmin` em modo `ShowDialog` e fecha-se assim que o admin terminar a sessão.

**Camada 2 — Controlo (`cControlo.cs`):**
`fTestarCamposAdmin` recebe Cliente e PIN e verifica se algum está vazio. Se estiver, define `operacao = false` e preenche `mensagem`. `fValidarAdmin` cria uma instância de `cLogin`, delega a verificação na BD e copia `operacao`, `existe` e `mensagem` para as suas próprias variáveis públicas — o formulário só lê o `cControlo`.

**Camada 3 — Dados (`cLogin.cs`):**
`fValidarAdmin` executa `SELECT Id FROM Admins WHERE Username = @Username AND Pin = @Pin`. Se a query devolver pelo menos uma linha (`HasRows = true`), define `existe = true` e `operacao = true`. Se não encontrar correspondência, define `existe = false` e `mensagem = "Credenciais de administrador inválidas."`. A tabela `Admins` é separada de `Credenciais` porque o admin não é um cliente com conta bancária — são entidades distintas.

**Fluxo resumido:**
```
frmAutenticacao       cControlo           cLogin              BD
      |                    |                   |               |
      |-- fTestarCamposAdmin -->              |               |
      |<-- operacao/mensagem ---              |               |
      |                    |                   |               |
      |-- fValidarAdmin -->|                   |               |
      |                    |-- fValidarAdmin -->|               |
      |                    |                   |-- SELECT Admins -->|
      |                    |                   |<-- HasRows --------|
      |                    |<-- operacao/existe/mensagem --|        |
      |<-- operacao/existe/mensagem --|        |           |        |
      |                    |                   |           |        |
      |-- (se ok) ShowDialog(frmAdmin) ------->|
```

---

### 4.2 Listar Clientes

**Ficheiros:** `frmAdmin.cs` → `cControlo.cs` → `cAdmin.cs` → BD (tabela Credenciais) → `cAdmin.cs` → `cControlo.cs` → `frmAdmin.cs`

**Camada 1 — Apresentação (`frmAdmin.cs`):**
Quando o `frmAdmin` abre, o evento `frmAdmin_Load` adiciona a coluna "Bloqueada" ao `ListView` (em código, não no Designer, para não ser apagada ao guardar) e invoca imediatamente `fCarregarLista`. Este método auxiliar cria um `cControlo`, chama `fListarCredenciais` e percorre `listaCredenciais` para preencher o `ListView` — uma linha por conta. Cada linha tem sete colunas: Id, Banco, Cliente, Conta, Saldo, MBWay e Bloqueada. A coluna Bloqueada permite ao admin identificar visualmente quais contas precisam de ser desbloqueadas. No rodapé é atualizado o contador de contas (`lblTotal`). O mesmo `fCarregarLista` é chamado no final de cada operação bem-sucedida para manter a lista atualizada.

**Camada 2 — Controlo (`cControlo.cs`):**
`fListarCredenciais` cria uma instância de `cAdmin`, delega a consulta e copia `operacao`, `mensagem` e `listaCredenciais` para as suas próprias variáveis públicas. O formulário lê `listaCredenciais` diretamente a partir do `cControlo`.

**Camada 3 — Dados (`cAdmin.cs`):**
`fListarCredenciais` limpa a lista anterior e executa `SELECT Id, Banco, Cliente, Conta, Saldo, MBWay, Bloqueada FROM Credenciais ORDER BY Cliente, Conta`. Para cada linha devolvida, cria um array de sete strings: Id e Conta são convertidos de inteiro para string; Saldo é formatado com duas casas decimais; MBWay e Bloqueada (booleanos na BD) são traduzidos para `"Sim"` ou `"Não"`. No final, a `mensagem` indica quantas contas foram encontradas.

**Fluxo resumido:**
```
frmAdmin              cControlo           cAdmin              BD
   |                      |                   |               |
   |-- fListarCredenciais -->                 |               |
   |                      |-- fListarCredenciais -->          |
   |                      |                   |-- SELECT Credenciais -->|
   |                      |                   |<-- linhas -------------|
   |                      |<-- listaCredenciais + operacao --|
   |<-- listaCredenciais --|                  |              |
   |-- preenche ListView ->|
```

---

### 4.3 Inserir Cliente

**Ficheiros:** `frmAdmin.cs` → `cControlo.cs` → `cAdmin.cs` → BD (tabela Credenciais) → `cAdmin.cs` → `cControlo.cs` → `frmAdmin.cs`

**Camada 1 — Apresentação (`frmAdmin.cs`):**
O admin preenche quatro campos no ecrã: Banco, Cliente, Conta e PIN. Ao clicar "Inserir", o formulário chama `fTestarCampos` com esses quatro valores para verificar se estão todos preenchidos. Se algum estiver vazio, a operação para e é mostrado um erro. Se estiverem todos preenchidos, chama `fInserirCliente` com os mesmos valores. Se `operacao` voltar a `true`, os campos são limpos e `fMostrarResultadoEAtualizar` mostra a mensagem de sucesso e recarrega a lista.

**Camada 2 — Controlo (`cControlo.cs`):**
`fTestarCampos` verifica se algum dos quatro campos está vazio. `fInserirCliente` cria um `cAdmin`, passa os quatro valores e copia `operacao` e `mensagem` de volta. O saldo inicial de 100€ não é um parâmetro — é uma regra definida na camada de dados.

**Camada 3 — Dados (`cAdmin.cs`):**
`fInserirCredencial` converte Conta e PIN para inteiro, define o Saldo fixo em `100.00€` e executa `INSERT INTO Credenciais (Banco, Cliente, Conta, Pin, Saldo) VALUES (@Banco, @Cliente, @Conta, @Pin, @Saldo)`. Se o número de conta já existir (restrição UNIQUE da BD), a exceção é capturada, `operacao` fica `false` e a mensagem inclui o erro de BD.

**Fluxo resumido:**
```
frmAdmin              cControlo           cAdmin              BD
   |                      |                   |               |
   |-- fTestarCampos ----->|                  |               |
   |<-- operacao/mensagem -|                  |               |
   |                      |                   |               |
   |-- fInserirCliente --->|                  |               |
   |                      |-- fInserirCredencial -->          |
   |                      |                   |-- INSERT Credenciais (saldo=100€) -->|
   |                      |                   |<-- ok / UNIQUE error -----------------|
   |                      |<-- operacao/mensagem --|          |
   |<-- operacao/mensagem -|                  |               |
   |-- (se ok) limpa campos + atualiza lista ->
```

---

### 4.4 Eliminar Cliente

**Ficheiros:** `frmAdmin.cs` → `cControlo.cs` → `cMovimento.cs` + `cAdmin.cs` → BD (tabela Credenciais) → volta pelo mesmo caminho → `frmAdmin.cs`

**Camada 1 — Apresentação (`frmAdmin.cs`):**
O admin seleciona uma linha no `ListView` e clica "Eliminar". O formulário verifica se há uma linha selecionada; se não houver, avisa e para. Se houver, lê o Id (coluna 0) e o nome do cliente (coluna 2) da linha selecionada e pede confirmação com `MessageBox.Show`. Se o admin confirmar, chama `fEliminarCliente(id)`. O resultado é tratado por `fMostrarResultadoEAtualizar`, que mostra sucesso ou erro e recarrega a lista.

**Camada 2 — Controlo (`cControlo.cs`):**
`fEliminarCliente` tem a regra de negócio: antes de apagar, usa `cMovimento.fObterSaldo` para ler o saldo atual. Se o saldo for diferente de zero (positivo ou negativo), bloqueia a operação e devolve uma mensagem com o valor exato. Só quando `saldo == 0` é que chama `cAdmin.fEliminarCredencial` para apagar o registo.

**Camada 3 — Dados (`cMovimento.cs` e `cAdmin.cs`):**
`cMovimento.fObterSaldo` executa `SELECT Saldo FROM Credenciais WHERE Id = @Id` e devolve o valor. `cAdmin.fEliminarCredencial` executa `DELETE FROM Credenciais WHERE Id = @Id`. A consulta do saldo foi propositadamente mantida em `cMovimento` para não duplicar o mesmo SQL em `cAdmin`.

**Fluxo resumido:**
```
frmAdmin              cControlo           cMovimento / cAdmin  BD
   |                      |                   |               |
   |-- (confirmar?) ------>
   |-- fEliminarCliente(id) -->               |               |
   |                      |-- fObterSaldo(id) -->             |
   |                      |                   |-- SELECT Saldo -->|
   |                      |                   |<-- saldo ---------|
   |                      |-- (saldo ≠ 0) → erro              |
   |                      |-- (saldo = 0) → fEliminarCredencial -->|
   |                      |                   |-- DELETE Credenciais -->|
   |                      |<-- operacao/mensagem --|          |
   |<-- operacao/mensagem -|                  |               |
```

---

### 4.5 Ativar / Desativar MBWay

**Ficheiros:** `frmAdmin.cs` → `cControlo.cs` → `cAdmin.cs` → BD (tabela Credenciais) → `cAdmin.cs` → `cControlo.cs` → `frmAdmin.cs`

**Camada 1 — Apresentação (`frmAdmin.cs`):**
O admin seleciona uma linha e clica "Ativar / Desativar MBWay". O formulário verifica se há seleção, lê o Id (coluna 0) e o estado atual do MBWay (coluna 5: `"Sim"` ou `"Não"`). Calcula o novo estado invertendo o atual (`novoEstado = !estadoAtual`). Pede confirmação indicando a ação exata ("ativar" ou "desativar"). Se confirmado, chama `fAlternarMBWay(id, novoEstado)`. O resultado é tratado por `fMostrarResultadoEAtualizar`.

**Camada 2 — Controlo (`cControlo.cs`):**
`fAlternarMBWay` cria um `cAdmin`, passa o Id e o novo estado booleano e copia `operacao` e `mensagem` de volta. Não há validação de negócio aqui — o toggle é sempre permitido.

**Camada 3 — Dados (`cAdmin.cs`):**
`fAlternarMBWay` executa `UPDATE Credenciais SET MBWay = @MBWay WHERE Id = @Id`. O valor de `@MBWay` é o booleano `novoEstado` que vem do formulário. Após o UPDATE bem-sucedido, a mensagem indica se foi ativado ou desativado. O efeito é imediato: quando a lista for recarregada, a coluna MBWay reflete o novo estado.

**Fluxo resumido:**
```
frmAdmin              cControlo           cAdmin              BD
   |                      |                   |               |
   |-- lê estado atual da linha selecionada   |               |
   |-- calcula novoEstado = !estadoAtual       |               |
   |-- (confirmar?) ------>                   |               |
   |-- fAlternarMBWay(id, novoEstado) ------->|               |
   |                      |-- fAlternarMBWay -->              |
   |                      |                   |-- UPDATE MBWay ->|
   |                      |                   |<-- ok ------------|
   |                      |<-- operacao/mensagem --|           |
   |<-- operacao/mensagem -|                  |               |
   |-- atualiza lista ---->
```

---

### 4.6 Desbloquear Conta

**Ficheiros:** `frmAdmin.cs` → `cControlo.cs` → `cAdmin.cs` → BD (tabela Credenciais) → `cAdmin.cs` → `cControlo.cs` → `frmAdmin.cs`

**Camada 1 — Apresentação (`frmAdmin.cs`):**
Quando uma conta fica bloqueada após três tentativas de PIN erradas, só o administrador pode desbloquear. A coluna "Bloqueada" na lista permite ao admin identificar visualmente qual conta está bloqueada ("Sim") antes de agir. O admin seleciona a linha e clica "Desbloquear". O formulário verifica se há seleção, lê o Id (coluna 0) e chama `fDesbloquearConta(id)`. O resultado é tratado por `fMostrarResultadoEAtualizar`. Não há pedido de confirmação nesta operação — desbloquear é sempre reversível.

**Camada 2 — Controlo (`cControlo.cs`):**
`fDesbloquearConta` cria um `cAdmin`, passa o Id e copia `operacao` e `mensagem` de volta. Não há validação de negócio — o UPDATE é sempre executado, mesmo que a conta já estivesse desbloqueada (o resultado é o mesmo: Tentativas = 0, Bloqueada = FALSE).

**Camada 3 — Dados (`cAdmin.cs`):**
`fDesbloquearConta` executa `UPDATE Credenciais SET Tentativas = 0, Bloqueada = FALSE WHERE Id = @Id`. O UPDATE é idempotente — correr numa conta já desbloqueada não causa erro nem efeito indesejado.

**Fluxo resumido:**
```
frmAdmin              cControlo           cAdmin              BD
   |                      |                   |               |
   |-- fDesbloquearConta(id) -->              |               |
   |                      |-- fDesbloquearConta -->           |
   |                      |                   |-- UPDATE Tentativas=0, Bloqueada=FALSE -->|
   |                      |                   |<-- ok -----------------------------------|
   |                      |<-- operacao/mensagem --|          |
   |<-- operacao/mensagem -|                  |               |
   |-- atualiza lista ---->
```
