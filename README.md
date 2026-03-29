# QueueCalc — Queuing Theory Calculator
### C# WinForms · .NET 8 · DCS-UOK

---

## Project Structure

```
QueueCalc/
├── QueueCalc.csproj   ← .NET 8 WinForms project file
├── Program.cs          ← Entry point
├── MainForm.cs         ← All UI (tabs, inputs, buttons, results)
└── QueueEngine.cs      ← All queuing math (models below)
```

---

## How to Build & Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
- Windows OS (WinForms is Windows-only)

### Steps
```bash
# 1. Navigate to project folder
cd QueueCalc

# 2. Build
dotnet build

# 3. Run
dotnet run
```

Or open in **Visual Studio 2022** → File → Open → Project → select `QueueCalc.csproj`

---

## Supported Queuing Models

| Arrivals | Service | Servers | Model        | Formula                         |
|----------|---------|---------|------------- |---------------------------------|
| M        | M       | 1       | M/M/1        | Exact (standard formulas)       |
| M        | M       | c       | M/M/c        | Exact (Erlang-C / P₀)           |
| M        | G       | 1       | M/G/1        | Pollaczek-Khinchine             |
| G        | M       | 1       | G/M/1        | Marshall's approximation        |
| G        | G       | 1       | G/G/1        | Marshall's approximation        |
| G/M      | G/M     | c       | G/G/c        | Whitt (1976) approximation      |

### Supported Distributions for G
- **Gamma** — provide mean and variance
- **Normal** — provide mean and variance
- **Uniform** — provide min and max (mean and variance computed automatically)

---

## Output Metrics

| Symbol | Description                        |
|--------|------------------------------------|
| L_q    | Mean number of customers in queue  |
| W_q    | Mean waiting time in queue (mins)  |
| W      | Mean time in system (mins)         |
| L      | Mean number in system              |
| ρ      | Traffic intensity (utilization)    |
| Idle   | Proportion of time server is idle (P₀ for multi-server) |

---

## Example (matches notes Example 1)
- Arrival: M, mean = 10 min  
- Service: M, mean = 8 min  
- Servers: 1  
→ L_q = 3.2, W_q = 32 min, W = 40 min, L = 4, Idle = 20%
