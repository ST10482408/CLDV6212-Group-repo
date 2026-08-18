using Azure;
using Azure.Data.Tables;
using CoffeeAndChill.Interface;



namespace CoffeeAndChill.Models
{
    public class MenuItems : ITableEntity
    {
        //Azure table storage Partition key
        public string PartitionKey { get; set; } = string.Empty;

        //Azure table storage Row key
        public string RowKey { get; set; } = string.Empty;

        // e.g. "Hot Drinks", "Cold Drinks", "Food"
        public string Category { get; set; } = string.Empty;

        //Menu Item name
        public string Name { get; set; } = string.Empty;

        //Description of the menu item
        public string Description { get; set; } = string.Empty;

        //selling price
        public Decimal Price { get; set; }

        // For ordering items within a category on the menu
        public int DisplayOrder { get; set; }

        //Indicates wether item is available or not
        public bool IsAvailable { get; set; }

        //Automatically maintained by Azure table storage
        public DateTimeOffset? Timestamp { get; set; }

        //Entity tag used for concurrency
        public ETag ETag { get; set; }

         
    }
}

