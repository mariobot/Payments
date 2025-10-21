## Hyip-Payments.Web

This project is a web application for managing and processing payments in a High-Yield Investment Program (HYIP). It provides features for user registration, investment tracking, payment processing, and administrative management.

### Features
- User registration and authentication
- Investment tracking and management
- Payment processing
- Administrative dashboard
- Reporting and analytics
- Responsive design for mobile and desktop

### Technologies Used
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Bootstrap
- JavaScript
- HTML/CSS
- RESTful APIs

### Getting Started
for migrations

### Migrations

To apply migrations and update the database, use the following commands in the Package Manager Console:
```
PM> Add-Migration InitialCreate
PM> Update-Database
```

Context PaymentsDbContext
dotnet ef migrations add Add_Coins --project Hyip-Payments.Context --startup-project Hyip-Payments.Web/Hyip-Payments.Web --context PaymentsDbContext

dotnet ef update database --project Hyip-Payments.Context --startup-project Hyip-Payments.Web/Hyip-Payments.Web --context PaymentsDbContext

Context Application
dotnet ef database update --project Hyip-Payments.Web\\Hyip-Payments.Web --context ApplicationDbContext

Context App



Make sure to replace `InitialCreate` with a descriptive name for your migration.