
---

# Client GUI Application

## Introduction

This client GUI application allows users to interact with an RSS proxy server. Users can specify the URL and port number when starting the client. Additionally, an RSS feed can be added to the proxy server via the GUI, and the application can fetch feeds from the server.

## Usage

### Client

To start the client, run the following command in the terminal:

```bash
./client <url:portNumber>
```

Replace `<url:portNumber>` with the URL and port number where the server is running. For example:

```bash
./client http://localhost:8080
```

### Server

To launch the server, run the following command in the terminal:

```bash
./server <portNumber>
```

Replace `<portNumber>` with the desired port number. For example:

```bash
./server 8080
```

## GUI Instructions

1. **Start the Client:**

   Launch the client application using the provided URL and port number.

2. **Add RSS Feed to Proxy Server:**

   - Open the GUI.
   - Look for an option to add an RSS feed.
   - Provide the URL of the RSS feed you want to add.
   - Add at elast one label. 
   - Confirm the addition.

3. **Fetch Feeds from Server:**

   - Use the GUI to initiate the fetch operation.
   - The client will retrieve the latest feeds from the proxy server.

## Notes

- Ensure the server is running before starting the client.
- The server and client communicate over the specified port number.
- Make sure to provide correct URL and port information when starting the client.

---
