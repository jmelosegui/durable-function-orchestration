﻿using Azure.Core;
using DurableFunctionOrchestration.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace DurableFunctionOrchestration.Activities
{
    internal class HotelFunctions
    {
        private static HttpClient _httpClient = null!;

        public HotelFunctions(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ReservationClient");
        }

        [Function(nameof(RegistrationAsync))]
        public async Task<bool> RegistrationAsync([ActivityTrigger] string userId, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(RegistrationAsync));
            logger.LogInformation("Creating hotel registration for user {userId}.", userId);

            var request = GetReservationRequest();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            throw new HotelFunctionException("Bad mojo");

            var response = await _httpClient.PostAsync("/api/reservation/hotel", content);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to create hotel registration for user {userId}.", userId);
                return false;
            }

            logger.LogInformation("Hotel registration created for user {userId}.", userId);

            return true;
        }

        private HotelReservationRequest GetReservationRequest()
        {
            // Create a ramdom HotelReservationRequest
            var random = new Random();
            var reservationRequest = new HotelReservationRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "John Doe",
                CheckIn = DateTime.Now.AddDays(random.Next(1, 30)),
                CheckOut = DateTime.Now.AddDays(random.Next(31, 60)),
                Address = "123 Main St, Redmond, WA 98052",

            };
            return reservationRequest;
        }
    }
}
