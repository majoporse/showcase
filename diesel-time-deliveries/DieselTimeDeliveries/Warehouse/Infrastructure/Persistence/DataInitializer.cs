using Microsoft.EntityFrameworkCore;

namespace Warehouse.Infrastructure.Persistence;

public static class DataInitializer
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Courier>().HasData(
        //     Courier.Create("Fero", "feri@zoznam.sk").Value,
        //     Courier.Create("Jozo", "jozko@gmail.com").Value,
        //     Courier.Create("Miro", "mirci_raketa@turbodiesel.cz").Value
        // );
        //
        // modelBuilder.Entity<Vehicle>().HasData(
        //     Vehicle.Create("Mazda", "mx5", 23.5m, "BA666XX").Value,
        //     Vehicle.Create("Ford", "Focus", 45.0m, "KE123AB").Value,
        //     Vehicle.Create("Skoda", "Octavia", 48.5m, "TT000TT").Value,
        //     Vehicle.Create("Volkswagen", "Golf", 50.0m, "KE123AB").Value
        // );

        // modelBuilder.Entity<Package>().HasData(
        //     Package.Create("Package1", 10.5m, "Destination1").Value,
        //     Package.Create("Package2", 5.0m, "Destination2").Value,
        //     Package.Create("Package3", 7.25m, "Destination3").Value
        // );
    }
}