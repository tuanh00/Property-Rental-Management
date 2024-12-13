# Property Rental Management System

This repository contains the **Property Rental Management System**, a web-based application developed using **ASP.NET MVC** and **Entity Framework**. It is designed to streamline operations for **Owners**, **Managers**, and **Tenants**, offering secure and efficient management of rental properties.

---

## 📝 Project Description

The **Property Rental Management System** provides the following features:
- **Role-Specific Dashboards** for Owners, Managers, and Tenants.
- **Session-Based Access Control** for enhanced security.
- **CRUD Operations** for managing buildings, apartments, events, messages, and appointments.

### Key Technologies Used:
- **ASP.NET MVC Framework**: For building structured and scalable web applications.
- **Entity Framework (Database First Approach)**: For efficient database management.
- **SQL Server Management Studio (SSMS) 19**: For database design and queries.

---

## 📋 Features by User Role

### **Owner**
- Manage Managers and Tenants (CRUD).
- View and search Buildings and Apartments.
- Manage Events and Messages.
- Update their profile or delete their account.

### **Manager**
- Manage Buildings, Apartments, Events, and Appointments (CRUD).
- Communicate with Owners and Tenants via Messages.
- Update their profile or delete their account.

### **Tenant**
- Register for an account.
- Search and view Apartments.
- Book Appointments with Managers.
- Communicate with Managers via Messages.
- Update their profile or delete their account.

---

## 📂 Folder Structure

- **`script.sql`**: SQL script to initialize the database.
- **`imgs/`**: Folder containing screenshots and diagrams.
- **`2231473_HuynhTuAnhChau_Report.docx`**: Project report.

---

## 🚀 Deployment

1. Install **Visual Studio 2022** and **SQL Server Management Studio (SSMS) 19**.
2. Import the `script.sql` file into SSMS to set up the database.
3. Open the project in Visual Studio and run the application.

---

## 📸 Screenshots

### Home Page
<img src="./imgs/Homepage.png" alt="Home Page" width="500px"/>

### Owner Login
<img src="./imgs/OwnerLogin.png" alt="Owner Login" width="500px"/>

### Manager Dashboard
<img src="./imgs/ManagerResponse.png" alt="Manager Dashboard" width="500px"/>

### Manager - Apartments Management
<img src="./imgs/ManagerApartments.png" alt="Manager Apartments" width="500px"/>

### Manager - Buildings Management
<img src="./imgs/ManagerBuildings.png" alt="Manager Buildings" width="500px"/>

### Owner - Events Management
<img src="./imgs/OwnerEvents.png" alt="Owner Events" width="500px"/>

### Owner - Manager Communication
<img src="./imgs/OwnerManagerMsg.png" alt="Owner Manager Messages" width="500px"/>

### System Diagram
<img src="./imgs/Diagram.png" alt="System Diagram" width="500px"/>

---

## 📋 Sample User Credentials

For testing purposes, you can use the following credentials:

| **Role** | **Email**              | **Password** |
|----------|------------------------|--------------|
| Owner    | tylerdurden@gmail.com  | password123  |
| Manager  | ednamode@gmail.com     | password123  |
| Tenant   | michaelcorleone@gmail.com | password123 |

---

## 📖 Conclusion

This project provided hands-on experience with:
- **ASP.NET MVC** and **Entity Framework** for building robust web applications.
- Designing responsive user interfaces.
- Implementing secure session-based access control.
