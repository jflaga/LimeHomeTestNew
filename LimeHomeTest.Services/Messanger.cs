using LimeHomeTest.Dto;
using LimeHomeTest.Dto.CrossCut;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LimeHomeTest.Services
{
    public class Messanger : IMessanger
    {
        private readonly IRestClient _restClient;
        private readonly ILogger<Messanger> _logger;

        private readonly string _baseUrl;
        private readonly string _apiKey;

        public Messanger(IRestClient restClient, ILogger<Messanger> logger, IOptions<HereApi> hereApi)
        {
            Verify(restClient, logger, hereApi);

            _restClient = restClient;
            _apiKey = hereApi.Value.ApiKey;
            _baseUrl = hereApi.Value.RequestUrl;
        }


        public async Task<Result> GetHotelsByLocation(string at)
        {
            if (string.IsNullOrWhiteSpace(at))
            {
                throw new ArgumentNullException("Coordinates are null");
            }

            _restClient.BaseUrl = new Uri(CreateUrlString(at));

            var request = new RestRequest(Method.GET);
            IRestResponse response = await _restClient.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<Result>(response.Content);
        }

        private void Verify(IRestClient restClient, ILogger<Messanger> logger, IOptions<HereApi> hereApi)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("Logger is null");
            }

            if (hereApi == null || string.IsNullOrWhiteSpace(hereApi.Value.ApiKey)
                || string.IsNullOrWhiteSpace(hereApi.Value.RequestUrl))
            {
                _logger.LogError("Config is null");
                throw new ArgumentNullException("Config is null");
            }

            if (restClient == null)
            {
                _logger.LogError("Rest client is null");
                throw new ArgumentNullException("Rest client is null");
            }
        }

        private string CreateUrlString(string at)
        {
            var requestString = new StringBuilder(_baseUrl);

            requestString.Append("?at=");
            requestString.Append(at);
            requestString.Append("&q=hotel&apiKey=");
            requestString.Append(_apiKey);

            return requestString.ToString();
        }
    }
}