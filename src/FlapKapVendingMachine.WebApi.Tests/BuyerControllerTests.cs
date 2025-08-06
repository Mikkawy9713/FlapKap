using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

namespace FlapKapVendingMachine.WebApi.Tests
{
    public class BuyerControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public record PurchaseResponse(
    int totalSpent,
    string productPurchased,
    int quantity,
    List<int> change
);

        public BuyerControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> AuthenticateBuyerAsync()
        {
            var response = await _client.PostAsync("/api/auth/login?username=buyer11&password=123", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            json.Should().ContainKey("token");

            return json["token"];
        }

        [Fact]
        public async Task Deposit_WithValidCoin_ShouldIncreaseDeposit()
        {
            var token = await AuthenticateBuyerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var depositAmount = 50; // valid coin
            var response = await _client.PostAsync($"/api/buyer/deposit?coin={depositAmount}", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            json.Should().ContainKey("deposit");
            ((JsonElement)json["deposit"]).GetInt32().Should().Be(50);

        }

        [Fact]
        public async Task Deposit_WithInvalidCoin_ShouldReturnBadRequest()
        {
            var token = await AuthenticateBuyerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var invalidCoin = 7; // not allowed
            var response = await _client.PostAsync($"/api/buyer/deposit?coin={invalidCoin}", null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        
        public async Task Buy_WithEnoughDeposit_ShouldPurchaseProductAndReturnChange()
        {
            var token = await AuthenticateBuyerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Step 1: Deposit enough money
            await _client.PostAsync("/api/buyer/deposit?coin=100", null); // deposit 100
            await _client.PostAsync("/api/buyer/deposit?coin=100", null); // deposit 200 total

            // Step 2: Buy product (Coke = 50 per unit)
            var response = await _client.PostAsync("/api/buyer/buy?productId=1&quantity=2", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize directly into a strongly typed record
            var result = await response.Content.ReadFromJsonAsync<PurchaseResponse>();

            result.Should().NotBeNull();
            result!.totalSpent.Should().Be(100);
            result.productPurchased.Should().Be("Coke");
            result.change.Should().NotBeNull();
            result.change.Should().AllBeOfType<int>();
        }

        [Fact]
        public async Task Buy_WithInsufficientFunds_ShouldReturnBadRequest()
        {
            var token = await AuthenticateBuyerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Only deposit 20
            await _client.PostAsync("/api/buyer/deposit?coin=20", null);

            var response = await _client.PostAsync("/api/buyer/buy?productId=1&quantity=1", null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Reset_ShouldSetDepositToZero()
        {
            var token = await AuthenticateBuyerAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Deposit some money
            await _client.PostAsync("/api/buyer/deposit?coin=50", null);

            // Reset deposit
            var resetResponse = await _client.PostAsync("/api/buyer/reset", null);
            resetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await resetResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            json.Should().ContainKey("deposit");
            ((JsonElement)json["deposit"]).GetInt32().Should().Be(0);
        }
    }

}
