# Package Delivery System

## A Scalable and Modular API for Logistics

---

## **Project Background**

This project was developed as a **group assignment** during my studies, specifically for a course focused on **Software Architecture**. The core objective was to design and implement an application with a strong emphasis on **scalability** and the adoption of **advanced architectural patterns**.

The application is built as a **modular monolith**, a strategic architectural choice that allows for very easy future decomposition into **microservices**. This design enables seamless and powerful scaling of the application as demand grows.

---

### **Main Description**

This Package Delivery System functions as a robust **API-only backend**, providing the necessary functionalities for managing package deliveries without a direct frontend.

The application is entirely written in **C#** and leverages **Wolverine**, an in-memory message bus framework, for efficient internal communication and event handling. A key feature of the system is its integration with **Google Maps API**, which allows for obtaining real-world geographical data and results, crucial for accurate package routing and delivery estimations.

Given its modular design and C# foundation, this system is primed for efficient development and deployment in modern cloud environments, offering a highly scalable solution for logistics operations.