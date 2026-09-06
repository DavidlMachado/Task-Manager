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
- [ ] Address nullability (`Nullable`) warnings in `TaskService`.
