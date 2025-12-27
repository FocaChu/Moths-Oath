# Blueprint System - Guia do Desenvolvedor

## Estrutura de Manifests

O sistema de blueprints do MothsOath usa arquivos `manifest.json` para listar os blueprints disponíveis em cada pasta. Este sistema é **essencial para o WebAssembly** funcionar corretamente.

## Localizações dos Blueprints

Os blueprints existem em duas localizações:

1. **`MothsOath.Core\Data\Blueprints\`** - Usado quando rodando no servidor ou desktop (FileSystem)
2. **`MothsOath.BlazorUI\wwwroot\Data\Blueprints\`** - Usado quando rodando no WebAssembly (navegador)

### Pastas Disponíveis

- `Archetypes/` - Arquétipos de personagens
- `Cards/` - Cartas/habilidades
- `Diseases/` - Doenças
- `NPCs/` - NPCs e inimigos
- `Races/` - Raças de personagens

## Como Adicionar um Novo Blueprint

### Passo 1: Criar o arquivo JSON

Crie seu arquivo blueprint na pasta apropriada, por exemplo:
```
MothsOath.BlazorUI\wwwroot\Data\Blueprints\Races\vampire_race.json
```

### Passo 2: Atualizar o manifest.json

**IMPORTANTE**: Adicione o nome do arquivo no `manifest.json` da pasta correspondente.

Exemplo - `MothsOath.BlazorUI\wwwroot\Data\Blueprints\Races\manifest.json`:
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

### Passo 3: Manter ambas as localizações sincronizadas

?? **IMPORTANTE**: Sempre adicione o blueprint em **AMBAS** as localizações:

1. `MothsOath.Core\Data\Blueprints\[Pasta]\[arquivo].json`
2. `MothsOath.BlazorUI\wwwroot\Data\Blueprints\[Pasta]\[arquivo].json`

E atualize ambos os manifests:

1. `MothsOath.Core\Data\Blueprints\[Pasta]\manifest.json`
2. `MothsOath.BlazorUI\wwwroot\Data\Blueprints\[Pasta]\manifest.json`

### Passo 4: Não é necessário recompilar!

? Após atualizar os arquivos e manifests, o sistema automaticamente carregará os novos blueprints na próxima execução.

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

### 1. Criar arquivos JSON

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

**`MothsOath.BlazorUI\wwwroot\Data\Blueprints\NPCs\vampire.json`**
(mesmo conteúdo acima)

### 2. Atualizar manifests

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

**`MothsOath.BlazorUI\wwwroot\Data\Blueprints\NPCs\manifest.json`**
(mesmo conteúdo acima)

### 3. Executar e testar

? O novo vampiro será carregado automaticamente!

## Troubleshooting

### "Blueprint não está sendo carregado no WebAssembly"

?? Verifique se o arquivo está em `MothsOath.BlazorUI\wwwroot\Data\Blueprints\`
?? Verifique se o nome está correto no `manifest.json`
?? Verifique se o arquivo JSON é válido (use um validador JSON online)
?? Limpe o cache do navegador (Ctrl+Shift+Delete)

### "InvalidOperationException: LoadAllBlueprintsFromFiles não pode ser usado no WebAssembly"

Este erro ocorre quando algum código está tentando usar o método síncrono `LoadAllBlueprintsFromFiles()` no WebAssembly. Use sempre `LoadAllBlueprintsFromFilesAsync()` para compatibilidade.

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
