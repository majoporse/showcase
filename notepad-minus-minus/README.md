# Notepad--

## A text editor with additional features developed in Rust

---
## Demo 

https://github.com/user-attachments/assets/2730d91f-5da0-404a-aa59-157cf0484b90

---
## **Project Background**

This application was developed as a **group project** during my software development studies, specifically for a course focused on the **Rust programming language**. Our team consisted of three developers. While my individual contribution was part of a collaborative effort, I'm proud to showcase this project as I believe it demonstrates strong teamwork and a successful implementation.

We chose **Dioxus**, a relatively new Rust framework that was in its beta phase at the time, for its promising capabilities in building **desktop applications**. Its potential to deliver a robust and efficient user experience aligned perfectly with our project goals.

---

### **Main Description**

Notepad-- is a **desktop text editor** designed to provide comprehensive text file handling and advanced features. The application supports basic file operations such as **opening folders** and **creating various file types**. Users can interact with the editor using both **mouse and keyboard controls**.

Key functionalities include customizable **undo/redo steps** (configurable in settings) and standard editor features like **keyboard shortcuts**. These shortcuts facilitate efficient navigation (e.g., arrow key movement, Ctrl + arrow key jumps), text selection (Shift + arrow keys), and common operations like **copy, paste, cut, undo, and redo**. Additionally, the editor includes a robust **find and replace** functionality.

---

### **Implemented Functionality**

* **General Editor Functionality**: Core text editing capabilities.

* **File Handling**: Comprehensive operations including opening, creating, renaming, and deleting files.

* **Automatic File Explorer Update**: Ensures the file explorer view stays synchronized with changes.

* **Undo/Redo**: Revert and reapply changes with customizable history depth.

* **History of Recent Changes**: Tracks modifications for easy reference.

* **Keyboard Shortcuts**: Extensive support for efficient navigation and operations.

* **Find**: Quickly locate specific text within documents.

* **(Multiple) Terminal Support**: Integrate and manage multiple terminal sessions within the application.

---

## **Running the Application**

To get Notepad-- up and running, follow these steps:

1.  **Install Dioxus CLI**:
    ```bash
    cargo install dioxus-cli
    ```
    (Alternatively, refer to the official Dioxus getting started guide for more detailed instructions: [https://dioxuslabs.com/learn/0.6/getting_started/](https://dioxuslabs.com/learn/0.6/getting_started/))

2.  **Run the application**:
    ```bash
    dx serve -r
    ```

