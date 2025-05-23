## Wpf_OnlineRestaurantSystem

A modern, interactive WPF desktop application for managing a restaurant's digital operations — including ordering, inventory, users, and administrative controls.

This system is backed by a SQL Server database created and managed using **SQL Server Management Studio (SSMS)**. All database interactions are performed using **stored procedures**, with **ADO.NET** ensuring efficient communication between the application and the database.

## 📦 Features

### 👤 User Interface
- Secure login (admin/customer)
- View dishes by category, search and filter
- Add/remove items from cart
- Place orders with shipping cost calculation
- Track real-time order statuses
![image](https://github.com/user-attachments/assets/eb66e4be-1900-4d07-b679-71e45dc90c6e)
![image](https://github.com/user-attachments/assets/a5a96ef6-a76b-4912-9197-8098f34f4f53)

### 🔐 Admin Panel
- Admin dashboard for managing:
  - Dishes (add, edit, delete)
  - Orders and statuses
  - Users
- Dish allergen management
- Cancel and update orders
![image](https://github.com/user-attachments/assets/7dab716b-d08d-4c20-a1dd-2136a8441808)


### 💾 Database Integration
- Microsoft SQL Server used to store:
  - Users
  - Dishes
  - Orders
  - Inventory
- All operations (insert, update, delete, read) are executed via **stored procedures**
- Connected using **ADO.NET**

---

## 🛠️ Technologies

- **C#**
- **WPF (.NET Framework)**
- **MVVM Architecture**
- **XAML** (UI design)
- **SQL Server Management Studio (SSMS)**
- **ADO.NET** (for data access)
- **Stored Procedures** (performance and security)

---

## 🗃️ Database Setup

### Requirements:
- SQL Server Management Studio (SSMS)


## Installation
Clone the project:

bash
```
git clone https://github.com/Iulia-plesu/Wpf_OnlineRestaurantSystem.git
```

Open Wpf_OnlineRestaurantSystem.sln in Visual Studio.

Create and initialize the SQL database using the instructions above.

Set the correct SQL connection string in App.config.

Build and run the application.

## 🗃️ Database Setup

### Requirements:
- SQL Server (Express or Developer edition)
- SQL Server Management Studio (SSMS)

### Steps:

1. Open SSMS and connect to your local SQL Server instance.
2. Create a new database, e.g., `OnlineRestaurantDB`.
3. Run the provided SQL script (`Database/RestaurantScript.sql`) to create tables and stored procedures.
4. Update the connection string in your app config file:

```xml
<connectionStrings>
    	<add name="myConStr"
connectionString="Data Source=DESKTOP-LD6FL3O;Initial Catalog=RestaurantAppDB;Integrated Security=True;TrustServerCertificate=True;" />
</connectionStrings>
```
