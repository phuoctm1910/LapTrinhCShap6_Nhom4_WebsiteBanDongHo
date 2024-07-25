using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Web_DongHo_WebAssembly.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web_DongHo_WebAssembly.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private HttpClient Http { get; set; }

        private HomeProductRequest homeProductRequest = new HomeProductRequest();

        protected override async Task OnInitializedAsync()
        {
            await GetProduct();
        }
        public async Task GetProduct()
        {
            try
            {
                var response = await Http.GetAsync("https://localhost:44355/api/Home/homeProducts");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                homeProductRequest = JsonConvert.DeserializeObject<HomeProductRequest>(jsonResponse);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"JSON error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
    public class HomeProductRequest
    {
        public List<Product> Productfirst8 { get; set; }
        public List<Product> Productsecond8 { get; set; }
        public List<Product> Productthird8 { get; set; }
    }
}
