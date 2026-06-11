# Decisões de Implementação — Multibanco v3

---

## MBWay — Janela dedicada com lista de contactos
**Decisão:** Criar formulário próprio (`frmMBWay`) em vez de reutilizar os campos do ecrã principal, com lista de contas aderentes e limite de 300€ por transação.

| Passo | O que foi feito |
|-------|-----------------|
| 1 | `criar_bd.sql` — adicionada coluna `Telefone VARCHAR(9)` a `Credenciais` + dados de teste com números |
| 2 | `_03Dados/cMBWay.cs` — novo método `fListarContasMBWay(int contaOrigem)` que devolve `[Cliente, Telefone, Conta]` excluindo a conta do utilizador e contas sem MBWay |
| 3 | `_02Controlo/cControlo.cs` — exposto `fListarContasMBWay` + `listaContasMBWay` |
| 4 | `_01Apresentacao/frmMBWay.cs` + `frmMBWay.Designer.cs` — janela com listbox, campo valor, botão Enviar/Cancelar; devolve `DialogResult.OK` com `NovoSaldo` |
| 5 | `_01Apresentacao/Form1.cs` — `btnMBWay_Click` simplificado para abrir `frmMBWay` |

**Regras aplicadas:**
- Conta origem tem de ter MBWay ativo (botão desativado se não for aderente)
- Conta destino tem de ter MBWay ativo (verificado em `cControlo.fMBWay`)
- Não é possível enviar para a própria conta (excluída da lista pela query)
- Limite de 300€ por transação

---

## Bloqueio de conta — Contador de tentativas de login
**Decisão:** Bloquear a conta após 3 tentativas de login falhadas. O desbloqueio é feito pelo administrador "sibs" no BackOffice — sem reset automático ao fechar a app.

| Passo | O que foi feito |
|-------|-----------------|
| 1 | `criar_bd.sql` — adicionadas colunas `Tentativas INTEGER DEFAULT 0` e `Bloqueada BOOLEAN DEFAULT FALSE` à tabela `Credenciais` |
| 2 | `_03Dados/cLogin.fObterCredenciais` — reestruturado com 3 fases: (1) verificar bloqueio antes de tentar login; (2) verificar credenciais normalmente; (3) reset ou incremento de tentativas consoante o resultado |
| 3 | `_03Dados/cAdmin.cs` — novo método `fDesbloquearConta(int id)` — `UPDATE Credenciais SET Tentativas = 0, Bloqueada = FALSE WHERE Id = @Id` |
| 4 | `_02Controlo/cControlo.cs` — exposto `fDesbloquearConta` |
| 5 | `_01Apresentacao/frmAdmin.cs` + `frmAdmin.Designer.cs` — botão "Desbloquear conta" no BackOffice |

**Regras aplicadas:**
- Bloqueio por conta, não por utilizador — conta 123456 bloqueada não afeta a 123457
- Mensagem distinta: "Credenciais inválidas" vs "Conta bloqueada após 3 tentativas"
- Login bem-sucedido faz reset ao contador (proteção contra bloqueio acidental)
- O incremento usa SQL `CASE WHEN Tentativas + 1 >= 3 THEN TRUE ELSE Bloqueada END` — tudo numa única query UPDATE
- Só o admin "sibs" pode desbloquear via BackOffice
