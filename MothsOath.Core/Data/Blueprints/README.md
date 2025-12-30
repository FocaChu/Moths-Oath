# Blueprint System - Guia do Desenvolvedor

## Estrutura de Manifests

O sistema de blueprints do MothsOath usa arquivos `manifest.json` para listar os blueprints disponíveis em cada pasta. Este sistema é **essencial para o WebAssembly** funcionar corretamente.

## ? Fonte Única de Dados

**IMPORTANTE**: Todos os blueprints devem ser editados **APENAS** em:
```
MothsOath.Core\Data\Blueprints\
```

Os arquivos em `MothsOath.BlazorUI\wwwroot\Data\Blueprints\` são **gerados automaticamente** durante o build e **NÃO devem ser editados manualmente**.

## Como o Sistema Funciona

### Em Desenvolvimento (FileSystem)
Quando rodando localmente, o sistema lê os arquivos diretamente de `MothsOath.Core\Data\Blueprints\`.

### Em WebAssembly (Navegador)
Como o navegador não tem acesso ao sistema de arquivos, o MSBuild **copia automaticamente** todos os blueprints para `wwwroot` durante a compilação, tornando-os acessíveis via HTTP.

### Pastas Disponíveis

- `Archetypes/` - Arquétipos de personagens
- `Cards/` - Cartas
- `Diseases/` - Doenças
- `NPCs/` - Aliados e inimigos
- `Races/` - Raças de personagens

## Como Adicionar um Novo Blueprint

### Passo 1: Criar o arquivo JSON

Crie seu arquivo blueprint na pasta apropriada em **MothsOath.Core**, por exemplo:
```
MothsOath.Core\Data\Blueprints\Races\vampire_race.json
```

### Passo 2: Atualizar o manifest.json

**IMPORTANTE**: Adicione o nome do arquivo no `manifest.json` da pasta correspondente.

Exemplo - `MothsOath.Core\Data\Blueprints\Races\manifest.json`:
```json
{
  "files": [
    "ghoul_race.json",
    "human_race.json",
    "yulkin_race.json",
    "vampire_race.json"  // <- Adicione aqui
  ]
}
```

### Passo 3: Recompilar

Execute um clean build para garantir que todos os arquivos sejam copiados:
```bash
dotnet clean
dotnet build
```

### Passo 4: Reiniciar a Aplicação

?? **IMPORTANTE**: 
- Se testando no navegador, faça um **hard refresh** (Ctrl+Shift+R ou Ctrl+F5)
- Se executando via Visual Studio, **pare e reinicie** completamente a aplicação
- O BlueprintCache é inicializado apenas no startup da aplicação

### ? Pronto!

Após reiniciar, o sistema automaticamente carregará os novos blueprints.

## ?? Problemas Comuns

### "Adicionei um blueprint mas não aparece"

**Causa**: Cache do navegador ou BlueprintCache não foi reinicializado.

**Solução**:
1. Certifique-se de ter executado `dotnet build` após adicionar o arquivo
2. **Pare completamente** a aplicação
3. Limpe o cache do navegador (Ctrl+Shift+Delete)
4. Faça um **hard refresh** (Ctrl+Shift+R ou Ctrl+F5)
5. **Reinicie** a aplicação

### "Arquivo não está no wwwroot após build"

**Solução**:
1. Execute `dotnet clean`
2. Execute `dotnet build` novamente
3. Verifique se o arquivo existe em `MothsOath.Core\Data\Blueprints\`
4. Verifique se o nome está correto no `manifest.json`

## Fallback Automático (FileSystem)

Quando rodando em modo FileSystem (servidor/desktop), se o `manifest.json` não existir ou não puder ser lido, o sistema automaticamente descobrirá todos os arquivos `*.json` na pasta (exceto o próprio `manifest.json`).

?? **No WebAssembly**, este fallback NÃO existe. O manifest é **obrigatório**.

## Estrutura do manifest.json

```json
{
  "files": [
    "arquivo1.json",
    "arquivo2.json",
    "arquivo3.json"
  ]
}
```

### Regras

- ? Use apenas nomes de arquivo (não caminhos completos)
- ? Inclua a extensão `.json`
- ? Um arquivo por linha
- ? Não adicione o próprio `manifest.json` na lista
- ? Não adicione comentários no JSON

## Exemplo Completo

Vamos adicionar um novo NPC chamado "Vampire":

### 1. Criar arquivo JSON

**`MothsOath.Core\Data\Blueprints\NPCs\vampire.json`**
```json
{
  "Id": "vampire",
  "Name": "Vampiro",
  "Description": "Um vampiro sedento por sangue",
  "Health": 100,
  "MaxHealth": 100,
  "AttackPower": 15
}
```

### 2. Atualizar manifest

**`MothsOath.Core\Data\Blueprints\NPCs\manifest.json`**
```json
{
  "files": [
    "narrator_extra.json",
    "neko.json",
    "skeleton.json",
    "vampire.json"
  ]
}
```

### 3. Compilar

```bash
dotnet clean
dotnet build
```

### 4. Reiniciar aplicação e limpar cache

- Parar aplicação completamente
- Limpar cache do navegador
- Fazer hard refresh (Ctrl+Shift+R)
- Reiniciar aplicação

? O novo vampiro será carregado automaticamente!

## Troubleshooting

### "Blueprint não está sendo carregado no WebAssembly"

?? Verifique se o arquivo está em `MothsOath.Core\Data\Blueprints\` (não no wwwroot!)
?? Verifique se o nome está correto no `manifest.json`
?? Verifique se o arquivo JSON é válido (use um validador JSON online)
?? Execute `dotnet clean && dotnet build` para copiar os arquivos para wwwroot
?? **PARE E REINICIE** completamente a aplicação
?? Limpe o cache do navegador (Ctrl+Shift+Delete)
?? Faça um hard refresh (Ctrl+Shift+R ou Ctrl+F5)

### "InvalidOperationException: LoadAllBlueprintsFromFiles não pode ser usado no WebAssembly"

Este erro ocorre quando algum código está tentando usar o método síncrono `LoadAllBlueprintsFromFiles()` no WebAssembly. Use sempre `LoadAllBlueprintsFromFilesAsync()` para compatibilidade.

### "Arquivos no wwwroot estão desatualizados"

Execute `dotnet clean` seguido de `dotnet build` para forçar uma recópia completa.

## Código de Exemplo

### Carregando Blueprints (correto)

```csharp
// Async (funciona em FileSystem E WebAssembly)
var blueprints = await blueprintLoader.LoadAllBlueprintsFromFilesAsync<RaceBlueprint>("Races");

// Síncrono (apenas FileSystem)
var blueprints = blueprintLoader.LoadAllBlueprintsFromFiles<RaceBlueprint>("Races");
```

### Criando States com Blueprints

Se seu state precisa carregar blueprints no construtor, use o padrão async factory:

```csharp
public class MyState : IGameState
{
    private readonly List<MyBlueprint> _blueprints;

    // Construtor privado
    private MyState(List<MyBlueprint> blueprints)
    {
        _blueprints = blueprints;
    }

    // Método factory público e assíncrono
    public static async Task<MyState> CreateAsync(BlueprintLoader loader)
    {
        var blueprints = await loader.LoadAllBlueprintsFromFilesAsync<MyBlueprint>("MyFolder");
        return new MyState(blueprints.Values.ToList());
    }
}
```

## Notas Técnicas

- O sistema usa `JsonSerializer` para deserialização
- A opção `PropertyNameCaseInsensitive = true` está habilitada
- Erros de deserialização são logados no console mas não interrompem o carregamento dos outros arquivos
- IDs duplicados são detectados e o segundo blueprint com o mesmo ID é ignorado
- O target MSBuild `CopyDataFilesToWwwroot` é executado automaticamente antes de cada build
- A pasta `wwwroot\Data\Blueprints\` é ignorada pelo Git (arquivos gerados)
- **BlueprintCache é inicializado apenas no startup** - mudanças em blueprints requerem reinicialização da aplicação
