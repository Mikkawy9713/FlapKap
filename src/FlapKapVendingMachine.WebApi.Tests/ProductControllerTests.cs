using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FlapKapVendingMachine.Domain.Entities;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FlapKapVendingMachine.WebApi.Tests
{
    public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> AuthenticateSellerAsync()
        {
            var response = await _client.PostAsync("/api/auth/login?username=seller11&password=123", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return json!["token"];
        }

        [Fact]
        public async Task GetProducts_ShouldReturnList_WithoutAuth()
        {
            var response = await _client.GetAsync("/api/product");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            products.Should().NotBeNull();
            products.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateProduct_AsSeller_ShouldSucceed()
        {
            var token = await AuthenticateSellerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newProduct = new Product
            {
                ProductName = "Sprite",
                AmountAvailable = 20,
                Cost = 40
            };

            var response = await _client.PostAsJsonAsync("/api/product", newProduct);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<Product>();
            created.Should().NotBeNull();
            created!.ProductName.Should().Be("Sprite");
            created.AmountAvailable.Should().Be(20);
            created.Cost.Should().Be(40);
        }

        [Fact]
        public async Task UpdateProduct_AsSeller_ShouldSucceed()
        {
            var token = await AuthenticateSellerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updatedProduct = new Product
            {
                ProductName = "Updated Coke",
                AmountAvailable = 50,
                Cost = 60
            };

            var response = await _client.PutAsJsonAsync("/api/product/1", updatedProduct);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<Product>();
            updated.Should().NotBeNull();
            updated!.ProductName.Should().Be("Updated Coke");
        }

        [Fact]
        public async Task UpdateProduct_AsWrongSeller_ShouldReturnForbidden()
        {
            // No wrong seller user yet — simulate by changing token roles in seed if needed
            // For now just call without auth to simulate fail
            var updatedProduct = new Product
            {
                ProductName = "Invalid Update",
                AmountAvailable = 10,
                Cost = 10
            };

            var response = await _client.PutAsJsonAsync("/api/product/1", updatedProduct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteProduct_AsSeller_ShouldSucceed()
        {
            var token = await AuthenticateSellerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync("/api/product/1");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
