var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DieselTimeDeliveries>("DieselTimeDeliveries");

builder.Build().Run();
