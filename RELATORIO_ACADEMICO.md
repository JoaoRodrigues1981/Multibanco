# Sistema Multibanco Simulado — Relatório Académico

| | |
|---|---|
| **Disciplina** | Algoritmos e Estruturas de Dados — Arquitetura e Estrutura de Dados |
| **Curso** | Licenciatura em Engenharia Informática |
| **Ano / Semestre** | 1.º Ano / 2.º Semestre — 2025/2026 |
| **Autor** | João Rodrigues |
| **Data de entrega** | 17 de Junho de 2026 |
| **Tecnologias** | C# · .NET 8 · Windows Forms · PostgreSQL 18 · Npgsql |

---

## Índice

1. [Introdução e Âmbito](#1-introdução-e-âmbito)
2. [Requisitos e Cobertura](#2-requisitos-e-cobertura)
3. [Arquitetura da Solução](#3-arquitetura-da-solução)
4. [Modelo de Dados](#4-modelo-de-dados)
5. [Funcionalidades do Sistema](#5-funcionalidades-do-sistema)
6. [Implementação por Camadas](#6-implementação-por-camadas)
7. [Princípios de Engenharia de Software Aplicados](#7-princípios-de-engenharia-de-software-aplicados)
8. [Decisões Técnicas Justificadas](#8-decisões-técnicas-justificadas)
9. [Limitações e Evoluções](#9-limitações-e-evoluções)
10. [Conclusão](#10-conclusão)
- [Anexo A — Estrutura de Ficheiros](#anexo-a--estrutura-de-ficheiros)
- [Anexo B — Esquema da Base de Dados](#anexo-b--esquema-da-base-de-dados)
- [Anexo C — Interface Visual (com assistência de IA)](#anexo-c--interface-visual-com-assistência-de-ia)

---

## 1. Introdução e Âmbito

O projeto implementa um sistema ATM/Multibanco simulado com interface gráfica em Windows Forms. A aplicação suporta dois perfis distintos: 
1. **cliente bancário**, que realiza operações sobre a sua conta
2. **administrador "sibs"**, que gere contas e acessos da plataforma.

O sistema foi desenvolvido de raiz sobre a estrutura base desenvolvida durante as aulas da disciplina, respeitando a arquitetura em três camadas como requisito técnico central.

> **Nota sobre SGBD:** o enunciado especifica SQL Server. Foi utilizado PostgreSQL 18, já disponível na minha máquina de desenvolvimento. A sintaxe SQL, os tipos de dados e a estrutura de queries são equivalentes para este projeto. A migração para SQL Server implicaria alterar apenas a connection string e substituir o driver `Npgsql` por `System.Data.SqlClient`, a lógica aplicacional permaneceria inalterada. Esta opção foi previamente acordada com o docente da disciplina


## 2. Requisitos

### 2.1 Funcionalidades Obrigatórias

De acordo com o enunciado do trabalho, partilhado no moodle (https://moodle.ensinolusofona.pt/pluginfile.php/211982/mod_resource/content/2/2025-26-Projeto%20Final.pdf), foram enumeradas as seguintes funcionalidades obrigatórias:

| #   | Requisito (enunciado)                             | Estado | Onde                                             |
| --- | ------------------------------------------------- | ------ | ------------------------------------------------ |
| 1   | Administrador "sibs" com CRUD de credenciais      | ✅      | `frmAdmin` + `cAdmin`                            |
| 2   | Saldo inicial de 100€ em contas criadas pelo sibs | ✅      | `cAdmin.fInserirCredencial`                      |
| 3   | Bloqueio de eliminação se saldo ≠ 0               | ✅      | `cControlo.fEliminarCliente`                     |
| 4   | Consulta de saldo ao entrar                       | ✅      | `frmMultibanco_Load`                             |
| 5   | Levantamento com validação de saldo               | ✅      | `cControlo.fLevantar`                            |
| 6   | Depósito                                          | ✅      | `cControlo.fDepositar`                           |
| 7   | Transferência entre contas                        | ✅      | `cControlo.fTransferir`                          |
| 8   | MBWay (conta destino tem de ser aderente)         | ✅      | `cControlo.fMBWay`                               |
| 9   | Pagamentos de serviços pré-definidos              | ✅      | `frmPagamentosServicos` + tabela `Servicos`      |
| 10  | Histórico com filtro por intervalo de datas       | ✅      | `frmMultibanco` — DateTimePicker De/Até          |
| 11  | Registo imediato na BD e na UI                    | ✅      | `fInserirMovimento` + `fRefrescarEcra`           |
| 12  | Movimentos imutáveis (só INSERT)                  | ✅      | `cMovimento` — sem UPDATE/DELETE em `Movimentos` |
| 13  | Validações de entrada com mensagens claras        | ✅      | Camada de Controlo + `KeyPress` na Apresentação  |
### 2.2 Funcionalidades Extra (criatividade)

Também de acordo com o mesmo enunciado, existia espaço para se adicionar funcionalidades extra, o que levou ao desenvolvimento das seguintes funcionalidades:

| #   | Funcionalidade                                           | Justificação                                                                                                                |
| --- | -------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| E1  | Configuração por conta, de MBWay pelo sibs no BackOffice | O enunciado define a regra de adesão mas não o mecanismo de ativação. Atribuído ao administrador por coerência operacional. |
| E2  | Simulador de empréstimo (Indexado vs Taxa Fixa)          | Funcionalidade standalone sem BD; implementa a fórmula financeira PMT equivalente ao Excel.                                 |

## 3. Arquitetura da Solução
O projeto respeita estritamente a arquitetura em **três camadas**, com a regra de comunicação unidirecional descendente: cada camada comunica apenas com a camada imediatamente abaixo. Esta arquitetura é o ponto central de todo o trabalho, sobre qual recai o maior objetivo da disciplina, e consequentemente deste projeto.

Assim, temos o nosso projeto estruturado da seguinte forma:

```
┌────────────────────────────────────────┐
│ _01Apresentacao  (Formulários WinForms)│  ← o utilizador vê e clica
├────────────────────────────────────────┤
│ _02Controlo      (cControlo.cs)        │  ← regras de negócio
├────────────────────────────────────────┤
│ _03Dados         (cConexao, cLogin,    │  ← SQL e ligação à BD
│                   cAdmin, cMovimento)  │
└────────────────────────────────────────┘
         ↓ PostgreSQL 18 (base de dados)
```

**Consequência prática desta arquitectura:** a camada de dados pode ser integralmente substituída (ex: migrar para SQL Server, ou para ficheiros JSON) sem tocar nos formulários nem nas regras de negócio. Os formulários nunca contêm SQL. As classes de dados nunca contêm validações de negócio.

_01Apresentacao_:  São os ecrãs que o utilizador vê e clica. Cada ficheiro é uma janela:
- frmAutenticacao - Ecrã de login, corresponde ao ecrã de entrada na aplicação
- frmMultibanco - Ecrã principal, onde se realizam várias ações, como levantamentos, depósitos, etc. Dá também acesso a ecrã específicos de outras ações.
- frmAdmin - BackOffice do admin "sibs" 
- frmUpdate - Ecrã de alteração de PIN
- frmPagamentosServicos - Lista de serviços para pagar
- frmSimuladorEmprestimo - Calculadora de empréstimo
- Tema.cs  - Não é um ecrã, define apenas as cores de toda a app (totalmente produzida por AI). Simples de alterar se pretendermos outro tipo de cores.

_02Controlo_: Contém todas as regras. Os ecrãs chamam métodos daqui. Esta classe nunca desenha nada no ecrã nem fala diretamente com a BD. É um único ficheiro: cControlo.cs

_03Dados_:  São as classes que falam com o PostgreSQL. Estas classes não decidem nada, limitam-se a receber uma instrução, executar o SQL, devolver o resultado. Cada uma tem uma responsabilidade:
- cConexao.cs - Abre e fecha a ligação à base de dados
- cLogin.cs - Verifica se o utilizador existe e se o PIN está certo
- cAdmin.cs - Gere contas (inserir, eliminar, ativar MBWay)
- cMovimento.cs - Regista movimentos, atualiza saldos, lista histórico
- cServico.cs - Lista os serviços disponíveis para a funcionalidade de Pagamentos de Serviços
### Fluxo de controlo — login e navegação

```
Program.cs → frmAutenticacao
                │
                ├─► txtCliente == "sibs"?
                │       SIM → fTestarCamposAdmin() → fValidarAdmin() → frmAdmin
                │       NÃO → fTestarCampos() → fValidarCredenciais() → frmMultibanco
                │
                └─► [Alterar PIN] → fValidarCredenciais() → frmUpdate
```

A distinção de perfis é feita **antes** da validação, porque o sibs não tem `Banco` nem `Conta` — obrigá-lo a preencher esses campos seria um erro de usabilidade. Um único ponto de entrada serve dois perfis sem duplicar o ecrã de login.

---

## 4. Modelo de Dados

### 4.1 Diagrama de entidades
O nosso modelo relacional é na verdade bastante simples, servindo apenas para garantir as funcionalidades descritas anteriormente. Não tivemos preocupação de normalizar a base dados, nem criar procedimentos, triggers ou permissões/segurança.
#### Admins
 Tabela que identifica os utilizadores com privilegio de administrador. O admin "sibs" não é um cliente , este user não tem conta bancária, não tem saldo, não faz levantamentos. Se estivesse em Credenciais teríamos de inventar valores falsos para Banco, Conta, Saldo, etc.

| Coluna   | Tipo         | O que guarda       | Exemplo  |
| -------- | ------------ | ------------------ | -------- |
| ID       | Inteiro (PK) | Identificado único | 1,2,3... |
| Username | Texto        | Nome de utilizador | "sibs"   |
| Pin      | Número       | PIN de acesso      | 9999     |
#### Credenciais
É a nossa tabela principal, onde temos os nossos clientes e respetivas contas. Cada linha representa uma conta bancária de um cliente.
Não colocamos o Conta como ID da tabela, pois entendemos que Conta é o número que o cliente conhece (como o IBAN). O Id é interno, será o valor que o programa usará para
tudo, e assim além de ser mais seguro, garantimos que nunca muda.

| Coluna  | Tipo         | O que guarda                              | Exemplo  |
| ------- | ------------ | ----------------------------------------- | -------- |
| ID      | Inteiro (PK) | Identificado único                        | 1,2,3... |
| Banco   | Texto        | Nome do banco                             | BPI      |
| Cliente | Texto        | Nome do cliente                           | Maria    |
| Conta   | Número       | Número da conta                           | 12345678 |
| Pin     | Número       | Pin de acesso à aplicação                 | 1234     |
| Saldo   | Decimal      | O saldo atual do cliente nessa conta      | 250.00   |
| MBWay   | Booleano     | Define se a conta aceita movimentos MBWay | true     |

#### Movimentos
Esta tabela tem a função de registar o histórico de movimentos de um cliente/conta. Cada vez que há uma operação (levantamento, depósito, etc.), é criada uma linha nesta tabela.

| Coluna    | Tipo         | O que guarda                         | Exemplo                                                                                                        |
| --------- | ------------ | ------------------------------------ | -------------------------------------------------------------------------------------------------------------- |
| ID        | Inteiro (PK) | Identificado único                   | 1,2,3...                                                                                                       |
| ContaId   | Número (FK)  | A qual conta este movimento pertence | 1                                                                                                              |
| Tipo      | Texto        | Define o tipo de operação            | "L" - levantamento, <br>"D" - depósito,<br>"T" - transferência,<br>"P" - pagamento de serviços,<br>"M" - MBWay |
| Valor     | Decimal      | Valor movimentado                    | 10.00                                                                                                          |
| SaldoApos | Decimal      | Saldo final após a operação          | 240.00                                                                                                         |
| DataHora  | Data/Hora    | Data e hora do movimento             | 2026-06-08 14:32                                                                                               |
| Descricao | Texto        | Descrição informativa                | "Levantamento"                                                                                                 |

#### Servicos

A tabela `Servicos` é independente, não tendo relação de chave estrangeira com as restantes. Um pagamento de serviço é registado em `Movimentos` como qualquer outro movimento do tipo `'P'`; a referência ao serviço fica codificada no campo `Descricao` (ex: `"EDP — Electricidade | Ent 10001 Ref 123456789"`). Não existe FK porque o movimento não precisa de saber qual o serviço após ser registado, a informação descritiva é suficiente para auditoria.

| Coluna   | Tipo         | O que guarda       | Exemplo               |
| -------- | ------------ | ------------------ | --------------------- |
| ID       | Inteiro (PK) | Identificado único | 1,2,3...              |
| Nome     | Texto        | Nome do serviço    | "EDP — Electricidade" |
| Entidade | Número       | Código da entidade | 10001                 |

### 4.2 Decisões de desenho do modelo

**`Admins` separada de `Credenciais`:** o administrador não é um cliente bancário. Misturá-los na mesma tabela obrigaria a valores nulos em `Banco`, `Conta` e `Saldo` para o registo do sibs — violaria a integridade semântica da tabela.

**`Credenciais` mantida como base de aula:** a estrutura foi apenas estendida com duas colunas (`Saldo`, `MBWay`). Reescrever a tabela implicaria reescrever o código de login sem ganho funcional. Pragmatismo de engenharia: não refatorar o que funciona.

**`SaldoApos` desnormalizado em `Movimentos`:** em vez de recalcular o saldo histórico somando todos os movimentos, cada linha de movimento guarda o saldo exato no momento. O histórico é auto-suficiente e sempre consistente, mesmo que a coluna `Saldo` em `Credenciais` fosse corrompida.

**`Movimentos` imutável:** só INSERT, nunca UPDATE nem DELETE. Regra explícita do enunciado; alinha com práticas de audit trail em sistemas bancários reais.

**Tipos `CHAR(1)` para `Tipo`:** os valores possíveis são apenas 5 (`D`, `L`, `T`, `P`, `M`). Um `CHAR(1)` é mais eficiente e legível do que um `VARCHAR`. Uma constraint `CHECK` garante que nenhum valor inválido é inserido.

**`Conta` com `UNIQUE`:** o número de conta visível é o identificador que o utilizador introduz nas transferências. Deve ser único para que `WHERE Conta = @Conta` devolva no máximo uma linha.

**`Servicos` sem FK para `Movimentos`:** os serviços pré-definidos são apenas um catálogo de leitura. O vínculo entre um pagamento e o serviço correspondente é preservado no campo `Descricao` do movimento, não numa chave estrangeira. Isto permite eliminar ou alterar serviços no catálogo sem invalidar o histórico de movimentos já registados.

### 4.3 Connection string

```csharp
"Host=localhost;Database=lei2526;Username=postgres;Password=1234;Search Path=public"
```

O parâmetro `Search Path=public` é obrigatório no PostgreSQL 15+ — sem ele, o driver não resolve os nomes das tabelas no schema `public` e devolve `42P01: relation does not exist`. Em SQL Server este parâmetro não existe (o schema padrão é `dbo`).

---

## 5. Funcionalidades do Sistema

Esta secção descreve cada funcionalidade em dois níveis: primeiro o que o utilizador vê e faz (**fluxo funcional**), depois o que acontece internamente nas 3 camadas (**fluxo técnico**).

---

### 5.1 Autenticação

#### Fluxo funcional
1. O utilizador abre a aplicação e vê o ecrã de login com 4 campos: Banco, Cliente, Conta, PIN
2. Se for o administrador, preenche apenas Cliente ("sibs") e PIN
3. Clica no botão "Entrar"
4. Se as credenciais estiverem erradas, recebe mensagem de erro e permanece no ecrã
5. Se estiverem corretas, é encaminhado para o ecrã principal (cliente) ou BackOffice (sibs)

#### Fluxo técnico

```
frmAutenticacao.btnOK_Click
  ├─ txtCliente == "sibs"?
  │     SIM → cControlo.fTestarCamposAdmin (valida Cliente + PIN)
  │         → cControlo.fValidarAdmin → cLogin.fValidarAdmin
  │         → SELECT Id FROM Admins WHERE Username=@U AND Pin=@P
  │         → abre frmAdmin
  │
  └─ NÃO → cControlo.fTestarCampos (valida 4 campos)
          → cControlo.fValidarCredenciais → cLogin.fObterCredenciais
          → SELECT Id, Saldo, Banco, Cliente, Conta FROM Credenciais WHERE ...
          → abre frmMultibanco(contaId, saldo, banco, cliente, conta)
             └─ frmMultibanco_Load → fCarregarMovimentos → SELECT Movimentos
```

**Nota técnica:** `cControlo` é instanciado dentro do handler de clique (ou seja, no código do botão), não como campo da classe. Garante estado limpo (`operacao=true`) em cada tentativa, e assim um login falhado não condiciona a tentativa seguinte.

---
### 5.2 Levantamento

#### Fluxo funcional
1. O utilizador introduz o valor no campo "Montante (€)"
2. Clica no botão "Levantar"
3. Se o campo estiver vazio ou não for um número, recebe mensagem de erro
4. Se o saldo for insuficiente, recebe mensagem com o saldo disponível
5. Se correr bem, o saldo atualiza no ecrã e o movimento aparece no histórico

#### Fluxo técnico

```
frmMultibanco.btnLevantar_Click
  └─ fValidarValor → é número e > 0?
       ↓
  cControlo.fLevantar(contaId, valor)
    ├─ cMovimento.fObterSaldo       → SELECT Saldo FROM Credenciais WHERE Id=@Id
    ├─ fSaldoSuficiente             → saldo >= valor?
    ├─ novoSaldo = saldo - valor
    ├─ cMovimento.fAtualizarSaldo   → UPDATE Credenciais SET Saldo=@novoSaldo WHERE Id=@Id
    └─ cMovimento.fInserirMovimento → INSERT INTO Movimentos (tipo='L', ...)
       ↓
  fMostrarResultado → MessageBox "Sucesso" + fRefrescarEcra(novoSaldo)
```

---
### 5.3 Depósito

#### Fluxo funcional
1. O utilizador introduz o valor no campo "Montante (€)"
2. Clica "Depositar"
3. Se o campo for inválido, recebe erro
4. Se correr bem, o saldo aumenta no ecrã e o movimento aparece no histórico

#### Fluxo técnico

```
frmMultibanco.btnDepositar_Click
  └─ fValidarValor → é número e > 0?
       ↓
  cControlo.fDepositar(contaId, valor)
    ├─ cMovimento.fObterSaldo       → SELECT Saldo
    ├─ novoSaldo = saldo + valor    (sem validação de saldo mínimo — depósito nunca falha por saldo)
    ├─ cMovimento.fAtualizarSaldo   → UPDATE Credenciais SET Saldo=@novoSaldo
    └─ cMovimento.fInserirMovimento → INSERT INTO Movimentos (tipo='D', ...)
       ↓
  fMostrarResultado → MessageBox "Sucesso" + fRefrescarEcra(novoSaldo)
```

---

### 5.4 Transferência

#### Fluxo funcional
1. O utilizador introduz o valor e o número de conta destino
2. Clica "Transferir"
3. Se a conta destino não existir, recebe erro
4. Se tentar transferir para a própria conta, recebe erro
5. Se o saldo for insuficiente, recebe erro
6. Se correr bem, o saldo da conta origem diminui, o da destino aumenta, e ambas ficam com registo no histórico

#### Fluxo técnico

```
frmMultibanco.btnTransferir_Click
  ├─ fValidarValor
  └─ fValidarContaDestino → é número inteiro?
       ↓
  cControlo.fTransferir(contaOrigem, contaNumDest, valor)
    ├─ cMovimento.fObterContaDestino → SELECT Id, Saldo FROM Credenciais WHERE Conta=@Conta
    │                                  (resolve número visível → Id interno + saldo destino)
    ├─ contaOrigem == idDestino?     → erro "contas iguais"
    ├─ cMovimento.fObterSaldo(origem)
    ├─ fSaldoSuficiente
    ├─ cMovimento.fAtualizarSaldo(origem, saldo-valor)
    ├─ cMovimento.fAtualizarSaldo(destino, saldo+valor)
    ├─ cMovimento.fInserirMovimento(origem, 'T', "Transferência para conta X")
    └─ cMovimento.fInserirMovimento(destino, 'T', "Transferência de conta X")
       ↓
  fMostrarResultado → MessageBox + fRefrescarEcra(novoSaldoOrigem)
```

**Nota:** cada transferência gera 2 movimentos — débito na origem e crédito no destino. Cada cliente vê o histórico completo da sua perspetiva.

---

### 5.5 MBWay

#### Fluxo funcional
1. O utilizador introduz o valor e o número de conta destino
2. Clica "MBWay"
3. Se a conta destino não tiver MBWay ativo, recebe erro
4. Se o saldo for insuficiente, recebe erro
5. Se correr bem, comporta-se como uma transferência mas com tipo 'M' no histórico

#### Fluxo técnico

```
frmMultibanco.btnMBWay_Click
  ├─ fValidarValor
  └─ fValidarContaDestino
       ↓
  cControlo.fMBWay(contaOrigem, contaNumDest, valor)
    ├─ cMovimento.fObterContaDestino → resolve conta → Id + saldo destino
    ├─ cMovimento.fObterMBWay(idDestino) → SELECT MBWay FROM Credenciais WHERE Id=@Id
    │                                       MBWay == true?
    ├─ cMovimento.fObterSaldo(contaOrigem)
    ├─ fSaldoSuficiente
    ├─ 2× fAtualizarSaldo + 2× fInserirMovimento (tipo='M')
       ↓
  fMostrarResultado
```

**Diferença face à transferência:** validação adicional `fObterMBWay` — a conta destino tem de ter `MBWay = true`. Esta flag é gerida pelo sibs no BackOffice.

---

### 5.6 Pagamento de Serviços

#### Fluxo funcional
1. O utilizador clica "Pagamento de Serviços"
2. Abre uma lista com os serviços disponíveis, carregada da BD
3. Seleciona um serviço — o código de entidade é preenchido automaticamente
4. Introduz a referência e confirma o valor
5. Clica "Pagar"
6. O saldo diminui e o movimento aparece no histórico com descrição detalhada

#### Fluxo técnico

```
frmMultibanco.btnPagamentos_Click → abre frmPagamentosServicos(contaId)
  │
  frmPagamentosServicos_Load
    └─ cControlo.fListarServicos → cServico.fListarServicos
       → SELECT Id, Nome, Entidade FROM Servicos ORDER BY Nome
  │
  btnPagar_Click
    └─ cControlo.fPagamento(contaId, valor, descricao)
         ├─ cMovimento.fObterSaldo
         ├─ fSaldoSuficiente
         ├─ cMovimento.fAtualizarSaldo
         └─ cMovimento.fInserirMovimento (tipo='P', descricao="EDP | Ent 10001 Ref 123456789")
  │
  DialogResult.OK → frmMultibanco.fRefrescarEcra(novoSaldo)
```

---

### 5.7 Alteração de PIN

#### Fluxo funcional
1. O utilizador preenche todos os campos no ecrã de login e clica "Alterar PIN"
2. As credenciais são validadas antes de permitir a alteração
3. Abre o ecrã de alteração com Banco, Cliente e Conta já preenchidos (somente leitura)
4. O utilizador introduz o novo PIN duas vezes
5. Se os PINs não forem iguais, recebe erro
6. Se forem iguais, o PIN é gravado e o ecrã fecha

#### Fluxo técnico

```
frmAutenticacao.btnAltPin_Click
  ├─ cControlo.fTestarCampos       → campos preenchidos?
  └─ cControlo.fValidarCredenciais → credenciais existem na BD?
       ↓
  abre frmUpdate(banco, cliente, conta, pin)
  │
  frmUpdate.btnOK_Click
    ├─ cControlo.fValidarPinsIguais → txtNovoPin == txtConfPin?
    └─ cControlo.fAtualizarPin → cLogin.fGravarNovoPin
       → UPDATE Credenciais SET Pin=@novo
         WHERE Banco=@B AND Cliente=@C AND Conta=@Co AND Pin=@old
```

**Nota de segurança:** o PIN antigo está no `WHERE` do UPDATE — se a sessão for comprometida entre o login e a alteração, o UPDATE não atualiza nada.

---

### 5.8 BackOffice — admin sibs

#### Fluxo funcional
1. O sibs autentica-se com username "sibs" e o seu PIN
2. Vê a lista de todos os clientes com Id, Banco, Cliente, Conta, Saldo e estado MBWay
3. Pode inserir um novo cliente — saldo inicial fixo de 100€
4. Pode eliminar um cliente — só se o saldo for exatamente 0
5. Pode ativar ou desativar MBWay de qualquer conta

#### Fluxo técnico

```
frmAdmin_Load → fCarregarLista
  └─ cControlo.fListarCredenciais → cAdmin.fListarCredenciais
     → SELECT Id, Banco, Cliente, Conta, Saldo, MBWay FROM Credenciais ORDER BY Cliente, Conta

btnInserir_Click
  └─ cControlo.fInserirCliente → cAdmin.fInserirCredencial
     → INSERT INTO Credenciais (..., Saldo=100.00)

btnEliminar_Click
  └─ cControlo.fEliminarCliente(id)
     ├─ cMovimento.fObterSaldo → saldo == 0?
     └─ cAdmin.fEliminarCredencial → DELETE FROM Credenciais WHERE Id=@Id

btnMBWay_Click
  └─ cControlo.fAlternarMBWay(id, !estadoAtual)
     → UPDATE Credenciais SET MBWay=@novoEstado WHERE Id=@Id
```

---

### 5.9 Simulador de Empréstimo

#### Fluxo funcional
1. O utilizador clica "Simulador Empréstimo" no ecrã principal
2. Introduz Capital, Prazo (anos), Spread, Euribor 12M e TAN fixa
3. Clica "Calcular"
4. Vê side-by-side a prestação mensal, total de juros e custo total para modalidade indexada vs taxa fixa

#### Fluxo técnico

Sem base de dados — cálculo local no próprio formulário.

```
frmSimuladorEmprestimo.btnCalcular_Click
  ├─ valida campos (capital > 0, prazo > 0, taxas >= 0)
  ├─ rIndexada = (Euribor + Spread) / 100 / 12
  ├─ rFixa     = TAN / 100 / 12
  ├─ n         = prazo × 12
  ├─ PMT(r, n) = capital × r / (1 − (1+r)^(−n))   [se r > 0]
  │            = capital / n                         [se r = 0, sem juros]
  └─ preenche labels com prestação, juros totais e custo total para cada modalidade
```

A Euribor é um campo editável — não está hardcoded — porque o seu valor muda mensalmente.

---

### 5.10 Funcionalidades Extra

#### E1 — Toggle MBWay pelo sibs (BackOffice)

O enunciado especifica que "a conta tem de ser aderente para que seja enviado dinheiro" mas não define o mecanismo de ativação. A decisão de atribuir ao sibs esta responsabilidade decorre da lógica operacional: é o administrador da plataforma que gere os serviços das contas, tal como acontece com os operadores de telecomunicações.

**Implementação:** botão no `frmAdmin` que lê o estado `MBWay` da linha selecionada no ListView e inverte-o. A coluna `MBWay` é `BOOLEAN` em `Credenciais`; o `cControlo.fAlternarMBWay` delega para `cAdmin.fAlternarMBWay`.

#### E2 — Simulador de Empréstimo

Calculadora financeira pura — sem BD. Compara duas modalidades de crédito em simultâneo:

| Parâmetro | Modalidade Indexada | Modalidade Fixa |
|---|---|---|
| Taxa | Euribor 12M + Spread | TAN fixa |
| Prestação | PMT(Capital, Euribor+Spread, meses) | PMT(Capital, TAN, meses) |
| Total juros | Prestação × meses − Capital | Prestação × meses − Capital |

**Fórmula PMT** (equivalente ao `PMT` do Excel):

```
Prestação = Capital × r / (1 − (1+r)^(−n))

onde:  r = TAN anual / 100 / 12   (taxa mensal)
       n = prazo em anos × 12     (meses)
```

Caso especial: se TAN = 0%, `Prestação = Capital / n` (sem juros).

---

## 6. Implementação por Camadas

### 6.1 Camada de Dados — `_03Dados`

### `cConexao.cs`

Responsabilidade única: abrir e fechar a ligação. Usada por todas as outras classes de dados como dependência.

```csharp
public NpgsqlConnection conectar()  { oConectar.Open();  return oConectar; }
public void desConectar()           { oConectar.Close(); }
```

O método `desconectar` (internal, minúscula) foi removido — era código morto: mesmo nome, mesmo comportamento, nunca referenciado. Manter dois métodos com nomes quase idênticos é fonte de confusão em manutenção.

---

### `cLogin.cs`

Três responsabilidades de autenticação:

| Método | SQL | Notas |
|---|---|---|
| `fObterCredenciais` | `SELECT Id, Saldo, Banco, Cliente, Conta FROM Credenciais WHERE Banco=@B AND Cliente=@C AND Conta=@Co AND Pin=@P` | Devolve `contaId` + dados de sessão |
| `fValidarAdmin` | `SELECT Id FROM Admins WHERE Username=@U AND Pin=@P` | Login do sibs |
| `fGravarNovoPin` | `UPDATE Credenciais SET Pin=@novo WHERE ... AND Pin=@old` | PIN antigo no `WHERE` — validação implícita |

Todos os comandos usam `Parameters.AddWithValue` — **sem concatenação de strings no SQL**. Proteção contra SQL Injection (OWASP Top 10 A03).

---

### `cAdmin.cs`

Gestão de contas bancárias pelo sibs. Separado do `cLogin` pelo **Princípio de Responsabilidade Única**: `cLogin` autentica, `cAdmin` gere.

Contém um helper privado que elimina o boilerplate de abertura/fecho de ligação (DRY):

```csharp
private bool fExecutarNonQuery(NpgsqlCommand oCmd, string prefixoErro)
{
    try { oCmd.Connection = oConexao.conectar(); oCmd.ExecuteNonQuery(); oConexao.desConectar(); }
    catch (Exception ex) { operacao = false; mensagem = prefixoErro + ex.Message; }
    return operacao;
}
```

Este helper é reutilizado em 4 dos 5 métodos públicos da classe, eliminando 4 blocos `try/catch` idênticos.

Métodos públicos:

| Método | Operação SQL | Regra de negócio |
|---|---|---|
| `fListarCredenciais` | `SELECT` ordenado por `Cliente, Conta` | — |
| `fInserirCredencial` | `INSERT ... VALUES (..., 100)` | Saldo inicial fixo em 100€ |
| `fEliminarCredencial` | `DELETE` | Só chamado se saldo = 0 (validado no `cControlo`) |
| `fAlternarMBWay` | `UPDATE ... SET MBWay = @MBWay` | Toggle — o estado é passado pelo `cControlo` |

> **Nota:** `fObterSaldo` não existe em `cAdmin` — existe em `cMovimento`, onde é partilhado por todas as operações bancárias. Duplicar o mesmo SQL em duas classes seria redundância desnecessária; o `cControlo.fEliminarCliente` usa `cMovimento.fObterSaldo` antes de chamar `fEliminarCredencial`.

---

### `cMovimento.cs`

Operações sobre saldos e histórico de movimentos. Saldo e movimentos coexistem na mesma classe porque cada operação bancária realiza sempre as duas ações em conjunto (atualizar saldo + inserir movimento).

O mesmo helper `fExecutarNonQuery` de `cAdmin` está presente aqui — ambas as classes herdam do mesmo padrão.

Método de destaque — `fObterContaDestino`:

```csharp
public void fObterContaDestino(int contaNum, out int idDestino, out decimal saldoDestino)
{
    // SELECT Id, Saldo FROM Credenciais WHERE Conta = @Conta
}
```

Este método resolve numa única query dois problemas: converte o número de conta visível (introduzido pelo utilizador) para o `Id` interno (chave primária) e devolve simultaneamente o saldo atual. Sem ele, seriam necessárias duas queries separadas para transferências e MBWay.

`fListarMovimentos` aceita datas opcionais:

```sql
SELECT Tipo, Valor, SaldoApos, DataHora, Descricao
FROM Movimentos
WHERE ContaId = @ContaId
  [AND DataHora >= @DataInicio AND DataHora <= @DataFim + 23:59:59]
ORDER BY DataHora DESC
```

O filtro de datas é dinâmico — a cláusula `AND` só é adicionada quando os parâmetros não são nulos. Um único método serve os dois casos (com e sem filtro).

---

### 6.2 Camada de Controlo — `_02Controlo`

### `cControlo.cs`

Intermediário obrigatório entre formulários e BD. **Os formulários nunca instanciam classes de dados diretamente.**

#### Propriedades de resultado (protocolo de comunicação com a apresentação)

| Propriedade | Tipo | Significado |
|---|---|---|
| `operacao` | `bool` | `true` se a operação correu sem erros |
| `mensagem` | `string` | Texto para mostrar ao utilizador |
| `saldoAtualizado` | `decimal` | Novo saldo após operação (lido pelo formulário para refrescar o ecrã) |
| `contaId`, `saldo`, `banco`, `cliente`, `conta` | vários | Dados de sessão após login |
| `listaCredenciais`, `listaMovimentos`, `listaServicos` | `List<string[]>` | Listas para preencher os ListViews |

#### Regras de negócio por operação

| Operação | Validações aplicadas (por ordem) |
|---|---|
| `fLevantar` | valor > 0 → saldo ≥ valor → atualiza saldo → insere movimento `'L'` |
| `fDepositar` | valor > 0 → atualiza saldo → insere movimento `'D'` |
| `fTransferir` | valor > 0 → resolve conta destino → origem ≠ destino → saldo ≥ valor → 2×update + 2×insert `'T'` |
| `fMBWay` | valor > 0 → resolve conta destino → verifica `MBWay = true` na destino → saldo ≥ valor → 2×update + 2×insert `'M'` |
| `fPagamento` | saldo ≥ valor → atualiza saldo → insere movimento `'P'` com descrição livre |
| `fEliminarCliente` | obtém saldo via `cMovimento.fObterSaldo` → bloqueia se ≠ 0 → `cAdmin.fEliminarCredencial` |

A regra "não eliminar se saldo ≠ 0" está aqui, na camada de controlo — **não** na camada de dados. A `cAdmin.fEliminarCredencial` executa um DELETE sem questionar; é o `cControlo` que decide se tem autorização para o chamar. Isso respeita a separação de responsabilidades da arquitectura de 3 camadas.

#### Helper `fSaldoSuficiente`

```csharp
private bool fSaldoSuficiente(decimal saldo, decimal valor)
{
    if (saldo < valor)
    {
        operacao = false;
        mensagem = "Saldo insuficiente. Disponível: " + saldo.ToString("F2") + "€.";
        return false;
    }
    return true;
}
```

Este helper é chamado em 4 métodos (`fLevantar`, `fTransferir`, `fMBWay`, `fPagamento`). Sem ele, o bloco `if + mensagem` estaria duplicado 4 vezes — qualquer alteração ao formato da mensagem de erro teria de ser feita em 4 sítios.

---

### 6.3 Camada de Apresentação — `_01Apresentacao`

### Visão geral dos formulários

| Formulário | Perfil | Função |
|---|---|---|
| `frmAutenticacao` | Todos | Ponto de entrada único; routing por perfil |
| `frmUpdate` | Cliente | Alteração de PIN |
| `frmAdmin` | sibs | BackOffice CRUD + toggle MBWay |
| `frmMultibanco` | Cliente | Ecrã principal — todas as operações bancárias |
| `frmPagamentosServicos` | Cliente | Pagamentos pré-definidos (9 serviços, carregados da BD) |
| `frmSimuladorEmprestimo` | Cliente | Simulador financeiro — sem BD (extra) |

---

### `frmAutenticacao.cs`

**Routing de perfil antes da validação:**

```csharp
if (txtCliente.Text.ToLower() == "sibs")
{
    oCtrl.fTestarCamposAdmin(); // valida só Cliente + PIN
    if (oCtrl.operacao) oCtrl.fValidarAdmin();
    if (oCtrl.operacao) { new frmAdmin().ShowDialog(); ... }
}
else
{
    oCtrl.fTestarCampos();      // valida os 4 campos
    if (oCtrl.operacao) oCtrl.fValidarCredenciais();
    if (oCtrl.operacao) { new frmMultibanco(...).ShowDialog(); ... }
}
```

**Bug corrigido:** `cControlo oCtrl` é criado **dentro** de cada handler de clique, não como campo da classe. Se fosse campo, após uma tentativa falhada (`operacao = false`), na tentativa seguinte o estado residual mantinha-se — o botão "Alterar PIN" abria mesmo após erro. Instanciar por clique garante estado limpo.

**Encapsulamento de dados de sessão:** após login, o `frmAutenticacao` passa os dados ao `frmMultibanco` via construtor:

```csharp
new frmMultibanco(oCtrl.contaId, oCtrl.saldo, oCtrl.banco, oCtrl.cliente, oCtrl.conta)
```

O formulário principal tem esses dados desde o momento em que abre, sem query adicional.

---

### `frmUpdate.cs` — Encapsulamento explícito (requisito OOP)

Os campos de dados são `private`; o acesso externo é exclusivamente por getters/setters:

```csharp
private string txt_Banco = "";
public string get_Banco()           { return txt_Banco; }
public void   set_Banco(string ban) { txt_Banco = ban; }
```

Este padrão — **encapsulamento**, um dos 4 pilares de OOP — é requisito explícito do enunciado. A diferença para o `frmMultibanco` (que usa campos privados sem getters/setters) é intencional: o `frmUpdate` é construído com dados externos que chegam pelo `frmAutenticacao`; o `frmMultibanco` recebe os dados pelo construtor diretamente, tornando getters/setters redundantes nesse contexto.

---

### `frmAdmin.cs`

**`fCarregarLista`** — helper privado chamado ao abrir e após cada operação:

```csharp
private void fCarregarLista()
{
    cControlo oCtrl = new cControlo();
    oCtrl.fListarCredenciais();
    lvClientes.Items.Clear();
    foreach (string[] linha in oCtrl.listaCredenciais)
    {
        ListViewItem item = new ListViewItem(linha[0]); // Id
        item.SubItems.Add(linha[1]); // Banco
        // ...
    }
    lblTotal.Text = "Total: " + oCtrl.listaCredenciais.Count + " conta(s)";
}
```

**`fMostrarResultadoEAtualizar`** — elimina o bloco `if/else + MessageBox + refresh` repetido em 3 handlers (DRY).

---

### `frmMultibanco.cs`

Ecrã principal. Recebe 5 parâmetros via construtor: `contaId`, `saldo`, `banco`, `cliente`, `conta`. Todos os dados de sessão são campos `private`.

**Colunas do `ListView` no evento `Load` (não no Designer):**

```csharp
private void frmMultibanco_Load(object sender, EventArgs e)
{
    lstMovimentos.Columns.Add("Tipo",        55);
    lstMovimentos.Columns.Add("Valor €",     78);
    lstMovimentos.Columns.Add("Saldo Após",  88);
    lstMovimentos.Columns.Add("Data/Hora",  128);
    lstMovimentos.Columns.Add("Descrição",  125);
    // ...
}
```

As colunas estão no `Load` e não no `Designer.cs` porque o Windows Forms Designer, ao guardar alterações gráficas, serializa apenas as propriedades que conhece pela sua interface — e apaga colunas adicionadas manualmente. Mover para o `Load` protege-as de qualquer intervenção do Designer.

**`fMostrarResultado`** — DRY para os 4 handlers de operações bancárias:

```csharp
private void fMostrarResultado(cControlo oCtrl)
{
    if (!oCtrl.operacao) MessageBox.Show(oCtrl.mensagem, "ERRO", ...);
    else { MessageBox.Show(oCtrl.mensagem, "Sucesso", ...); fRefrescarEcra(oCtrl.saldoAtualizado); }
}
```

**Filtro de datas:**

```
Por defeito: dtpInicio = 1.º dia do mês atual | dtpFim = hoje

[Filtrar] → valida dtpInicio ≤ dtpFim → fCarregarMovimentos(dataInicio, dataFim)
[Todos]   → fCarregarMovimentos()  ← sem argumentos = sem filtro
```

`fCarregarMovimentos` aceita parâmetros opcionais (`DateTime? = null`) — as chamadas existentes no `Load` e após cada operação não precisaram de ser alteradas.

---

### `frmPagamentosServicos.cs`

Os serviços não estão hardcoded — são carregados da tabela `Servicos` ao abrir o formulário:

```sql
SELECT Id, Nome, Entidade FROM servicos ORDER BY Nome
```

Ao selecionar um serviço na lista, o campo `Entidade` é preenchido automaticamente. O utilizador introduz apenas a referência (9 dígitos) e o valor.

**Decisão de design:** o sibs não tem UI para gerir serviços (só a BD pode ser editada diretamente). Documentado como limitação e evolução possível — ver secção 9.

**Tratamento de erro no construtor:** se a BD falhar ao carregar serviços, `btnPagar` é desativado. O formulário não é fechado no construtor (`Close()` num construtor, antes do `Show`, lança `ObjectDisposedException`).

---

## 7. Princípios de Engenharia de Software Aplicados

| Princípio | Onde se manifesta |
|---|---|
| **Arquitetura em 3 camadas** | Estrutura de pastas `_01/_02/_03`; formulários nunca acedem à BD diretamente |
| **SRP — Responsabilidade Única** | `cLogin` autentica; `cAdmin` gere; `cMovimento` opera; `cControlo` valida |
| **DRY — Don't Repeat Yourself** | 4 helpers extraídos: `fExecutarNonQuery`, `fSaldoSuficiente`, `fMostrarResultado`, `fMostrarResultadoEAtualizar` |
| **Encapsulamento (OOP)** | Campos `private` em todos os formulários; getters/setters explícitos em `frmUpdate`; dados de sessão inacessíveis do exterior |
| **Separação de responsabilidades** | Regras de negócio no `cControlo`; SQL na camada de dados; apresentação sem lógica |
| **Anti SQL Injection** | `Parameters.AddWithValue` em todos os comandos — sem concatenação de strings no SQL |
| **Imutabilidade de registos** | Tabela `Movimentos` — só INSERT; nunca UPDATE/DELETE |
| **Gestão de estado** | `cControlo` instanciado por clique, não como campo — garante estado limpo entre tentativas |
| **Injeção de contexto via construtor** | Dados de sessão passados pelo construtor; formulário nunca faz query de login redundante |

---

## 8. Decisões Técnicas Justificadas

### 8.1 Um ecrã de login para dois perfis

O mesmo `frmAutenticacao` serve clientes e o sibs. O routing é feito pelo campo `Cliente`: se for `"sibs"`, a validação usa só 2 campos (Cliente + PIN); se for qualquer outra coisa, usa os 4 campos (Banco + Cliente + Conta + PIN). **Benefício:** um único ponto de entrada; sem ecrãs duplicados; a distinção é feita na lógica, não na interface.

### 8.2 `fObterContaDestino` — resolução em query única

Transferências e MBWay precisam de converter o número de conta visível (ex: `654321`) para o `Id` interno da tabela, **e** de saber o saldo atual da conta destino. Dois métodos separados significariam 2 queries. Um único método resolve ambos — 1 query, 1 round trip à BD.

### 8.3 Dois movimentos por transferência/MBWay

Cada transferência gera um registo de débito na conta origem e um registo de crédito na conta destino. Cada cliente vê o histórico completo da sua própria perspetiva, sem necessidade de cruzar dados com a outra conta.

### 8.4 `SaldoApos` no movimento — desnormalização controlada

É redundante face ao saldo atual em `Credenciais`. É intencional: o histórico de saldos tem de ser imutável e auto-suficiente. Se o saldo em `Credenciais` fosse alterado (corrupção, erro), o histórico continuaria correto.

### 8.5 `List<string[]>` como estrutura de transferência entre camadas

As listas de dados (credenciais, movimentos, serviços) são transportadas como `List<string[]>` entre camadas. A alternativa seria criar classes DTO. Para o âmbito deste projeto, `string[]` é suficiente: os formulários apenas apresentam os dados — não precisam de comportamento de objetos tipados.

### 8.6 Serviços pré-definidos em base de dados, não hardcoded

A versão inicial tinha os 9 serviços num array no código. Foram movidos para a tabela `Servicos`, carregada ao abrir o formulário. Vantagem imediata: qualquer alteração de serviços é feita na BD sem recompilar a aplicação. Limitação atual: não há UI para gestão pelo sibs — documentado em secção 11.

---

## 9. Limitações e Evoluções

| Área | Limitação atual | Evolução possível |
|---|---|---|
| Gestão de serviços | Tabela `Servicos` só lida pela aplicação — sem UI de gestão pelo sibs | Separador no `frmAdmin` para CRUD de serviços |
| Euribor no simulador | Introduzida manualmente | Integração com API pública do Banco de Portugal |
| Transferências externas | Só entre contas do sistema | Suporte a IBAN para bancos externos |
| Simulações persistidas | Cálculo PMT descartado ao fechar | Tabela `Simulacoes` associada à conta para consulta futura |
| Sessão múltipla | Uma sessão de cada vez | Suporte a sessões paralelas com isolamento por conta |

---

## 10. Conclusão

O projeto implementa a totalidade dos requisitos obrigatórios do enunciado e acrescenta duas funcionalidades extra com justificação técnica e operacional documentada.

A decisão central de organização — **arquitetura em três camadas com comunicação unidirecional** — é o fio condutor de todas as escolhas de implementação. Cada classe tem uma responsabilidade delimitada; as regras de negócio vivem exclusivamente na camada de controlo; a camada de dados executa sem decidir; os formulários apresentam sem conter lógica.

O conjunto de refactorizações aplicadas (4 helpers DRY, remoção de código morto, parâmetros opcionais para compatibilidade retroativa) demonstra que a arquitetura foi tratada não como requisito formal mas como estrutura de trabalho real — o código foi escrito, testado, e refatorizado de forma iterativa.

---

## Anexo A — Estrutura de Ficheiros

```
Multibanco_v3/
│
├── Program.cs                          Ponto de entrada — Application.Run(new frmAutenticacao())
├── criar_bd.sql                        3 tabelas + dados de teste (4 contas, 5 movimentos)
│
├── _01Apresentacao/
│   ├── frmAutenticacao.cs + .Designer  Login + routing por perfil
│   ├── frmUpdate.cs + .Designer        Alteração de PIN (encapsulamento OOP)
│   ├── Form1.cs + .Designer            frmMultibanco — ecrã principal cliente
│   ├── frmAdmin.cs + .Designer         BackOffice sibs
│   ├── frmPagamento.cs + .Designer     Pagamento livre (Entidade + Referência)
│   ├── frmPagamentosServicos.cs        Pagamento pré-definido (serviços da BD)
│   ├── frmSimuladorEmprestimo.cs       Simulador PMT (extra)
│   └── Tema.cs                         Tema visual escuro — ver Anexo C
│
├── _02Controlo/
│   └── cControlo.cs                    Todos os métodos de negócio e validação
│
└── _03Dados/
    ├── cConexao.cs                     Ligação PostgreSQL
    ├── cLogin.cs                       Autenticação + alteração de PIN
    ├── cAdmin.cs                       CRUD de credenciais (sibs)
    ├── cMovimento.cs                   Operações bancárias + histórico
    └── cServico.cs                     Leitura da tabela Servicos
```

---

## Anexo B — Esquema da Base de Dados

```sql
-- Administrador da plataforma (sibs)
CREATE TABLE Admins (
    Id       SERIAL      PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Pin      INTEGER     NOT NULL
);

-- Contas bancárias dos clientes
CREATE TABLE Credenciais (
    Id      SERIAL        PRIMARY KEY,
    Banco   VARCHAR(50)   NOT NULL,
    Cliente VARCHAR(100)  NOT NULL,
    Conta   INTEGER       NOT NULL UNIQUE,
    Pin     INTEGER       NOT NULL,
    Saldo   DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    MBWay   BOOLEAN       NOT NULL DEFAULT FALSE
);

-- Histórico imutável de operações
CREATE TABLE Movimentos (
    Id        SERIAL        PRIMARY KEY,
    ContaId   INTEGER       NOT NULL REFERENCES Credenciais(Id),
    Tipo      CHAR(1)       NOT NULL CHECK (Tipo IN ('D','L','T','P','M')),
    Valor     DECIMAL(10,2) NOT NULL,
    SaldoApos DECIMAL(10,2) NOT NULL,
    DataHora  TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Descricao VARCHAR(200)
);

-- Serviços pré-definidos (EDP, Água, Gás, telecomunicações, carregamentos)
CREATE TABLE Servicos (
    Id       SERIAL      PRIMARY KEY,
    Nome     VARCHAR(100) NOT NULL,
    Entidade VARCHAR(5)   NOT NULL
);
```

**Dados de teste incluídos:**

| Tabela | Conteúdo |
|---|---|
| `Admins` | sibs / PIN 1111 |
| `Credenciais` | João Silva (CGD, 2 contas), Maria Santos (BPI, MBWay ativo), Rui Costa (BCP) |
| `Movimentos` | 5 movimentos históricos distribuídos pelas 3 contas |
| `Servicos` | 9 registos: EDP, EPAL, Galp, NOS, MEO, Vodafone, NOWO, CTT, Carregamento |

---

## Anexo C — Interface Visual (com assistência de IA)

> **Declaração de transparência:** o tema visual escuro e o refactor de layout descritos neste anexo foram desenvolvidos com assistência de IA (Claude). Toda a lógica aplicacional, a arquitetura, a base de dados e as funcionalidades foram implementadas pelo autor. O trabalho de design visual foi colaborativo com IA e é aqui documentado com total honestidade, conforme solicitado.

---

### C.1 — Abordagem técnica

A estilização é centralizada na classe estática `Tema.cs`. Um único método `Tema.Aplicar(this)` no construtor de cada formulário percorre recursivamente todos os controlos e aplica o estilo adequado a cada tipo. Qualquer alteração de cor propaga-se imediatamente a todos os formulários — DRY aplicado ao design visual.

### C.2 — Paleta de cores

| Constante | RGB | Uso |
|---|---|---|
| `Fundo` | `(22, 22, 22)` | Fundo de todos os formulários |
| `FundoControlo` | `(45, 45, 45)` | TextBox, ListView, GroupBox |
| `TextoPrimario` | `(210, 210, 210)` | Labels normais |
| `TextoAcento` | `(255, 200, 0)` — amarelo ATM | Títulos, valores monetários |
| `BotaoAcao` | `(0, 100, 180)` — azul | Ações principais |
| `BotaoPerigo` | `(180, 40, 40)` — vermelho | Sair, Cancelar, Eliminar |
| `BotaoNeutro` | `(65, 65, 65)` — cinza | Ações secundárias (Filtrar, Todos) |

### C.3 — Lógica de cor dos botões (por nome)

```
Nome contém "sair" | "cancelar" | "fechar" | "exit" | "eliminar"  →  vermelho
Nome contém "todos" | "limpar" | "filtrar"                        →  cinza
Qualquer outro botão                                              →  azul
```

### C.4 — Decisões de layout no `frmMultibanco`

| Problema | Solução |
|---|---|
| Tabela e filtro sem delimitação visual | `GroupBox "Histórico de Movimentos"` agrupa ListView + barra de filtro |
| Botões de filtro fora do limite da tabela | Coordenadas relativas ao GroupBox — contenção garantida pelo container |
| Saldo sem contexto | Label `"Saldo disponível:"` acima do campo de valor |
| Banco/conta imperceptíveis | Font 10pt Bold em `lblBanco` e `lblCliente` |
| Campos colados verticalmente | Reorganização com espaçamentos mínimos de 20px entre grupos |

### C.5 — Nota técnica sobre o Windows Forms Designer

O Designer do Visual Studio elimina chamadas `Columns.Add` do `ListView` ao guardar alterações gráficas — serializa apenas propriedades que conhece pela sua interface. Solução: as colunas foram movidas para o evento `Load` em `Form1.cs`, que o Designer nunca modifica.
