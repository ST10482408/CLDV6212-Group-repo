using CoffeeAndChill.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeAndChill.Interface
{
    internal class ItableStorageService
    {
    
        public interface ITableStorageService
        {
            Task<MenuItems> CreateMenuItemAsync(CreateMenuItemRequest request);

            Task<List<MenuItems>> GetAllMenuItemsAsync();

            Task<List<MenuItems>> GetMenuItemsByCatergoryAsync(string catergory);

            Task<MenuItems> GetMenuItemsAsync(string catergory, string sku);

            Task<MenuItems> UpdateMenuItemAsync(
                string catergory,
                string sku,
                UpdateMenuItemRequest request

                );
            Task<bool> DeleteMenuItemAsync(
                string catergory,
                string sku
                );




        }
    }

}

