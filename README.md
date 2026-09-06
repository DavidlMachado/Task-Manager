# TaskManager

A learning project in C# — a simple task manager, built to prepare for an internship at Checkmarx (SCA team), with the goal of practicing language fundamentals, encapsulation best practices, and .NET project architecture concepts.

## Solution structure

The project is organized into three separate projects within a single solution (`.sln`), following the principle of separation of concerns:

```
TaskManagerSolution/
├── TaskManager.Core/     # Business logic and persistence (no dependency on UI or tests)
├── TaskManager.Gui/      # Graphical interface built with Blazor Server
└── TaskManager.Tests/    # Unit tests (xUnit)
```

### TaskManager.Core

Contains the business logic and data access, with no dependency on any UI or presentation layer.

- **`TaskItem`** — represents a single task. Has `Id` (auto-generated via `Guid`, mapped to MongoDB's `_id` via `[BsonId]`), `Name`, `Description`, and `Status` (enum). Validates `Name`/`Description` against null or empty values in the constructor. Exposes `ConcludeTask()` to mark the task as completed (idempotent, returns `bool` indicating whether a change occurred) and an overridden `ToString()` for a readable representation. Note: `ConcludeTask()` only mutates the in-memory object — callers must explicitly persist the change via `TaskService.UpdateTask(...)`.
- **`TaskItemStatus`** — enum with the states `Pending` and `Concluded`.
- **`ITaskRepository`** — interface (contract) defining the storage operations a task repository must support: `AddTask`, `RemoveTask`, `GetTask`, `GetAllTasks`, and `UpdateTask`. Decouples `TaskService` from any specific storage technology.
- **`InMemoryTaskRepository`** — in-memory implementation of `ITaskRepository`, backed by a `Dictionary<string, TaskItem>`. Useful for quick testing without external dependencies.
- **`MongoTaskRepository`** — MongoDB-backed implementation of `ITaskRepository`, using the official `MongoDB.Driver` package. Connects to a local MongoDB instance (`mongodb://localhost:27017`), database `TaskManagerDB`, collection `TasksCollection`. Uses strongly-typed filters (`Builders<TaskItem>.Filter`) for all queries, avoiding NoSQL injection risks associated with hand-built query strings.
- **`TaskService`** — the single entry point used by consumers (GUI, tests). Validates arguments (`ArgumentException.ThrowIfNullOrWhiteSpace`, `ArgumentNullException.ThrowIfNull`) and delegates storage operations to an injected `ITaskRepository`. Exposes:
  - `AddTask(TaskItem)` — adds a new task.
  - `RemoveTask(string taskId)` — removes a task by `Id`.
  - `GetTask(string taskId, out TaskItem foundTask)` — looks up a task by `Id`.
  - `GetAllTasks()` — returns all tasks as `IEnumerable<TaskItem>`.
  - `UpdateTask(TaskItem)` — persists changes made to an existing task (e.g. after calling `ConcludeTask()`).

### TaskManager.Gui

Graphical interface built with **Blazor Server** (chosen because the development environment is Linux, where WPF/WinForms/MAUI desktop are not available).

- Allows adding new tasks through a form (name + description).
- Lists existing tasks, with a visual indication of status.
- Allows completing or removing tasks directly from the list.
- `ITaskRepository` (currently `MongoTaskRepository`) and `TaskService` are registered as singletons via dependency injection in `Program.cs`. Swapping storage implementations only requires changing one line in `Program.cs`.
- Error handling distinguishes between validation errors (`ArgumentException`, e.g. empty task name) and infrastructure errors (`Exception`, e.g. database unreachable), showing an appropriate message in each case.

### TaskManager.Tests

Unit test project (xUnit) for validating the behavior of `TaskService` and `TaskItem`, currently exercised against `InMemoryTaskRepository`.

- **Planned**: integration tests for `MongoTaskRepository` against a real MongoDB instance, using a dedicated test database/collection to avoid polluting real data.

## Persistence

Task data is persisted in **MongoDB**, running locally via Docker:

```bash
docker run -d -p 27017:27017 --name mongo-taskmanager mongo:latest
```

Inspect stored data directly:
```bash
docker exec -it mongo-taskmanager mongosh
use TaskManagerDB
db.TasksCollection.find().pretty()
```

## Running the project

**Prerequisite:** MongoDB container must be running (see above).

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

- **Repository pattern**: `TaskService` depends on the `ITaskRepository` abstraction rather than a concrete storage implementation, allowing storage to be swapped (in-memory ↔ MongoDB) without changing business logic, GUI, or test code that consumes `TaskService`.
- **Encapsulated collections**: repositories expose data through interfaces (`IReadOnlyDictionary` in the original design, now `IEnumerable<TaskItem>` via `GetAllTasks()`), preventing external code from bypassing the service layer.
- **Enum instead of bool for status**: `TaskItemStatus` (instead of a simple `bool Status`) for clarity and future extensibility (e.g. `InProgress`).
- **Consistent `bool` returns**: all action methods (`AddTask`, `RemoveTask`, `UpdateTask`, `ConcludeTask`) return `bool` to indicate success/failure, instead of throwing exceptions or failing silently for expected outcomes (e.g. "task not found").
- **Argument validation centralized in `TaskService`**: repositories stay simple and focused only on storage; validation logic isn't duplicated across implementations.
- **Selective exception handling**: expected outcomes (e.g. duplicate key on insert, no document found on delete/update) are represented via `bool`/return values rather than exceptions. Truly exceptional conditions (e.g. database connection failures) are allowed to propagate up to the GUI layer, where they're caught and translated into a user-friendly message — distinct from validation error messages.
- **NoSQL injection avoidance**: all MongoDB queries use the strongly-typed `Builders<TaskItem>.Filter` API rather than hand-built query strings, ensuring user input is always treated as a value, never as a query operator.

## Next steps

- [ ] Add integration tests for `MongoTaskRepository` (dedicated test database/collection, setup/teardown per test).
- [ ] Make MongoDB connection string/database/collection names configurable (currently hardcoded in `MongoTaskRepository`'s constructor) instead of injected via constructor parameters.
- [ ] Address remaining nullability (`Nullable`) warnings.
- [ ] Consider adding a global exception handler in the Blazor app for unhandled infrastructure errors.# TaskManager

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
- [ ] Address nullability (`Nullable`) warnings in `TaskService`.
