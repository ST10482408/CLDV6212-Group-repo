
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using CoffeeAndChill.Interface;



namespace Coffe_Chill.Functions
{
    public class CreateMenuItemsFunction
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<CreateMenuItemsFunction> _logger;

        public CreateMenuItemsFunction(
            ITableStorageService tableStorageService,
            ILogger<CreateMenuItemsFunction> logger)
        {
            _tableStorageService = tableStorageService;
            _logger = logger;
        }

        [Function("CreateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "post",
                Route = "menuitems")]
            HttpRequestData req)
        {
            try
            {
                 
                var request = await JsonSerializer.DeserializeAsync<CreateMenuItemRequest>(
                    req.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                
                if (request == null)
                {
                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    await bad.WriteAsJsonAsync(new { error = "Request body is missing or malformed." });
                    return bad;
                }

                // Input validation 

                var validationErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Name))
                    validationErrors.Add("Name is required.");

                if (string.IsNullOrWhiteSpace(request.Catergory))
                    validationErrors.Add("Category is required.");

                if (string.IsNullOrWhiteSpace(request.SKU))
                    validationErrors.Add("SKU is required.");

                if (request.Price <= 0)
                    validationErrors.Add("Price must be greater than zero.");

                if (validationErrors.Any())
                {
                    _logger.LogWarning("CreateMenuItem validation failed: {Errors}",
                        string.Join(", ", validationErrors));

                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    await bad.WriteAsJsonAsync(new { errors = validationErrors });
                    return bad;
                }

                //Map DTO to entity

                _logger.LogInformation(
                    "Creating menu item: {Name} | Category: {Category} | SKU: {SKU} | Price: {Price:C}",
                    request.Name, request.Catergory, request.SKU, request.Price);

                var menuItem = await _tableStorageService.CreateMenuItemAsync(request);
 
                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(menuItem);
                return response;
            }
            catch (InvalidOperationException ex)
            {
                
                _logger.LogWarning(ex, "Conflict creating menu item.");
                var conflict = req.CreateResponse(HttpStatusCode.Conflict);
                await conflict.WriteAsJsonAsync(new { error = ex.Message });
                return conflict;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating menu item.");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred. Please try again later."
                });
                return error;
            }
        }
    }
}