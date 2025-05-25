# PowerPals

## A Social Platform for Gym Enthusiasts

---

## **Project Background**

This application was a **personal project** developed during my "Basics of Programming in C#" course. I designed "PowerPals" as a custom assignment to explore web application development and data persistence.

---

### **Main Description**

**PowerPals** is a web application envisioned as a social platform for gym enthusiasts – imagine **"Twitter for gym-people."** Users can create profiles, **post updates on their personal walls**, and **follow** other users to see their content. This platform focuses on content sharing and community building, intentionally omitting direct messaging features to maintain a streamlined social feed experience.

The application is built using **Blazor** for the web interface, leveraging its capabilities for interactive client-side development with C#. For **persistent data storage**, "PowerPals" utilizes **Entity Framework** to interact with a **MSSQLServer database**, ensuring that user data and posts are reliably stored and retrieved.

---

## **Prerequisites**

To run PowerPals, you need:

- **MSSQLServer**: The application requires MSSQLServer to be installed and running on your machine. If you encounter issues starting the application, please verify your MSSQLServer installation and ensure it's operational.

---

## **Optional Steps**

- **Update Database Migrations**: To ensure your database schema is up-to-date with the latest migrations, open the Package Manager Console in Visual Studio and run the following command:
  ```powershell
  Update-Database
  ```

---

## **Running the Application**

To launch the application, navigate to the project directory in your terminal and use the standard Blazor commands:

```bash
dotnet run
```
