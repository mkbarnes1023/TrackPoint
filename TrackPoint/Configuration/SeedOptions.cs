using System.Collections.Generic;

namespace TrackPoint.Configuration
{
    public sealed class SeedOptions
    {
        public const string SectionName = "Seed";

        public List<string> AdminEmails { get; set; } = new();
    }
}