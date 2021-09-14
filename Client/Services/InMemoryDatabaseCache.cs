using Client.Static;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    internal sealed class InMemoryDatabaseCache
    {
        private readonly HttpClient _httpClient;

        public InMemoryDatabaseCache(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private List<Category> _categories = null;
        internal List<Category> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                NotifyCategoriesDataChanged();
            }
        }

        internal async Task<Category> GetCategoryByCategoryId(int categoryId)
        {
            if (_categories == null)
            {
                await GetCategoriesFromDatabaseAndCache();
            }

            return _categories.First(category => category.CategoryId == categoryId);
        }

        private bool _gettingCategoriesFromDatabaseAndCaching = false;
        internal async Task GetCategoriesFromDatabaseAndCache()
        {
            // Only allow one Get request to run at a time.
            if (_gettingCategoriesFromDatabaseAndCaching == false)
            {
                _gettingCategoriesFromDatabaseAndCaching = true;
                _categories = await _httpClient.GetFromJsonAsync<List<Category>>(APIEndpoints.s_categories);
                _gettingCategoriesFromDatabaseAndCaching = false;
            }            
        }

        internal event Action OnCategoriesDataChanged;
        private void NotifyCategoriesDataChanged() => OnCategoriesDataChanged?.Invoke();
    }
}
