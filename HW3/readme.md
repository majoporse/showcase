<!-- HOW TO ACTIVATE CLIENT: 
----------------------
./client <name> <ipv4 address of the server>

SUPPORTED COMMANDS ON CLIENT AND SERVER:
---------------------------------------
get 
    total
        income
        profit
        movement
    vat-by
        profit
        added
    unpaid vat
add
    company <name> <short name> <ico>
    invoie <id> <ico_exhibitor> <ico_customer> <price> <vat>
    vat-payment <id> <type> <price> -->

```markdown
# Client Activation and Supported Commands

## How to Activate Client

To activate the client, use the following command in your terminal:

```bash
./client <name> <ipv4 address of the server>
```

Replace `<name>` with the desired name and `<ipv4 address of the server>` with the appropriate IPv4 address of the server.

## Supported Commands on Client and Server

The client and server support the following commands:

### Get

- #### Total
  - `income`
  - `profit`
  - `movement`

### Vat-By

- #### Profit
- #### Added

### Unpaid Vat

### Add

- #### Company
  - Syntax: `company <name> <short name> <ico>`

- #### Invoice
  - Syntax: `invoice <id> <ico_exhibitor> <ico_customer> <price> <vat>`

- #### VAT Payment
  - Syntax: `vat-payment <id> <type> <price>`

## Usage Example

### Activate Client

```bash
./client JohnDoe 192.168.1.100
```

### Get Total Income

```bash
./client get total income
```

### Add a Company

```bash
./client add company ABCCompany ABC ABC123
```

### Add an Invoice

```bash
./client add invoice 001 ABCExhibitor XYZCustomer 100.00 20.00
```

### Make a VAT Payment

```bash
./client add vat-payment 001 online 30.00
```
