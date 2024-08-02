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
using static System.Net.WebRequestMethods;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Web_DongHo_WebAssembly.Models;
using static Web_DongHo_WebAssembly.Pages.Product.Product;

namespace Web_DongHo_WebAssembly.Pages.Home
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private HttpClient Http { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private AuthState Auth { get; set; }

        private HomeProductRequest homeProductRequest = new HomeProductRequest();

        protected override async Task OnInitializedAsync()
        {
            await GetProduct();
        }
        public async Task GetProduct()
        {
            try
            {
                var response = await Http.GetAsync("api/Home/homeProducts");
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
        public async Task AddProductToCart(int productId, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                bool isConfirmed = await JS.InvokeAsync<bool>("confirm", "Bạn chưa đăng nhập. Bạn có muốn đăng nhập không?");

                if (isConfirmed)
                {
                    Navigation.NavigateTo("/login");
                }
            }
            else
            {
                try
                {
                    var response = await Http.PostAsJsonAsync("api/cart/addToCart", new AddToCartRequest { ProductId = productId, Username = username });
                    var responseContent = await response.Content.ReadFromJsonAsync<ApiResponse>();

                    if (responseContent != null)
                    {
                        if (!responseContent.Success)
                        {
                            await JS.InvokeVoidAsync("alert", responseContent.Message);
                        }
                        else
                        {
                            await JS.InvokeVoidAsync("alert", "Sản phẩm đã được thêm vào giỏ hàng.");
                        }
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("alert", "Phản hồi không hợp lệ từ máy chủ.");
                    }
                }
                catch (Exception ex)
                {
                    await JS.InvokeVoidAsync("console.log", $"Error adding product to cart: {ex.Message}");
                }
            }
        }

    }
    public class AddToCartRequest
    {
        public string Username { get; set; }
        public int ProductId { get; set; }
    }
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class HomeProductRequest
    {
        public List<Web_DongHo_WebAssembly.Data.Product> Productfirst8 { get; set; }
        public List<Web_DongHo_WebAssembly.Data.Product> Productsecond8 { get; set; }
        public List<Web_DongHo_WebAssembly.Data.Product> Productthird8 { get; set; }
    }
}
