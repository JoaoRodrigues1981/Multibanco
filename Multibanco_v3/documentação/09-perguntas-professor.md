# 9. Perguntas do Professor — Respostas Rápidas

**"Porque é que os formulários não acedem diretamente à BD?"**
> Para separar responsabilidades — se mudarmos de PostgreSQL para MySQL, só mudamos a camada de dados. Os formulários e o controlo ficam intactos.

**"O que é SQL Injection e como está protegido?"**
> SQL Injection é quando o utilizador escreve SQL numa caixa de texto para manipular a query. Estamos protegidos porque usamos `Parameters.AddWithValue("@param", valor)` — o Npgsql trata o valor sempre como dado, nunca como SQL.

**"O que é encapsulamento?"**
> Esconder os dados internos de uma classe. Em `frmMultibanco`, os campos `_contaId`, `_saldo`, etc. são `private` — só o formulário os pode ler e alterar. Se fossem `public`, qualquer classe poderia corromper os dados da sessão.

**"O que é um construtor com parâmetros?"**
> É um construtor que obriga quem cria o objeto a fornecer dados. `new frmMultibanco(contaId, saldo, banco, cliente, conta)` garante que nunca existe um ecrã principal sem uma sessão válida.

**"Como funciona o ExecuteReader vs ExecuteNonQuery?"**
> `ExecuteReader` é para SELECT — devolve um `NpgsqlDataReader` que lemos linha a linha com `Read()`. `ExecuteNonQuery` é para INSERT/UPDATE/DELETE — não devolve linhas, apenas executa.

**"O que é o `out` na transferência?"**
> Permite a um método devolver mais do que um valor. `fObterContaDestino(contaNum, out int id, out decimal saldo)` resolve o número de conta e preenche o Id e saldo numa só query — mais eficiente do que fazer duas queries separadas.

**"Como o MBWay é diferente da transferência?"**
> Interface completamente diferente (janela com lista de contactos vs campo manual), limite de 300€, verifica se origem E destino têm MBWay ativo, exclui a própria conta da lista, regista com tipo `"M"` em vez de `"T"`.

**"Como funciona o bloqueio de conta?"**
> Cada falha de login incrementa `Tentativas` e usa `CASE WHEN Tentativas + 1 >= 3 THEN TRUE ELSE Bloqueada END` para bloquear automaticamente na 3ª falha. Está na BD — persiste mesmo que a app feche. Só o admin pode desbloquear.

**"O que é `DateTime?` com o ponto de interrogação?"**
> O `?` torna o tipo nullable — pode ter um valor de data OU ser `null`. Usamos em `fListarMovimentos(DateTime? dataInicio, DateTime? dataFim)`: quando é `null`, mostramos todos os movimentos; quando tem data, filtramos.

**"O que faz o AutoResizeColumn?"**
> Ajusta a largura de uma coluna do ListView ao conteúdo mais largo. Usamos na coluna "Descrição" (índice 4) para que descrições longas não fiquem cortadas — activa scroll horizontal automaticamente.

**"O que é DRY?"**
> Don't Repeat Yourself — não copiar o mesmo código em vários sítios. Temos 4 helpers que evitam repetição: `fExecutarNonQuery` (em cAdmin), `fSaldoSuficiente`, `fMostrarResultado` e `fMostrarResultadoEAtualizar` (nos formulários).

**"O cliente pode ter várias contas?"**
> Sim. O campo único é o número de conta (`Conta INTEGER UNIQUE`), não o nome do cliente. O João Silva tem as contas 123456 e 123457 — são duas linhas distintas em `Credenciais` com o mesmo nome.

**"Porque é que os movimentos só têm INSERT e nunca UPDATE ou DELETE?"**
> Para manter um histórico imutável — como um extrato bancário real. Qualquer operação fica registada permanentemente. O saldo atual está sempre em `Credenciais.Saldo`, e o histórico de como chegou lá está em `Movimentos`.
