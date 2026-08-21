using Azure;
using Azure.Data.Tables;
using CoffeeAndChill.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 

namespace CoffeeNChill.Functions.Services
{
    public class TableStorageService : ITableStorageService
    {

        // Add Private Field
        private readonly TableClient _tableClient;

        // Adding Constructor
        public TableStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureWebJobsStorage"]
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage connection string is missing.");

            _tableClient = new TableClient(connectionString, "MenuItems");

            _tableClient.CreateIfNotExists();
        }

        public async Task<MenuItems> CreateMenuItemAsync(CreateMenuItemRequest request)
        {
            // Create a new MenuItem entity
            MenuItems menuItem = new MenuItems
            {
                PartitionKey = request.Catergory,
                RowKey = request.SKU,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
                
                
                
                
                
                ,
                IsAvailable = request.IsAvailable
            };

            // Save to Azure Table Storage
            await _tableClient.AddEntityAsync(menuItem);

            return menuItem;
        }


        public async Task<bool> DeleteMenuItemAsync(string category, string sku)
        {
            try
            {
                // Retrieve the entity to obtain its ETag
                Response<MenuItems> response =
                    await _tableClient.GetEntityAsync<MenuItems>(category, sku);

                // Delete the entity
                await _tableClient.DeleteEntityAsync(
                    category,
                    sku,
                    response.Value.ETag);

                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return false;
            }
        }


        public async Task<List<MenuItems>> GetAllMenuItemsAsync()
        {
            List<MenuItems> menuItems = new List<MenuItems>();

            await foreach (MenuItems item in _tableClient.QueryAsync<MenuItems>())
            {
                menuItems.Add(item);
            }

            return menuItems;
        }


        public async Task<MenuItems?> GetMenuItemsAsync(string category, string sku)
        {
            try
            {
                Response<MenuItems> response =
                    await _tableClient.GetEntityAsync<MenuItems>(category, sku);

                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task<List<MenuItems>> GetMenuItemsByCatergoryAsync(string category)
        {
            List<MenuItems> menuItems = new List<MenuItems>();

            string filter = $"PartitionKey eq '{category}'";

            await foreach (MenuItems item in _tableClient.QueryAsync<MenuItems>(filter))
            {
                menuItems.Add(item);
            }

            return menuItems;
        }


        public async Task<MenuItems> UpdateMenuItemAsync(
            string category,
            string sku,
            UpdateMenuItemRequest request)
        {
            try
            {
                // Retrieve the existing entity
                Response<MenuItems> response =
                    await _tableClient.GetEntityAsync<MenuItems>(category, sku);

                MenuItems menuItem = response.Value;

                // Update the entity
                menuItem.Name = request.Name;
                menuItem.Description = request.Description;
                menuItem.Price = request.Price;
                menuItem.IsAvailable = request.IsAvailable;

                // Save the updated entity
                await _tableClient.UpdateEntityAsync(
                    menuItem,
                    menuItem.ETag,
                    TableUpdateMode.Replace);

                return menuItem;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }
    }
}
