using Microsoft.NET.StringTools;

namespace RouteManager.Models
{
    public class Location
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public ICollection<string> References { get; set; }

        public Location()
        {
            Id = Guid.NewGuid();
            References = new List<string>();
        }

        public override string ToString()
        {
            SpanBasedStringBuilder sb = new SpanBasedStringBuilder();
            
            if (!string.IsNullOrWhiteSpace(Name))
            {
                sb.Append(Name);
                sb.Append(": ");
            }

            if (!string.IsNullOrWhiteSpace(Address))
            {
                sb.Append(Address);
            }
            else
            {
                sb.Append($"{Math.Round(Latitude, 4)}, {Math.Round(Latitude, 4)}");
            }

            return sb.ToString();
        }
    }
}
