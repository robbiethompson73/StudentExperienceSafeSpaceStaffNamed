using SharedViewModelLibrary.Models;

namespace MvcUi.Services.ApiClients
{
    public class StudentExperienceFormApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public StudentExperienceFormApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Retrieves a list of active student experience forms from the Web API
        public async Task<List<StudentExperienceFormViewModel>> GetActiveFormsAsync()
        {
            // var fullUri = new Uri(_httpClient.BaseAddress!, "api/forms/active");
            //var baseUrlTemp = _configuration["ApiUrls:StudentExperienceForms"];

            //System.Diagnostics.Debug.WriteLine($"Base URL from config: {baseUrlTemp}");
            //System.Diagnostics.Debug.WriteLine($"Calling API: {fullUri}");

            var response = await _httpClient.GetFromJsonAsync<List<StudentExperienceFormViewModel>>("api/forms/active");

            return response ?? new List<StudentExperienceFormViewModel>();
        }

    }
}
