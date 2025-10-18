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

<div align="center">
  <img src="https://github.com/user-attachments/assets/0bd1f95b-044b-4612-9f58-70cc9b1dc59a" alt="Homepage (full length)" width="60%">
   <br/>
  <img src="https://github.com/user-attachments/assets/924bb94b-117a-4e16-a19a-5976fb09aca5" alt="Account-Register" width="49%">
  <img src="https://github.com/user-attachments/assets/79637aa0-2950-4a78-b0a7-3d8312a46588" alt="Account-Login" width="49%">
  <br/>
  <img src="https://github.com/user-attachments/assets/d1916139-a51b-480a-b2e3-31d97f40e2ca" alt="Admins-Dashboard" width="49%">
  <img src="https://github.com/user-attachments/assets/0932b6be-3dd7-41e0-b8f6-7192208db1be" alt="Admins-Medecins" width="49%">
  <br/>
  <img src="https://github.com/user-attachments/assets/2029a965-ffc1-45a3-b6db-efb22dbb0c9d" alt="Medecins-Dashboard" width="49%">
  <img src="https://github.com/user-attachments/assets/2cc57b88-bb4e-49f4-b372-40446ade40c7" alt="Patients-DossierMedicals" width="49%">
</div>



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
