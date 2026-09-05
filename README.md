# TaskManager

Projeto de aprendizagem em C# — um gestor de tarefas simples, criado como preparação para um estágio na Checkmarx (equipa de SCA), com o objetivo de praticar fundamentos da linguagem, boas práticas de encapsulamento, e conceitos de arquitetura de projetos .NET.

## Estrutura da solução

O projeto está organizado em três projetos separados dentro de uma solução (`.sln`), seguindo o princípio de separação de responsabilidades:

```
TaskManager/
├── TaskManager.Core/     # Lógica de negócio (sem dependências de UI ou testes)
├── TaskManager.Gui/      # Interface gráfica em Blazor Server
└── TaskManager.Tests/    # Testes unitários (xUnit)
```

### TaskManager.Core

Contém a lógica de negócio pura, sem qualquer dependência de interface ou forma de apresentação.

- **`TaskItem`** — representa uma tarefa individual. Tem `Id` (gerado automaticamente via `Guid`), `Name`, `Description`, e `Status` (enum). Valida `Name`/`Description` contra valores nulos ou vazios no construtor. Expõe `ConcludeTask()` para marcar a tarefa como concluída (idempotente, devolve `bool` a indicar se houve alteração) e um `ToString()` sobrescrito para representação legível.
- **`TaskItemStatus`** — enum com os estados `Pending` e `Concluded`.
- **`TaskService`** — gere uma coleção de tarefas em memória. Expõe:
  - `AddTask(TaskItem)` — adiciona uma tarefa nova (usa `TryAdd`, não lança exceção em caso de colisão de `Id`).
  - `RemoveTask(string taskId)` — remove uma tarefa pelo `Id`.
  - `GetTask(string taskId, out TaskItem foundTask)` — procura uma tarefa pelo `Id`.
  - `TaskList` — exposição só de leitura (`IReadOnlyDictionary`) das tarefas, para evitar que código externo modifique a coleção interna diretamente.

### TaskManager.Gui

Interface gráfica construída em **Blazor Server** (escolhida por o ambiente de desenvolvimento ser Linux, onde WPF/WinForms/MAUI desktop não estão disponíveis).

- Permite adicionar novas tarefas através de um formulário (nome + descrição).
- Lista as tarefas existentes, com indicação visual de estado.
- Permite concluir ou remover tarefas diretamente na lista.
- `TaskService` é registado como singleton (`AddSingleton`) para persistir os dados em memória durante a execução da aplicação (nota: sem base de dados, os dados são perdidos ao reiniciar a app).

### TaskManager.Tests

Projeto de testes unitários (xUnit) para validar o comportamento de `TaskService` e `TaskItem`.

## Como correr o projeto

**Interface gráfica:**
```bash
cd TaskManager.Gui
dotnet run
```
Depois abrir o URL indicado no terminal (ex: `http://localhost:5002`) no browser.

**Testes:**
```bash
cd TaskManager.Tests
dotnet test
```

**Compilar tudo:**
```bash
dotnet build
```
(a partir da raiz da solução)

## Decisões de design

- **Encapsulamento da coleção de tarefas**: `TaskService` expõe as tarefas através de `IReadOnlyDictionary`, mantendo um campo privado `Dictionary` interno — evita que código externo modifique a coleção sem passar pelos métodos da classe.
- **Enum em vez de bool para estado**: `TaskItemStatus` (em vez de um simples `bool Status`) para maior clareza e possibilidade de expansão futura (ex: `InProgress`).
- **Retorno consistente de `bool`**: todos os métodos de ação (`AddTask`, `RemoveTask`, `ConcludeTask`) devolvem `bool` para indicar sucesso/falha da operação, em vez de lançar exceções ou falhar silenciosamente.
- **Validação de argumentos**: uso de `ArgumentException.ThrowIfNullOrWhiteSpace` e `ArgumentNullException.ThrowIfNull` para validação consistente em toda a codebase.

## Próximos passos

- [ ] Adicionar persistência com base de dados (a decidir entre MySQL ou NoSQL/MongoDB).
- [ ] Introduzir o padrão Repository (`ITaskRepository`) para desacoplar `TaskService` da forma de armazenamento.
- [ ] Expandir cobertura de testes unitários.
- [ ] Rever tratamento de nulabilidade (`Nullable` warnings) em `TaskService`.
