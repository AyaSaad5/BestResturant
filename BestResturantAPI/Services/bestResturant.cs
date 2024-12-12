using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BestResturantAPI.Services
{
    public class Result
    {
        /// <summary>
        /// this code solve a problem where you need to find the best resturant 
        /// in a given city, based on criteria max cost and average user rating.
        /// 
        /// </summary>
        
        public async Task<string> bestRestura(string city, int cost)
        {
           string baseUrl = $"https://jsonmock.hackerrank.com/api/food_outlets?city={city}";

            List<foodOutlet> bestResturant = new List<foodOutlet>();
            //to make http request 
            using(var httpClient = new HttpClient())
            {
                int currentPage = 1, tottalPage = 0;
                do
                {
                    string url = $"{baseUrl}&page={currentPage}";

                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Failed to fetch data from the API");
                    
                    var apiRresponse = JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync());
                    if(apiRresponse != null)
                    {
                        tottalPage = apiRresponse.total_pages;
                        bestResturant.AddRange(apiRresponse.data.Where(r => r.estimated_cost <= cost));
                    }

                    currentPage++;
                } while (currentPage <= tottalPage);

                var best = bestResturant
                           .OrderByDescending(r => r.user_rating?.average_rating)
                           .ThenBy(r => r.estimated_cost)
                           .FirstOrDefault();

                return best?.name ?? "No restaurant found";
            }

        }
    }

    internal class ApiResponse
    {
        public int total_pages { get; set; }
        public List<foodOutlet> data { get; set; } = new List<foodOutlet>();
    }

    internal class foodOutlet
    {
        public string? name { get; set; }
        public int estimated_cost { get; set; }
        public userRating? user_rating { get; set; }
    }

    public class userRating
    {
        public double? average_rating { get; set; }
    }
}
