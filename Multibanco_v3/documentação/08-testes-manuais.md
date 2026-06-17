# 8. Bateria de Testes Manuais

> Correr `criar_bd.sql` antes de cada sessão para garantir dados limpos.

---

### T.1 Autenticação

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T01 | Campos vazios | Clicar OK sem preencher nada | Erro: "Preencha todos os campos." |
| T02 | Banco em falta | Preencher Cliente, Conta, PIN; Banco vazio | Erro: "Preencha todos os campos." |
| T03 | PIN em falta | Preencher Banco, Cliente, Conta; PIN vazio | Erro: "Preencha todos os campos." |
| T04 | PIN errado | Banco=CGD, Cliente=João Silva, Conta=123456, PIN=**9999** | Erro: "Credenciais inválidas." |
| T05 | Conta inexistente | Conta=**999999**, PIN=1234 | Erro: "Credenciais inválidas." |
| T06 | Login válido | Banco=CGD, Cliente=João Silva, Conta=123456, PIN=1234 | Abre frmMultibanco, saldo 850,00€ |
| T07 | Segunda conta | Conta=**123457**, PIN=1234 | Abre frmMultibanco, saldo 200,00€ |
| T08 | Admin correto | Cliente=sibs, PIN=1111 | Abre frmAdmin |
| T09 | Admin PIN errado | Cliente=sibs, PIN=**0000** | Erro: "Credenciais de administrador inválidas." |
| T10 | Admin PIN vazio | Cliente=sibs, PIN vazio | Erro: "Preencha o utilizador e o PIN." |

---

### T.2 Bloqueio de conta

*Pré-condição: conta 123456 com Tentativas=0 e Bloqueada=FALSE*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T11 | 1ª falha | Conta=123456, PIN=**0000** | Erro: "Credenciais inválidas." — BD: Tentativas=1 |
| T12 | 2ª falha | Repetir T11 | BD: Tentativas=2 |
| T13 | 3ª falha — bloqueia | Repetir T11 | BD: Tentativas=3, Bloqueada=TRUE |
| T14 | Após bloqueio, PIN correto | Conta=123456, PIN=**1234** | Erro: "Conta bloqueada após 3 tentativas." |
| T15 | Bloqueio persiste | Fechar app, reabrir, tentar login | Erro: "Conta bloqueada..." |
| T16 | Admin desbloqueia | sibs → selecionar João Silva (Id 1) → Desbloquear | "Conta desbloqueada com sucesso." |
| T17 | Login após desbloqueio | Conta=123456, PIN=1234 | Abre frmMultibanco normalmente |
| T18 | Reset após login OK | 2 falhas → login correto → 1 falha | "Credenciais inválidas" (não bloqueia) |

---

### T.3 Levantamento

*Pré-condição: João Silva, Conta 123456, saldo 850,00€*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T19 | Valor vazio | Não preencher; clicar Levantar | Erro: "valor numérico válido e maior que zero" |
| T20 | Valor = 0 | txtValor="0" | Erro: "maior que zero" |
| T21 | Valor negativo | txtValor="-10" | Erro: "maior que zero" |
| T22 | Valor com texto | txtValor="abc" | Erro: "valor numérico válido" |
| T23 | Valor > saldo | txtValor="1000" | Erro: "Saldo insuficiente. Saldo disponível: 850.00€." |
| T24 | Valor = saldo | txtValor="850" | Sucesso; saldo fica **0,00€** |
| T25 | Valor com vírgula | txtValor="10,50" | Sucesso; saldo atualizado |
| T26 | Valor válido | txtValor="50" | Sucesso; saldo = 800,00€ |

---

### T.4 Depósito

*Pré-condição: João Silva, Conta 123456*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T27 | Valor vazio | Clicar Depositar sem valor | Erro: "valor numérico válido" |
| T28 | Valor = 0 | txtValor="0" | Erro: "maior que zero" |
| T29 | Valor negativo | txtValor="-50" | Erro: "maior que zero" |
| T30 | Depósito válido | txtValor="200" | Sucesso; saldo aumenta 200€ |

---

### T.5 Transferência

*Pré-condição: João Silva, Conta 123456, saldo 850,00€*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T31 | Valor vazio | Destino=654321; valor vazio | Erro: "valor numérico válido" |
| T32 | Destino vazio | Valor=50; destino vazio | Erro: "número de conta destino válido" |
| T33 | Destino com letras | txtContaDestino="abc" | Erro: "número de conta destino válido" |
| T34 | Destino inexistente | Destino=**999999** | Erro: "Conta destino não encontrada." |
| T35 | Destino = origem | Destino=**123456** | Erro: "origem e destino não podem ser iguais" |
| T36 | Valor > saldo | Valor=**1000** | Erro: "Saldo insuficiente" |
| T37 | Transferência válida | Valor=100; Destino=654321 | Sucesso; João -100€; Maria +100€; 2 movimentos "T" |

---

### T.6 MBWay

*João Silva (MBWay=Não):*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T38 | Botão desativado | Login João Silva | Botão "MBWay" cinzento (Enabled=false) |

*Maria Santos (MBWay=Sim):*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T39 | Botão ativo | Login Maria Santos | Botão "MBWay" clicável |
| T40 | Lista carregada | Clicar "MBWay" | Abre frmMBWay com contas MBWay=Sim excluindo a própria |
| T41 | Própria conta excluída | Verificar lista | "Maria Santos" NÃO aparece |
| T42 | Cancelar | Clicar "Cancelar" | Janela fecha; sem movimento |
| T43 | Valor = 0 | txtValor="0" | Erro: "maior que zero" |
| T44 | Valor > 300€ | txtValor="350" | Erro: "O MBWay tem um limite de 300€ por transação." |
| T45 | Valor > saldo | txtValor="2000" (saldo=1500) | Erro: "Saldo insuficiente" |
| T46 | 300€ exatos | Ana Lima; txtValor="300" | Sucesso; Maria -300€; Ana +300€; 2 movimentos "M" |
| T47 | Valor normal | Sofia Pereira; txtValor="50" | Sucesso; janela fecha; frmMultibanco refresca |

---

### T.7 Pagamento de Serviços

*João Silva, Conta 123456*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T48 | Lista carregada | Abrir frmPagamentosServicos | 9 serviços listados (EDP, EPAL, etc.) |
| T49 | Referência < 9 dígitos | Referência="12345" | Erro: "exatamente 9 dígitos" |
| T50 | Referência > 9 dígitos | Referência="1234567890" | Erro: "exatamente 9 dígitos" |
| T51 | Valor = 0 | Referência="123456789"; Valor=0 | Erro: "maior que zero" |
| T52 | Valor > saldo | Valor=9999 | Erro: "Saldo insuficiente" |
| T53 | Pagamento válido | EDP; Ref="123456789"; Valor=30 | Sucesso; movimento "Pagamento" |
| T54 | Cancelar | Clicar Cancelar | Janela fecha; sem débito |

---

### T.8 Histórico e Filtro de Datas

*João Silva, Conta 123456*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T55 | Histórico ao abrir | Abrir frmMultibanco | Lista preenchida automaticamente |
| T56 | Coluna auto-ajusta | Verificar coluna Descrição | Alarga ao conteúdo; scroll horizontal |
| T57 | Início > Fim | Início=2026-06-01; Fim=2026-01-01 | Erro: "data de início não pode ser posterior" |
| T58 | Período sem movimentos | 2025-01-01 a 2025-01-31 | "Nenhum movimento encontrado no período" |
| T59 | Período com movimentos | 2026-04-01 a 2026-04-30 | Só movimentos de Abril 2026 |
| T60 | Ver Todos | Clicar "Ver Todos" após filtro | Lista completa sem filtro |

---

### T.9 Relógio

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T61 | Visível ao abrir | Login → ecrã principal | Label com data/hora |
| T62 | Atualiza por segundo | Observar 5 segundos | Segundos avançam |

---

### T.10 Alterar PIN

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T63 | Credenciais inválidas | Clicar "Alterar PIN" com PIN errado | Erro; frmUpdate não abre |
| T64 | PINs diferentes | Novo=5555; Confirmar=6666 | Erro: "Os PINs introduzidos são diferentes." |
| T65 | Alterar com sucesso | Novo=9876; Confirmar=9876 | Sucesso; login com 9876 funciona |
| T66 | PIN antigo inválido | Tentar login com PIN 1234 após T65 | Erro: "Credenciais inválidas." |

---

### T.11 BackOffice — Inserir cliente

*sibs / 1111*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T67 | Campos em branco | Clicar Inserir sem preencher | Erro: "Preencha todos os campos." |
| T68 | Conta duplicada | Conta=**123456** (já existe) | Erro de UNIQUE constraint |
| T69 | Inserção válida | Santander; Novo Cliente; Conta=999001; PIN=1111 | Sucesso; Saldo=100,00€; MBWay=Não |

---

### T.12 BackOffice — Eliminar cliente

*sibs / 1111*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T70 | Sem seleção | Clicar Eliminar | Aviso: "Selecione um cliente." |
| T71 | Cancelar | Selecionar; Eliminar; clicar **Não** | Nada acontece |
| T72 | Saldo ≠ 0 | João Silva (850€); confirmar Sim | Erro: "Não é possível eliminar. A conta tem saldo de 850.00€." |
| T73 | Saldo = 0 | Criar via T69; levantar 100€; eliminar | Sucesso |

---

### T.13 BackOffice — MBWay

*sibs / 1111*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T74 | Sem seleção | Clicar MBWay | Aviso: "Selecione um cliente." |
| T75 | Ativar | Rui Costa (MBWay=Não); confirmar Sim | Coluna MBWay passa a **Sim** |
| T76 | Cancelar | Selecionar qualquer; clicar **Não** | Estado não muda |
| T77 | Desativar | Maria Santos (MBWay=Sim); confirmar Sim | Coluna MBWay passa a **Não** |
| T78 | Desativado bloqueia receção | Após T77, login Maria; tentar receber MBWay | Maria não aparece na lista de destinatários |

---

### T.14 BackOffice — Desbloquear conta

*Conta 123456 bloqueada (executar T11→T13 primeiro)*

| ID | Descrição | Passos | Resultado Esperado |
|----|-----------|--------|--------------------|
| T79 | Sem seleção | Clicar Desbloquear | Aviso: "Selecione uma conta." |
| T80 | Desbloquear | João Silva (Id 1) → Desbloquear | "Conta desbloqueada." BD: Tentativas=0, Bloqueada=FALSE |
| T81 | Já desbloqueada | Selecionar conta normal; desbloquear | Sucesso (UPDATE sem erro) |
| T82 | Login após desbloqueio | Conta=123456, PIN=1234 | Abre frmMultibanco normalmente |

---

### Resumo da cobertura

| Área | Testes | IDs |
|------|--------|-----|
| Autenticação | 10 | T01–T10 |
| Bloqueio de conta | 8 | T11–T18 |
| Levantamento | 8 | T19–T26 |
| Depósito | 4 | T27–T30 |
| Transferência | 7 | T31–T37 |
| MBWay | 10 | T38–T47 |
| Pagamento de Serviços | 7 | T48–T54 |
| Histórico + Filtro | 6 | T55–T60 |
| Relógio | 2 | T61–T62 |
| Alterar PIN | 4 | T63–T66 |
| BackOffice — Inserir | 3 | T67–T69 |
| BackOffice — Eliminar | 4 | T70–T73 |
| BackOffice — MBWay toggle | 5 | T74–T78 |
| BackOffice — Desbloquear | 4 | T79–T82 |
| **TOTAL** | **82** | |
