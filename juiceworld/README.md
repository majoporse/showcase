# JuiceWorld

### Your trustworthy anabolic steroids online store 💪💪. Achieve your dream physique with minimal effort and questionable consequences! 🔥🔥

![Juice World Mascot](assets/grznar.jpg "Achieve your dream physique with minimal effort and questionable consequences!")

---

## **Project Background**

This application was developed as part of my studies, specifically for a course focused on **Systems and Applications Development in C#**. It represents an **e-commerce platform** for the sale of anabolic steroids, intended purely for **entertainment and educational purposes**.

The project is built using **C#** and leverages the **Entity Framework** for data management and **ASP.NET MVC** for the web application framework. Data persistence is handled by **PostgreSQL**, and local media files (such as product images) are managed using **Azure Blob Storage**. This architectural choice makes the application highly adaptable and easily deployable to the **Azure cloud**. For streamlined local development, the entire application is **containerized with Docker**, significantly simplifying the setup and development process.

---

## **Team "Cibulus 600mg" Members**

* Michal Bilanin
* Mário Hatalčík

---

## **Project Assignment**

The core objective was to create an electronic commerce platform for "JuiceWorld" to facilitate the sale of anabolic steroids. The platform was required to feature:

* A **user-friendly interface** for easy navigation and product browsing.
* **Advanced filtering capabilities** for products, based on categories (e.g., types of anabolics, usage methods) and manufacturers.
* **Customer accounts** allowing users to view their order history.
* **Administrator capabilities** to modify product details, categories, and manufacturers.

Additionally, the project required the code to be deployable on a test server after "Milestone 2," exclusively from the `master` branch to ensure reliability. Performance optimization was also a key requirement, addressed through the implementation of a **cache mechanism that refreshes every 10 minutes** to enhance response times and overall system efficiency.

---

## **Diagrams**

* **Use Case Diagram**:
    The use case diagram can be found on the project's wiki here: [https://gitlab.fi.muni.cz/xbilanin/juiceworld/-/wikis/Use-Case-Diagram](https://gitlab.fi.muni.cz/xbilanin/juiceworld/-/wikis/Use-Case-Diagram)

* **Entity Relationship Diagram**:
    The entity relationship diagram can be found on the project's wiki here: [https://gitlab.fi.muni.cz/xbilanin/juiceworld/-/wikis/Entity-Relationship-Diagram](https://gitlab.fi.muni.cz/xbilanin/juiceworld/-/wikis/Entity-Relationship-Diagram)

---

## **Modules of the System**

The system is modularized into the following components:

1.  **BusinessLayer**: Responsible for encapsulating the core business logic of the application.
2.  **BusinessLayer.Tests**: Contains unit and integration tests for the business layer.
3.  **Commons**: A shared library containing common classes and interfaces utilized across various parts of the system.
4.  **DataAccessLayer.EF**: Manages database operations, implemented using **Entity Framework Core**.
5.  **Infrastructure**: Provides foundational classes and interfaces for applying design patterns in an implementation-agnostic manner.
6.  **PresentationLayer.Mvc**: The **ASP.NET Core MVC** application serving as the front-end user interface.
7.  **TestUtilities.EF**: Contains utilities and helper objects specifically for testing Entity Framework-related functionalities.
8.  **WebApi**: Handles HTTP requests and responses, providing the API endpoints for the application.

---

## **Setup For Development**

The development environment can be set up either natively (with .NET installed) on the host machine or within a Dev Container.

### **Using Dev Container**

To launch the Dev Container, run:

```bash
docker compose run juiceworld-dev
```

### **General Setup Steps**

1.  **Clone the repository**.
2.  **Install NuGet packages**: Navigate to the project's root directory and run:
    ```bash
    dotnet restore
    ```
3.  **Configure Environment Variables**: Set up your `.env` file in the project's root directory. An `.env.example` file is provided as a template.
4.  **Start Databases**:
    * If you are developing in a Dev Container, both the logging and storage databases (PostgreSQL and MongoDB) should start automatically.
    * If they do not start automatically, or if you are not using the Dev Container, manually run the database containers from the project's root directory:
        ```bash
        docker compose up postgres mongodb
        ```
5.  **Run the Application**: After the databases are running, select one of the `*-development` launch profiles and execute:
    ```bash
    dotnet run --project WebApi --launch-profile <launch-profile>
    ```

---

### **Running Migrations**

To apply database migrations, ensure the `DB_CONNECTION_STRING` environment variable is set. Also, specify the correct project and startup project. An example command:

```bash
DB_CONNECTION_STRING="Host=localhost;Port=5432;Database=juiceworld;Username=postgres;Password=postgres" JWT_SECRET='bAafd@A7d9#@F4*V!LHZs#ebKQrkE6pad2f3kj34c3dXy@' dotnet ef migrations add <message> --project DataAccessLayer.EF --startup-project WebApi
```
*Remember to customize the connection string and provide a meaningful migration message.*

---

### **Updating The Database**

Updating the database follows a similar principle to running migrations. An example command:

```bash
DB_CONNECTION_STRING="Host=localhost;Port=5432;Database=juiceworld;Username=postgres;Password=postgres" JWT_SECRET='bAafd@A7d9#@F4*V!LHZs#ebKQrkE6pad2f3kj34c3dXy@' dotnet ef database update --project DataAccessLayer.EF --startup-project WebApi
```

---

## **Project Demo**

The final project is deployed and accessible at the following URL:

* [https://sypku.fetuje.me](https://sypku.fetuje.me)

### **Default Seeded Users**

For testing and demonstration purposes, the following user credentials are provided:

* **Admin**: `admin@example.com` / `password`
* **User**: `user@example.com` / `password`
```