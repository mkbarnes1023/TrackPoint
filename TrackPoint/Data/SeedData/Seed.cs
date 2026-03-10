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
                    }
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
    AssetTag = "LAP-0101",
    Make = "Dell",
    Model = "Latitude 7420",
    SerialNumber = "DL7420-101",
    CategoryId = 1003,
    LocationId = 1,
    AssetStatus = "InUse",
    Notes = "Assigned to IT staff",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-14),
    PurchasePrice = 1299.99m,
    Vendor = "Dell",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(20)
},

new Asset
{
    AssetTag = "LAP-0102",
    Make = "HP",
    Model = "EliteBook 840",
    SerialNumber = "HP840-102",
    CategoryId = 1003,
    LocationId = 2,
    AssetStatus = "InStorage",
    Notes = "Backup laptop",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-10),
    PurchasePrice = 1099.99m,
    Vendor = "HP",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(55)
},

new Asset
{
    AssetTag = "MON-0201",
    Make = "LG",
    Model = "UltraFine 27",
    SerialNumber = "LG27-201",
    CategoryId = 1004,
    LocationId = 3,
    AssetStatus = "PendingDeployment",
    Notes = "New monitor shipment",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-3),
    PurchasePrice = 349.99m,
    Vendor = "Amazon",
    Condition = "New",
    WarrantyExpirationDate = now.AddDays(75)
},

new Asset
{
    AssetTag = "MON-0202",
    Make = "Samsung",
    Model = "Odyssey G5",
    SerialNumber = "SG5-202",
    CategoryId = 1004,
    LocationId = 4,
    AssetStatus = "InUse",
    Notes = "",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-16),
    PurchasePrice = 399.99m,
    Vendor = "Best Buy",
    Condition = "Good",
    WarrantyExpirationDate = now.AddYears(1)
},

new Asset
{
    AssetTag = "PHN-0301",
    Make = "Apple",
    Model = "iPhone 14",
    SerialNumber = "IPH14-301",
    CategoryId = 1006,
    LocationId = 5,
    AssetStatus = "InUse",
    Notes = "Manager phone",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-11),
    PurchasePrice = 999.99m,
    Vendor = "Apple",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(10)
},

new Asset
{
    AssetTag = "PHN-0302",
    Make = "Samsung",
    Model = "Galaxy S22",
    SerialNumber = "SGS22-302",
    CategoryId = 1006,
    LocationId = 6,
    AssetStatus = "Lost",
    Notes = "Reported missing",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-18),
    PurchasePrice = 799.99m,
    Vendor = "Samsung",
    Condition = "Unknown",
    WarrantyExpirationDate = now.AddDays(40)
},

new Asset
{
    AssetTag = "TV-0401",
    Make = "Sony",
    Model = "Bravia 65",
    SerialNumber = "SB65-401",
    CategoryId = 1005,
    LocationId = 7,
    AssetStatus = "InUse",
    Notes = "Conference room display",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-20),
    PurchasePrice = 1299.99m,
    Vendor = "Best Buy",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(65)
},

new Asset
{
    AssetTag = "TV-0402",
    Make = "Samsung",
    Model = "Crystal UHD",
    SerialNumber = "UHD-402",
    CategoryId = 1005,
    LocationId = 8,
    AssetStatus = "OnHold",
    Notes = "Waiting for installation",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-5),
    PurchasePrice = 699.99m,
    Vendor = "Amazon",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddYears(2)
},

new Asset
{
    AssetTag = "VEH-0501",
    Make = "Ford",
    Model = "Transit Van",
    SerialNumber = "FTV-501",
    CategoryId = 1008,
    LocationId = 2,
    AssetStatus = "InUse",
    Notes = "Outreach transport",
    StatusDate = now,
    PurchaseDate = now.AddYears(-3),
    PurchasePrice = 35000m,
    Vendor = "Ford",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(25)
},

new Asset
{
    AssetTag = "VEH-0502",
    Make = "Toyota",
    Model = "Highlander",
    SerialNumber = "TH-502",
    CategoryId = 1008,
    LocationId = 3,
    AssetStatus = "UnderMaintenance",
    Notes = "Scheduled maintenance",
    StatusDate = now,
    PurchaseDate = now.AddYears(-4),
    PurchasePrice = 38000m,
    Vendor = "Toyota",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(50)
},

new Asset
{
    AssetTag = "OUT-0601",
    Make = "Banner",
    Model = "Event Banner",
    SerialNumber = "BN-601",
    CategoryId = 1009,
    LocationId = 3,
    AssetStatus = "InStorage",
    Notes = "Outreach materials",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-24),
    PurchasePrice = 150m,
    Vendor = "VistaPrint",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(70)
},

new Asset
{
    AssetTag = "OUT-0602",
    Make = "Folding Table",
    Model = "Portable Event Table",
    SerialNumber = "FT-602",
    CategoryId = 1009,
    LocationId = 5,
    AssetStatus = "Returned",
    Notes = "",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-30),
    PurchasePrice = 85m,
    Vendor = "Walmart",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddYears(1)
},

// --- more laptops / monitors for variety ---

new Asset
{
    AssetTag = "LAP-0103",
    Make = "Apple",
    Model = "MacBook Pro 14",
    SerialNumber = "MBP-103",
    CategoryId = 1003,
    LocationId = 4,
    AssetStatus = "NeedsReplacement",
    Notes = "Battery degrading",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-30),
    PurchasePrice = 1999.99m,
    Vendor = "Apple",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(15)
},

new Asset
{
    AssetTag = "LAP-0104",
    Make = "Lenovo",
    Model = "ThinkPad X1",
    SerialNumber = "TPX1-104",
    CategoryId = 1003,
    LocationId = 6,
    AssetStatus = "Transfered",
    Notes = "Transferred to another office",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-9),
    PurchasePrice = 1499.99m,
    Vendor = "Lenovo",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(45)
},

new Asset
{
    AssetTag = "MON-0203",
    Make = "Dell",
    Model = "UltraSharp 32",
    SerialNumber = "DU32-203",
    CategoryId = 1004,
    LocationId = 7,
    AssetStatus = "InStorage",
    Notes = "",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-6),
    PurchasePrice = 499.99m,
    Vendor = "Dell",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(85)
},

new Asset
{
    AssetTag = "MON-0204",
    Make = "Acer",
    Model = "Predator XB3",
    SerialNumber = "ACXB3-204",
    CategoryId = 1004,
    LocationId = 8,
    AssetStatus = "Retired",
    Notes = "Replaced by new monitor",
    StatusDate = now,
    PurchaseDate = now.AddYears(-5),
    PurchasePrice = 399.99m,
    Vendor = "Acer",
    Condition = "Poor",
    WarrantyExpirationDate = now.AddYears(-1)
},
new Asset
{
    AssetTag = "LAP-0105",
    Make = "Dell",
    Model = "Latitude 5530",
    SerialNumber = "DLL5530-105",
    CategoryId = 1003,
    LocationId = 1,
    AssetStatus = "PendingDeployment",
    Notes = "Prepared for new employee onboarding",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-2),
    PurchasePrice = 1249.99m,
    Vendor = "Dell Direct",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(12)
},

new Asset
{
    AssetTag = "LAP-0106",
    Make = "HP",
    Model = "ProBook 450 G9",
    SerialNumber = "HPPB450-106",
    CategoryId = 1003,
    LocationId = 2,
    AssetStatus = "InUse",
    Notes = "Assigned to regional office staff",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-11),
    PurchasePrice = 989.50m,
    Vendor = "HP Business",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(38)
},

new Asset
{
    AssetTag = "LAP-0107",
    Make = "Lenovo",
    Model = "ThinkPad E15",
    SerialNumber = "LNVE15-107",
    CategoryId = 1003,
    LocationId = 3,
    AssetStatus = "InStorage",
    Notes = "Spare unit kept in storage",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-7),
    PurchasePrice = 879.00m,
    Vendor = "Lenovo",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(67)
},

new Asset
{
    AssetTag = "LAP-0108",
    Make = "Apple",
    Model = "MacBook Air M2",
    SerialNumber = "MBAIR-108",
    CategoryId = 1003,
    LocationId = 4,
    AssetStatus = "Transfered",
    Notes = "Transferred from Pensacola to Gainesville office",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-13),
    PurchasePrice = 1299.00m,
    Vendor = "Apple",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddYears(1)
},

new Asset
{
    AssetTag = "LAP-0109",
    Make = "Acer",
    Model = "TravelMate P2",
    SerialNumber = "ACTMP2-109",
    CategoryId = 1003,
    LocationId = 5,
    AssetStatus = "NeedsReplacement",
    Notes = "Frequent overheating reported",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-28),
    PurchasePrice = 749.99m,
    Vendor = "CDW",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(21)
},

new Asset
{
    AssetTag = "MON-0205",
    Make = "Dell",
    Model = "P2422H",
    SerialNumber = "DLP24-205",
    CategoryId = 1004,
    LocationId = 6,
    AssetStatus = "InUse",
    Notes = "Front desk workstation monitor",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-15),
    PurchasePrice = 229.99m,
    Vendor = "Dell Direct",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(49)
},

new Asset
{
    AssetTag = "MON-0206",
    Make = "LG",
    Model = "24MP60G",
    SerialNumber = "LG24-206",
    CategoryId = 1004,
    LocationId = 7,
    AssetStatus = "OnHold",
    Notes = "Waiting on replacement HDMI cable",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-9),
    PurchasePrice = 179.99m,
    Vendor = "Amazon",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(73)
},

new Asset
{
    AssetTag = "MON-0207",
    Make = "Samsung",
    Model = "ViewFinity S6",
    SerialNumber = "SAMS6-207",
    CategoryId = 1004,
    LocationId = 8,
    AssetStatus = "PendingDeployment",
    Notes = "Received and ready for imaging lab",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-1),
    PurchasePrice = 319.99m,
    Vendor = "Best Buy",
    Condition = "New",
    WarrantyExpirationDate = now.AddYears(2)
},

new Asset
{
    AssetTag = "MON-0208",
    Make = "AOC",
    Model = "24B2XH",
    SerialNumber = "AOC24-208",
    CategoryId = 1004,
    LocationId = 1,
    AssetStatus = "Retired",
    Notes = "Backlight dimming issue",
    StatusDate = now,
    PurchaseDate = now.AddYears(-4),
    PurchasePrice = 139.99m,
    Vendor = "Staples",
    Condition = "Poor",
    WarrantyExpirationDate = now.AddMonths(-8)
},

new Asset
{
    AssetTag = "MON-0209",
    Make = "HP",
    Model = "E24 G4",
    SerialNumber = "HPE24-209",
    CategoryId = 1004,
    LocationId = 2,
    AssetStatus = "Returned",
    Notes = "Returned from temporary event setup",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-14),
    PurchasePrice = 249.99m,
    Vendor = "HP Business",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(28)
},

new Asset
{
    AssetTag = "PHN-0303",
    Make = "Apple",
    Model = "iPhone SE",
    SerialNumber = "IPSE-303",
    CategoryId = 1006,
    LocationId = 3,
    AssetStatus = "InStorage",
    Notes = "Backup mobile device",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-20),
    PurchasePrice = 429.99m,
    Vendor = "Apple",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(34)
},

new Asset
{
    AssetTag = "PHN-0304",
    Make = "Google",
    Model = "Pixel 8",
    SerialNumber = "PXL8-304",
    CategoryId = 1006,
    LocationId = 4,
    AssetStatus = "InUse",
    Notes = "Assigned to communications coordinator",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-5),
    PurchasePrice = 699.00m,
    Vendor = "Google Store",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(61)
},

new Asset
{
    AssetTag = "PHN-0305",
    Make = "Samsung",
    Model = "Galaxy A54",
    SerialNumber = "SGA54-305",
    CategoryId = 1006,
    LocationId = 5,
    AssetStatus = "Lost",
    Notes = "Lost during outreach event travel",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-17),
    PurchasePrice = 449.99m,
    Vendor = "T-Mobile",
    Condition = "Unknown",
    WarrantyExpirationDate = now.AddDays(16)
},

new Asset
{
    AssetTag = "PHN-0306",
    Make = "Motorola",
    Model = "Moto G Power",
    SerialNumber = "MGPWR-306",
    CategoryId = 1006,
    LocationId = 6,
    AssetStatus = "Returned",
    Notes = "Returned after employee departure",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-22),
    PurchasePrice = 299.99m,
    Vendor = "Verizon",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(82)
},

new Asset
{
    AssetTag = "TV-0403",
    Make = "LG",
    Model = "QNED 65",
    SerialNumber = "LGQN65-403",
    CategoryId = 1005,
    LocationId = 7,
    AssetStatus = "InUse",
    Notes = "Mounted in training room",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-12),
    PurchasePrice = 1099.99m,
    Vendor = "Best Buy",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(7)
},

new Asset
{
    AssetTag = "TV-0404",
    Make = "TCL",
    Model = "Roku 55",
    SerialNumber = "TCLR55-404",
    CategoryId = 1005,
    LocationId = 8,
    AssetStatus = "UnderMaintenance",
    Notes = "Intermittent power issue under review",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-19),
    PurchasePrice = 479.99m,
    Vendor = "Walmart",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(44)
},

new Asset
{
    AssetTag = "TV-0405",
    Make = "Sony",
    Model = "Bravia XR 75",
    SerialNumber = "SNY75-405",
    CategoryId = 1005,
    LocationId = 1,
    AssetStatus = "OnHold",
    Notes = "Waiting for wall bracket installation",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-4),
    PurchasePrice = 1799.99m,
    Vendor = "Sony",
    Condition = "New",
    WarrantyExpirationDate = now.AddDays(88)
},

new Asset
{
    AssetTag = "AVS-0701",
    Make = "Bose",
    Model = "Videobar VB1",
    SerialNumber = "BOSEVB1-701",
    CategoryId = 1012,
    LocationId = 2,
    AssetStatus = "InUse",
    Notes = "Conference room AV system",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-16),
    PurchasePrice = 1299.00m,
    Vendor = "Bose",
    Condition = "Good",
    WarrantyExpirationDate = now.AddYears(1)
},

new Asset
{
    AssetTag = "AVS-0702",
    Make = "Logitech",
    Model = "Rally Bar",
    SerialNumber = "LGRB-702",
    CategoryId = 1012,
    LocationId = 3,
    AssetStatus = "PendingDeployment",
    Notes = "Planned for executive board room",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-2),
    PurchasePrice = 3499.99m,
    Vendor = "CDW",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(19)
},

new Asset
{
    AssetTag = "AVS-0703",
    Make = "Poly",
    Model = "Studio X50",
    SerialNumber = "POLYX50-703",
    CategoryId = 1012,
    LocationId = 4,
    AssetStatus = "InStorage",
    Notes = "Stored after office renovation",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-26),
    PurchasePrice = 2899.99m,
    Vendor = "Insight",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(52)
},

new Asset
{
    AssetTag = "AVS-0704",
    Make = "Jabra",
    Model = "PanaCast 50",
    SerialNumber = "JAB50-704",
    CategoryId = 1012,
    LocationId = 5,
    AssetStatus = "NeedsReplacement",
    Notes = "Camera feed has recurring distortion",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-31),
    PurchasePrice = 1195.00m,
    Vendor = "Jabra",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(76)
},

new Asset
{
    AssetTag = "VEH-0503",
    Make = "Chevrolet",
    Model = "Express Van",
    SerialNumber = "CHVEXP-503",
    CategoryId = 1008,
    LocationId = 6,
    AssetStatus = "InUse",
    Notes = "Used for equipment transport",
    StatusDate = now,
    PurchaseDate = now.AddYears(-2),
    PurchasePrice = 33995.00m,
    Vendor = "Chevrolet",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(23)
},

new Asset
{
    AssetTag = "VEH-0504",
    Make = "Honda",
    Model = "Odyssey",
    SerialNumber = "HNODY-504",
    CategoryId = 1008,
    LocationId = 7,
    AssetStatus = "Returned",
    Notes = "Returned from temporary interoffice use",
    StatusDate = now,
    PurchaseDate = now.AddYears(-3),
    PurchasePrice = 36450.00m,
    Vendor = "Honda",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(58)
},

new Asset
{
    AssetTag = "VEH-0505",
    Make = "Ford",
    Model = "Escape",
    SerialNumber = "FRDESC-505",
    CategoryId = 1008,
    LocationId = 8,
    AssetStatus = "UnderMaintenance",
    Notes = "Brake inspection and service scheduled",
    StatusDate = now,
    PurchaseDate = now.AddYears(-4),
    PurchasePrice = 28750.00m,
    Vendor = "Ford",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(84)
},

new Asset
{
    AssetTag = "OUT-0603",
    Make = "Canopy",
    Model = "10x10 Event Tent",
    SerialNumber = "CNP1010-603",
    CategoryId = 1009,
    LocationId = 1,
    AssetStatus = "InStorage",
    Notes = "Stored with outreach supplies",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-18),
    PurchasePrice = 249.99m,
    Vendor = "Academy Sports",
    Condition = "Good",
    WarrantyExpirationDate = now.AddYears(2)
},

new Asset
{
    AssetTag = "OUT-0604",
    Make = "Yeti",
    Model = "Portable Cooler",
    SerialNumber = "YETI-604",
    CategoryId = 1009,
    LocationId = 2,
    AssetStatus = "InUse",
    Notes = "Used during community outreach events",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-9),
    PurchasePrice = 179.99m,
    Vendor = "Yeti",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddDays(14)
},

new Asset
{
    AssetTag = "OUT-0605",
    Make = "Display Stand",
    Model = "Tri-Fold Booth Stand",
    SerialNumber = "DSPSTD-605",
    CategoryId = 1009,
    LocationId = 3,
    AssetStatus = "OnHold",
    Notes = "Needs replacement signage panels",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-21),
    PurchasePrice = 94.50m,
    Vendor = "Office Depot",
    Condition = "Fair",
    WarrantyExpirationDate = now.AddDays(36)
},

new Asset
{
    AssetTag = "OUT-0606",
    Make = "Folding Chair Set",
    Model = "Event Seating Bundle",
    SerialNumber = "CHAIR-606",
    CategoryId = 1009,
    LocationId = 4,
    AssetStatus = "Transfered",
    Notes = "Transferred for regional outreach program",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-27),
    PurchasePrice = 210.00m,
    Vendor = "Sam's Club",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(69)
},

new Asset
{
    AssetTag = "LAP-0110",
    Make = "Microsoft",
    Model = "Surface Laptop 5",
    SerialNumber = "MSL5-110",
    CategoryId = 1003,
    LocationId = 5,
    AssetStatus = "Returned",
    Notes = "Returned after internship program ended",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-8),
    PurchasePrice = 1399.99m,
    Vendor = "Microsoft",
    Condition = "Excellent",
    WarrantyExpirationDate = now.AddYears(1)
},

new Asset
{
    AssetTag = "PHN-0307",
    Make = "Apple",
    Model = "iPhone 12",
    SerialNumber = "IPH12-307",
    CategoryId = 1006,
    LocationId = 6,
    AssetStatus = "Retired",
    Notes = "Retired from active mobile pool",
    StatusDate = now,
    PurchaseDate = now.AddYears(-3),
    PurchasePrice = 799.00m,
    Vendor = "Apple",
    Condition = "Poor",
    WarrantyExpirationDate = now.AddMonths(-10)
},

new Asset
{
    AssetTag = "AVS-0705",
    Make = "Epson",
    Model = "PowerLite Projector",
    SerialNumber = "EPSPL-705",
    CategoryId = 1012,
    LocationId = 7,
    AssetStatus = "InUse",
    Notes = "Used for presentations and training sessions",
    StatusDate = now,
    PurchaseDate = now.AddMonths(-13),
    PurchasePrice = 899.99m,
    Vendor = "Epson",
    Condition = "Good",
    WarrantyExpirationDate = now.AddDays(29)
}
                };

                context.Asset.AddRange(assets);
                await context.SaveChangesAsync();
            }
        }
    }
}
