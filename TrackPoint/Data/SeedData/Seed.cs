using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
using TrackPoint.Models;

namespace TrackPoint.Data.SeedData
{
    public static class Seed
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            // Seed Categories (if empty)
            if (!await context.Category.AnyAsync())
            {
                var categories = new[]
                {
                    // Null category, assets are assigned this category when their current category is deleted
                    new Category
                    {
                        Name = "Unassigned",
                        Abbreviation = "UN",
                        RequiresApproval = true,
                        DefaultLoanPeriodDays = 0,
                        Description = "Needs a proper category",
                        ContainsConsumables = false
                    },
                    new Category
                    {
                        Name = "Laptop",
                        Abbreviation = "LAP",
                        RequiresApproval = false,
                        DefaultLoanPeriodDays = 14,
                        Description = "Portable computer",
                        ContainsConsumables = false
                    },
                    new Category
                    {
                        Name = "Monitor",
                        Abbreviation = "MON",
                        RequiresApproval = false,
                        DefaultLoanPeriodDays = 7,
                        Description = "Display screen",
                        ContainsConsumables = false
                    },
                    new Category
                    {
                        Name = "Smart TV",
                        Abbreviation = "TV",
                        RequiresApproval = false,
                        DefaultLoanPeriodDays = 7,
                        Description = "Smart television",
                        ContainsConsumables = false
                    },
                    new Category
                    {
                        Name = "Phone",
                        Abbreviation = "PHN",
                        RequiresApproval = false,
                        DefaultLoanPeriodDays = 14,
                        Description = "Mobile phone",
                        ContainsConsumables = false
                    },
                    new Category
                    {
                        Name = "Sound Bar",
                        Abbreviation = "SB",
                        RequiresApproval = false,
                        DefaultLoanPeriodDays = 7,
                        Description = "External speaker bar",
                        ContainsConsumables = false
                    },
                    new Category
{
    Name = "Vehicle",
    Abbreviation = "VEH",
    RequiresApproval = true,
    DefaultLoanPeriodDays = 3,
    Description = "Company vehicle for transportation",
    ContainsConsumables = false
},
new Category
{
    Name = "Outreach Supplies",
    Abbreviation = "OUT",
    RequiresApproval = true,
    DefaultLoanPeriodDays = 7,
    Description = "Materials used for outreach events and activities",
    ContainsConsumables = true
},
new Category
{
    Name = "Keyboard",
    Abbreviation = "KEY",
    RequiresApproval = false,
    DefaultLoanPeriodDays = 14,
    Description = "Computer input keyboard",
    ContainsConsumables = false
},
new Category
{
    Name = "Mouse",
    Abbreviation = "MOU",
    RequiresApproval = false,
    DefaultLoanPeriodDays = 14,
    Description = "Computer pointing device",
    ContainsConsumables = false
},
new Category
{
    Name = "AV System",
    Abbreviation = "AVS",
    RequiresApproval = true,
    DefaultLoanPeriodDays = 3,
    Description = "Audio-visual equipment system",
    ContainsConsumables = false
},
new Category
{
    Name = "Printer",
    Abbreviation = "PRN",
    RequiresApproval = true,
    DefaultLoanPeriodDays = 7,
    Description = "Printing device",
    ContainsConsumables = false
},
new Category
{
    Name = "Furniture",
    Abbreviation = "FUR",
    RequiresApproval = true,
    DefaultLoanPeriodDays = 30,
    Description = "Office or facility furniture",
    ContainsConsumables = false
},
new Category
{
    Name = "Dock",
    Abbreviation = "DOC",
    RequiresApproval = false,
    DefaultLoanPeriodDays = 14,
    Description = "Laptop docking station",
    ContainsConsumables = false
},
new Category
{
    Name = "Charger",
    Abbreviation = "CHR",
    RequiresApproval = false,
    DefaultLoanPeriodDays = 14,
    Description = "Power adapter or charging device",
    ContainsConsumables = false
},
new Category
{
    Name = "Webcam",
    Abbreviation = "WBC",
    RequiresApproval = false,
    DefaultLoanPeriodDays = 14,
    Description = "External video camera for computers",
    ContainsConsumables = false
},
                };

                context.Category.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Locations with North Florida cities (8 total) if empty
            if (!await context.Location.AnyAsync())
            {
                var locations = new[]
                {
                    // Null location, assets are assigned this locaton when their current location is deleted
                    new Location { Name = "Unassigned", Abbreviation = "UN" },
                    new Location { Name = "Jacksonville", Abbreviation = "JAX" },
                    new Location { Name = "Tallahassee", Abbreviation = "TLH" },
                    new Location { Name = "Pensacola", Abbreviation = "PNS" },
                    new Location { Name = "Gainesville", Abbreviation = "GNV" },
                    new Location { Name = "Panama City", Abbreviation = "PCB" },
                    new Location { Name = "St. Augustine", Abbreviation = "STA" },
                    new Location { Name = "Fernandina Beach", Abbreviation = "FNB" },
                    new Location { Name = "Destin", Abbreviation = "DST" }
                };

                context.Location.AddRange(locations);
                await context.SaveChangesAsync();
            }

            // Seed sample Assets (if empty) - create 30 total sample assets
            if (!await context.Asset.AnyAsync())
            {
                // Load categories and locations to reference their IDs and abbreviations
                var cats = await context.Category.ToListAsync();
                var locs = await context.Location.ToListAsync();

                Category laptopCat = cats.FirstOrDefault(c => c.Name == "Laptop") ?? cats.First();
                Category monitorCat = cats.FirstOrDefault(c => c.Name == "Monitor") ?? cats.First();
                Category tvCat = cats.FirstOrDefault(c => c.Name == "Smart TV") ?? cats.First();
                Category phoneCat = cats.FirstOrDefault(c => c.Name == "Phone") ?? cats.First();
                Category sbCat = cats.FirstOrDefault(c => c.Name == "Sound Bar") ?? cats.First();

                // handy lists for round-robin location assignment
                var locationList = locs.ToList();
                int locCount = locationList.Count;

                var now = DateTime.UtcNow;

                var assets = new List<Asset>
                {
                    // base 8 (earlier examples)
                    new Asset
                    {
                        AssetTag = "LAP-0001",
                        Make = "Dell",
                        Model = "XPS 13",
                        SerialNumber = "DLXPS13-001",
                        CategoryId = laptopCat.CategoryId,
                        LocationId = locationList[0].LocationId,
                        AssetStatus = "InStorage",
                        Notes = "Assigned to pool inventory",
                        StatusDate = now,
                        PurchaseDate = now.AddMonths(-2),
                        PurchasePrice = 1199.99m,
                        Vendor = "Dell Direct",
                        Condition = "Good",
                        WarrantyExpirationDate = now.AddYears(1)
                    },
                    new Asset
                    {
                        AssetTag = "LAP-0002",
                        Make = "Apple",
                        Model = "MacBook Pro 14",
                        SerialNumber = "MBP14-5678",
                        CategoryId = laptopCat.CategoryId,
                        LocationId = locationList[1].LocationId,
                        AssetStatus = "InUse",
                        Notes = "Issued to faculty",
                        StatusDate = now.AddDays(-10),
                        PurchaseDate = now.AddYears(-1),
                        PurchasePrice = 1999.00m,
                        Vendor = "Apple Store",
                        Condition = "Good",
                        WarrantyExpirationDate = now.AddYears(1)
                    },
                    new Asset
                    {
                        AssetTag = "MON-0101",
                        Make = "LG",
                        Model = "27UL850",
                        SerialNumber = "LG271-990",
                        CategoryId = monitorCat.CategoryId,
                        LocationId = locationList[3].LocationId,
                        AssetStatus = "InStorage",
                        Notes = "4K monitor",
                        StatusDate = now,
                        PurchaseDate = now.AddMonths(-6),
                        PurchasePrice = 449.50m,
                        Vendor = "Best Buy",
                        Condition = "Good",
                        WarrantyExpirationDate = now.AddYears(2)
                    },
                    new Asset
                    {
                        AssetTag = "TV-1001",
                        Make = "Samsung",
                        Model = "The Frame 55",
                        SerialNumber = "SAMF55-2233",
                        CategoryId = tvCat.CategoryId,
                        LocationId = locationList[5].LocationId,
                        AssetStatus = "InUse",
                        Notes = "Display in lobby",
                        StatusDate = now.AddMonths(-1),
                        PurchaseDate = now.AddYears(-2),
                        PurchasePrice = 1299.99m,
                        Vendor = "Samsung",
                        Condition = "Good",
                        WarrantyExpirationDate = now.AddYears(1)
                    },
                    new Asset
                    {
                        AssetTag = "PHN-2001",
                        Make = "Apple",
                        Model = "iPhone 14",
                        SerialNumber = "IP14-3344",
                        CategoryId = phoneCat.CategoryId,
                        LocationId = locationList[2].LocationId,
                        AssetStatus = "InUse",
                        Notes = "Staff phone",
                        StatusDate = now.AddMonths(-3),
                        PurchaseDate = now.AddMonths(-14),
                        PurchasePrice = 799.00m,
                        Vendor = "Carrier",
                        Condition = "Fair",
                        WarrantyExpirationDate = now.AddMonths(10)
                    },
                    new Asset
                    {
                        AssetTag = "SB-3001",
                        Make = "Bose",
                        Model = "Soundbar 700",
                        SerialNumber = "BOSE700-778",
                        CategoryId = sbCat.CategoryId,
                        LocationId = locationList[6].LocationId,
                        AssetStatus = "InStorage",
                        Notes = "For conference room upgrades",
                        StatusDate = now,
                        PurchaseDate = now.AddMonths(-1),
                        PurchasePrice = 699.95m,
                        Vendor = "Bose",
                        Condition = "New",
                        WarrantyExpirationDate = now.AddYears(1)
                    },
                    new Asset
                    {
                        AssetTag = "LAP-0003",
                        Make = "Lenovo",
                        Model = "ThinkPad X1",
                        SerialNumber = "LENX1-882",
                        CategoryId = laptopCat.CategoryId,
                        LocationId = locationList[7].LocationId,
                        AssetStatus = "PendingDeployment",
                        Notes = "Waiting for assignment",
                        StatusDate = now.AddDays(-5),
                        PurchaseDate = now.AddMonths(-1),
                        PurchasePrice = 1399.00m,
                        Vendor = "Lenovo",
                        Condition = "Good",
                        WarrantyExpirationDate = now.AddYears(2)
                    },
                    new Asset
                    {
                        AssetTag = "MON-0102",
                        Make = "Dell",
                        Model = "P2422H",
                        SerialNumber = "DLP2422-445",
                        CategoryId = monitorCat.CategoryId,
                        LocationId = locationList[4].LocationId,
                        AssetStatus = "InUse",
                        Notes = "Workstation monitor",
                        StatusDate = now.AddMonths(-2),
                        PurchaseDate = now.AddYears(-1),
                        PurchasePrice = 199.99m,
                        Vendor = "Dell",
                        Condition = "Good",
                        WarrantyExpirationDate = now.AddYears(1)
                    }
                };

                context.Asset.AddRange(assets);
                await context.SaveChangesAsync();
            }
        }
    }
}
