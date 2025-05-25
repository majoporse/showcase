running migrations: 
```bash
POSTGRES_CONNECTION_STRING="Host=localhost;Port=5432;Database=DTD;Username=postgres;Password=postgres" dotnet ef migrations add init --project Warehouse --startup-project DieselTimeDeliveries -- 'Host=localhost;Port=5432;Database=DTD;Username=postgres;Password=postgres'
```

applying migrations
```bash
POSTGRES_CONNECTION_STRING="Host=localhost;Port=5432;Database=DTD;Username=postgres;Password=postgres" dotnet ef database update --project Warehouse --startup-project DieselTimeDeliveries -- 'Host=localhost;Port=5432;Database=DTD;Username=postgres;Password=postgres'
```

drop database
```bash
POSTGRES_CONNECTION_STRING="Host=localhost;Port=5432;Database=DTD;Username=postgres;Password=postgres" dotnet ef database drop --project Warehouse --startup-project DieselTimeDeliveries -- 'Host=localhost;Port=5432;Database=DTD;Username=postgres;Password=postgres'
```
