# Notepad--

## A text editor with additional features developed in Rust

---

## **Project Background**

This application was developed as a **group project** during my software development studies, specifically for a course focused on the **Rust programming language**. Our team consisted of three developers. While my individual contribution was part of a collaborative effort, I'm proud to showcase this project as I believe it demonstrates strong teamwork and a successful implementation.

We chose **Dioxus**, a relatively new Rust framework that was in its beta phase at the time, for its promising capabilities in building **desktop applications**. Its potential to deliver a robust and efficient user experience aligned perfectly with our project goals.

---

### **Main Description**

Notepad-- is a **desktop text editor** designed to provide comprehensive text file handling and advanced features. The application supports basic file operations such as **opening folders** and **creating various file types**. Users can interact with the editor using both **mouse and keyboard controls**.

Key functionalities include customizable **undo/redo steps** (configurable in settings) and standard editor features like **keyboard shortcuts**. These shortcuts facilitate efficient navigation (e.g., arrow key movement, Ctrl + arrow key jumps), text selection (Shift + arrow keys), and common operations like **copy, paste, cut, undo, and redo**. Additionally, the editor includes a robust **find and replace** functionality.

A unique feature of Notepad-- is its **"live share" mode**. This mode allows users to create a watch session, generating a unique code and password. Other instances of the application can then connect to this session to monitor code in real-time. The implementation of this mode (whether cloud-based or P2P) was left to our discretion. The application also supports **multiple UI themes** for a personalized user experience.

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

