# Fileversionator2000

## A Version Tracking Tool for Local Directories

---

## **Project Background**

This project served as a technical assignment from my previous company, designed to assess my proficiency with the **C# programming language** and the **ASP.NET MVC framework**. It was an opportunity to demonstrate practical application development skills under specific constraints.

---

### **Main Description**

Fileversionator2000 is conceived as a **tracking tool for local system directories**. Its primary function is to monitor changes within a specified directory, operating in a manner analogous to Git, but with a simpler scope: it tracks file versions and metadata rather than the actual content of the files.

A key requirement of this assignment was that the application **could not utilize a traditional relational database**. Instead, all application data, including version history and configuration, must be **serialized and deserialized to and from a dedicated configuration file**. This constraint necessitated a different approach to data persistence and management, focusing on file-based storage solutions.