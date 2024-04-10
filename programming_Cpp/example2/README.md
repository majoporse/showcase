# Network Management System

This program implements a simple network management system that allows users to create and manipulate networks consisting of endpoints, bridges, and routers. It provides functionality to serialize and deserialize networks, check for loops, and perform various network operations.

## Data Structures and Definitions:

- `network`: Represents a network consisting of nodes (endpoints, bridges, routers). It contains methods to add devices, parse connections, check for loops, and serialize/deserialize networks.
- `node`: Represents a network node and is inherited by endpoint, bridge, and router classes. It contains methods to connect/disconnect nodes, check reachability, and perform depth-first search (DFS) for loop detection.
- `comparator`: A custom comparator for comparing nodes based on their names.
- `node_type`: An enum class representing the type of node (endpoint, bridge, router).

## Usage:

The `main` function demonstrates various functionalities of the network management system:
- It creates networks with endpoints, bridges, and routers.
- It asserts the correctness of network configurations and connections.
- It serializes the networks into a string and deserializes them back.
- It performs assertions to validate the deserialized networks.
- Various test cases are provided to cover different scenarios of network configurations and operations.

## Serialization Format:

The serialization format for storing network information must satisfy the following criteria:
- Able to store any number of interconnected networks with routers.
- The result of deserialization from the serialized string must be indistinguishable from the original data.
- The content of the string should reflect the current state of input networks, regardless of the operations' sequence that led to their creation.
- The serialization and deserialization operations should be idempotent.

The serialization format is represented by the following functions:
- `serialize`: Serializes a list of networks into a string.
- `deserialize`: Deserializes a string into a list of networks.

## Additional Requirements:

To facilitate serialization, the following enhancements have been made to the existing code:
- Methods `add_bridge` and `add_router` now accept a parameter of type `std::string_view`, representing a non-empty identifier composed of digits and English letters.
- Additional methods `endpoints`, `bridges`, and `routers` are added to retrieve vectors of corresponding node types.
