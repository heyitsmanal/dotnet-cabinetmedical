# CabinetMedical

_A minimal medical cabinet management app built with ASP.NET Core (MVC) on .NET 8._  
Focus: patients, appointments, and basic admin workflows.

![Status](https://img.shields.io/badge/status-active-success)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

---

## ✨ Features
- Patient CRUD (create, read, update, delete)
- Appointment scheduling & listing
- Basic search/filtering
- Validation and error handling
- (Extend here: prescriptions, billing, auth, roles…)

## 🧰 Tech Stack
- **Backend:** ASP.NET Core MVC (.NET 8)
- **Views:** Razor
- **Data:** Entity Framework Core (SQLite/SQL Server)
- **Tooling:** Git, PowerShell, dotnet CLI

## 🖼️ Demo Screenshots





## 🚀 Quick Start

> Prereqs: [.NET SDK 8](https://dotnet.microsoft.com/en-us/download)

```powershell
# 1) Clone
git clone https://github.com/heyitsmanal/dotnet-cabinetmedical.git
cd dotnet-cabinetmedical

# 2) Restore & build
dotnet restore
dotnet build

# 3) Run the web app
dotnet run --project .\CabinetMedical\CabinetMedical.csproj
# App will print a URL like http://localhost:5xxx — open it in the browser
