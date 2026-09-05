# TaskManager

A learning project in C# — a simple task manager, built to prepare for an internship at Checkmarx (SCA team), with the goal of practicing language fundamentals, encapsulation best practices, and .NET project architecture concepts.

## Solution structure

The project is organized into three separate projects within a single solution (`.sln`), following the principle of separation of concerns:

```
TaskManagerSolution/
├── TaskManager.Core/     # Business logic (no dependency on UI or tests)
├── TaskManager.Gui/      # Graphical interface built with Blazor Server
└── TaskManager.Tests/    # Unit tests (xUnit)
```

### TaskManager.Core

Contains the pure business logic, with no dependency on any UI or presentation layer.

- **`TaskItem`** — represents a single task. Has `Id` (auto-generated via `Guid`), `Name`, `Description`, and `Status` (enum). Validates `Name`/`Description` against null or empty values in the constructor. Exposes `ConcludeTask()` to mark the task as completed (idempotent, returns `bool` indicating whether a change occurred) and an overridden `ToString()` for a readable representation.
- **`TaskItemStatus`** — enum with the states `Pending` and `Concluded`.
- **`TaskService`** — manages an in-memory collection of tasks. Exposes:
  - `AddTask(TaskItem)` — adds a new task (uses `TryAdd`, does not throw on `Id` collision).
  - `RemoveTask(string taskId)` — removes a task by `Id`.
  - `GetTask(string taskId, out TaskItem foundTask)` — looks up a task by `Id`.
  - `TaskList` — read-only exposure (`IReadOnlyDictionary`) of the tasks, preventing external code from modifying the internal collection directly.

### TaskManager.Gui

Graphical interface built with **Blazor Server** (chosen because the development environment is Linux, where WPF/WinForms/MAUI desktop are not available).

- Allows adding new tasks through a form (name + description).
- Lists existing tasks, with a visual indication of status.
- Allows completing or removing tasks directly from the list.
- `TaskService` is registered as a singleton (`AddSingleton`) to keep the in-memory data alive for the duration of the app's execution (note: without a database, data is lost on restart).

### TaskManager.Tests

Unit test project (xUnit) for validating the behavior of `TaskService` and `TaskItem`.

## Running the project

**GUI:**
```bash
cd TaskManager.Gui
dotnet run
```
Then open the URL shown in the terminal (e.g. `http://localhost:5002`) in your browser.

**Tests:**
```bash
cd TaskManager.Tests
dotnet test
```

**Build everything:**
```bash
dotnet build
```
(from the solution root)

## Design decisions

- **Encapsulated task collection**: `TaskService` exposes tasks through `IReadOnlyDictionary`, backed by a private internal `Dictionary` field — prevents external code from modifying the collection without going through the class's methods.
- **Enum instead of bool for status**: `TaskItemStatus` (instead of a simple `bool Status`) for clarity and future extensibility (e.g. `InProgress`).
- **Consistent `bool` returns**: all action methods (`AddTask`, `RemoveTask`, `ConcludeTask`) return `bool` to indicate success/failure, instead of throwing exceptions or failing silently.
- **Argument validation**: consistent use of `ArgumentException.ThrowIfNullOrWhiteSpace` and `ArgumentNullException.ThrowIfNull` throughout the codebase.

## Next steps

- [ ] Add persistence with a database (deciding between MySQL or NoSQL/MongoDB).
- [ ] Introduce the Repository pattern (`ITaskRepository`) to decouple `TaskService` from the storage mechanism.
- [ ] Expand unit test coverage.
- [ ] Address nullability (`Nullable`) warnings in `TaskService`.# TaskManager

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
