# 7. Dados de Teste

> Correr `criar_bd.sql` para repor sempre os dados limpos antes de testar:
	[[02 ISLA/1.2_Algoritmos e Estruturas Dados/ArqEstruturaDados/Multibanco/Multibanco_v3/criar_bd.sql]]
## Contas

| Id | Banco | Cliente | Conta | PIN | MBWay | Telefone | Saldo |
|----|-------|---------|-------|-----|-------|----------|-------|
| 1 | CGD | João Silva | 123456 | 1234 | Não | 912345001 | 850,00€ |
| 2 | CGD | João Silva | 123457 | 1234 | Não | 912345002 | 200,00€ |
| 3 | BPI | Maria Santos | 654321 | 4321 | **Sim** | 963215001 | 1500,00€ |
| 4 | BCP | Rui Costa | 111222 | 9999 | Não | 935671001 | 300,00€ |
| 5 | BES | Ana Lima | 777888 | 5555 | **Sim** | 916780001 | 500,00€ |
| 6 | Santander | Carlos Mendes | 222333 | 2233 | **Sim** | 961234567 | 750,00€ |
| 7 | Millennium | Sofia Pereira | 444555 | 4455 | **Sim** | 936789012 | 1200,00€ |
| 8 | Novo Banco | Miguel Ferreira | 666777 | 6677 | Não | 912398765 | 450,00€ |
| 9 | Santander | Inês Oliveira | 888999 | 8899 | **Sim** | 964321987 | 2000,00€ |
| 10 | CGD | Tiago Rodrigues | 321654 | 3216 | Não | 935678901 | 100,00€ |

**Admin:** `sibs` / PIN `1111`

## Notas rápidas para testes

- **Duas contas do mesmo cliente:** João Silva tem 123456 e 123457
- **Contas com MBWay:** Id 3, 5, 6, 7, 9 (Maria, Ana, Carlos, Sofia, Inês)
- **Testar bloqueio:** 3 PINs errados em 123456 → bloqueada → desbloquear no BackOffice
- **Testar eliminação:** criar conta nova via BackOffice → levantar 100€ → eliminar
