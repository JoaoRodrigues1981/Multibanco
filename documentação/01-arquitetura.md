# 1. Arquitetura — As 3 Camadas

O projeto tem **3 camadas separadas**. Cada camada só fala com a do lado — nunca saltam camadas.

```
┌──────────────────────────────────────┐
│  _01Apresentacao  (formulários)      │  ← o que o utilizador VÊ e clica
│  Form1.cs, frmAutenticacao.cs,       │
│  frmAdmin.cs, frmMBWay.cs, etc.      │
└────────────────┬─────────────────────┘
                 │ chama métodos de
┌────────────────▼─────────────────────┐
│  _02Controlo  (cControlo.cs)         │  ← valida, decide, coordena
│  Nunca faz SQL diretamente           │
└────────────────┬─────────────────────┘
                 │ chama métodos de
┌────────────────▼─────────────────────┐
│  _03Dados  (cLogin, cAdmin,          │  ← ÚNICA camada que fala com a BD
│   cMovimento, cServico, cMBWay)      │
└──────────────────────────────────────┘
```

**Regra de ouro:** os formulários NUNCA veem SQL. O cControlo NUNCA abre ligações à BD.

## Classes por camada

| Camada | Pasta | Classes |
|--------|-------|---------|
| Apresentação | `_01Apresentacao` | `frmAutenticacao`, `Form1` (frmMultibanco), `frmAdmin`, `frmMBWay`, `frmPagamentosServicos`, `frmUpdate`, `frmSimuladorEmprestimo` |
| Controlo | `_02Controlo` | `cControlo` |
| Dados | `_03Dados` | `cConexao`, `cLogin`, `cAdmin`, `cMovimento`, `cMBWay`, `cServico` |
