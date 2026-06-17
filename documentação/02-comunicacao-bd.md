# 2. Como a app comunica com a BD

## 2.1 A classe cConexao

Está em `_03Dados/cConexao.cs`. É a única classe que sabe a string de ligação ao PostgreSQL.

```csharp
public NpgsqlConnection conectar()
{
    oSqlConexao = new NpgsqlConnection("Host=localhost;Database=Multibanco;Username=postgres;Password=...");
    oSqlConexao.Open();
    return oSqlConexao;
}

public void desConectar()
{
    oSqlConexao.Close();
}
```

Todas as classes em `_03Dados` têm `cConexao oConexao = new cConexao();` e usam `oConexao.conectar()` / `oConexao.desConectar()`.

---

## 2.2 SELECT — ler dados (ExecuteReader)

```csharp
// Exemplo: verificar se conta está bloqueada (cLogin.cs)
NpgsqlCommand oCmd = new NpgsqlCommand();

// 1. Parâmetros — NUNCA concatenar strings (SQL Injection!)
oCmd.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));

// 2. SQL
oCmd.CommandText = "SELECT Bloqueada FROM Credenciais WHERE Conta = @Conta";

// 3. Ligação
oCmd.Connection = oConexao.conectar();

// 4. Executar — devolve leitor linha a linha
NpgsqlDataReader oReader = oCmd.ExecuteReader();

// 5. Ler
if (oReader.Read())
{
    bool bloqueada = oReader.GetBoolean(0);  // coluna 0 = Bloqueada
}

// 6. Fechar pela ordem certa: reader → ligação → comando
oReader.Close();
oConexao.desConectar();
oCmd.Dispose();
```

**Métodos do DataReader por tipo:**

| Tipo PostgreSQL | Método C# | Exemplo |
|-----------------|-----------|---------|
| INTEGER | `GetInt32(i)` | `oReader.GetInt32(0)` |
| BOOLEAN | `GetBoolean(i)` | `oReader.GetBoolean(0)` |
| VARCHAR / TEXT | `GetString(i)` | `oReader.GetString(1)` |
| DECIMAL/NUMERIC | `GetDecimal(i)` | `oReader.GetDecimal(4)` |
| TIMESTAMP | `GetDateTime(i)` | `oReader.GetDateTime(3)` |

**`oReader.HasRows`** — `true` se houver pelo menos uma linha.

---

## 2.3 INSERT / UPDATE / DELETE — escrever dados (ExecuteNonQuery)

```csharp
NpgsqlCommand oCmd = new NpgsqlCommand();
oCmd.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));

oCmd.CommandText = @"UPDATE Credenciais 
                     SET Tentativas = Tentativas + 1,
                         Bloqueada  = CASE WHEN Tentativas + 1 >= 3 THEN TRUE ELSE Bloqueada END
                     WHERE Conta = @Conta";

oCmd.Connection = oConexao.conectar();
oCmd.ExecuteNonQuery();   // não devolve linhas
oConexao.desConectar();
oCmd.Dispose();
```

---

## 2.4 Porquê parâmetros `@` e não concatenação?

**NUNCA:**
```csharp
oCmd.CommandText = "SELECT * FROM Credenciais WHERE Cliente = '" + txtCliente + "'";
// ' OR '1'='1  → SQL Injection!
```

**SEMPRE:**
```csharp
oCmd.Parameters.AddWithValue("@Cliente", txtCliente);
oCmd.CommandText = "SELECT * FROM Credenciais WHERE Cliente = @Cliente";
// Npgsql trata sempre como dado — 100% seguro.
```

---

## 2.5 O helper fExecutarNonQuery (cAdmin.cs)

Evita repetição de `try/catch` em todos os UPDATEs e DELETEs:

```csharp
private bool fExecutarNonQuery(NpgsqlCommand oCmd, string prefixoErro)
{
    try
    {
        oCmd.Connection = oConexao.conectar();
        oCmd.ExecuteNonQuery();
        oConexao.desConectar();
        oCmd.Dispose();
    }
    catch (Exception ex)
    {
        operacao = false;
        mensagem = prefixoErro + ex.Message;
    }
    return operacao;
}
```

---

## 2.6 Como os dados chegam ao formulário

```
BD → NpgsqlDataReader → variáveis em cLogin (saldo, banco, cliente...)
                      → copiadas pelo cControlo
                      → lidas pelo formulário (txtSaldo.Text = oControlo.saldo)
```

Para listas, o tipo é sempre `List<string[]>`:
- Cada `string[]` = uma linha da BD
- O formulário percorre com `foreach` e adiciona ao `ListView`
