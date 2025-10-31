# Music Course Purchase Web Application

## A full-stack web application for purchasing music courses built with ASP.NET Core API and Blazor Web App.


###### 🏗️ Technology Stack

## Backend

**Framework: ASP.NET Core API**
**ORM: Entity Framework Core**
**Database: SQL Server**
**Authentication: JWT Bearer Token**


## Frontend
**Technology: Blazor Web App**
**UI Framework: MudBlazor**
**Project Structure:**
* AdminApp - Administrative interface
* ClientApp - Customer-facing application

### 🚀 Features

* User registration and authentication
* Course browsing and selection
* Secure payment processing
* User dashboard
* Course enrollment management


### 📁 Project Structure

```bash
MusicCourseApp/
├── Backend/
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   ├── Services/
│   └── Migrations/
├── Frontend/
│   ├── ClientApp/
│   └── AdminApp/
└── Shared/
    └── Models/

```

## 🔧 Installation & Setup

### Prerequisites
1. Dot NET 8.0 SDK
2. SQL Server
3. VS Code

### Backend Setup
1. Navigate to the Backend directory
2. Update connection string in appsettings.json
3. Run database migrations:
```bash
dotnet ef database update
```
4. Run the API:
```bash
dotnet run
```

### Frontend Setup
1. Navigate to the Frontend directory
2. For Client App:
```bash
cd ClientApp
dotnet run
```
3. For Admin App:
```bash
cd AdminApp
dotnet run
```

## 🎯 Application Flow
Registration/Login: Users register or login with email and password

Course Browsing: Browse available music courses

Course Selection: Select desired music classes (e.g., Drum, "How to Become the Drum Expert")

Payment Processing: Complete payment for selected courses

Course Access: Gain access to purchased courses

## 📊 Database Entities

    Users

    Courses

    Categories

    Payments

    MethodPayment
    
    Checkout


## 🔐 Authentication

    JWT-based authentication

    Role-based access control (User/Admin)

    Secure password hashing

## 🛠️ Development

This project was developed as part of the final bootcamp project in collaboration with PT Sigma Cipta Utama through Taldio Web Developer Bootcamp.


## 👥 Team

Bootcamp participants from Taldio in collaboration with PT Sigma Cipta Utama.


Bootcamp: Taldio Web Developer Bootcamp
Collaboration: PT Sigma Cipta Utama
Technology: ASP.NET Core API + Entity Framework + SQL Server + Blazor Web App + MudBlazor

