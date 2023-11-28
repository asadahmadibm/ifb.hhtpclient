using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;

namespace ifb.httpclient
{
    public class BaseService : IHttpService
    {
        #region Constructors...
        public BaseService(
            System.Net.Http.HttpClient http) : base()
        {
            HttpClient = http;

            JsonOptions =
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

            BaseUrl = http.BaseAddress.AbsoluteUri;
            BaseUrl = BaseUrl.Remove(BaseUrl.Length - 1, 1);

            RequestUri = $"{BaseUrl}/";
        }

        #endregion

        #region Fields and Properties...

        protected System.Net.Http.HttpClient HttpClient { get; }
        protected string BaseUrl { get; set; }
        public string RequestUri { get; set; }
        protected JsonSerializerOptions JsonOptions { get; set; }

        public string AcceptMethod { get; set; } = "application/json";

        private Dictionary<string, string> _headers = null;
        public Dictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new Dictionary<string, string>();
                }

                return _headers;
            }
        }

        #endregion

        #region Methods...

        private async Task<O> SendRequest<O>(HttpRequestMessage request)
        {
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptMethod));

                if (Headers.Count > 0)
                {
                    foreach (var header in Headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                        //request.Headers.Add("Authorization", $"Bearer {header.Value}");
                    }
                }

                var response =
                    await HttpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
                {
                    throw (new System.Exception("Error Code 203. Sent Data is invalid"));
                }

                // auto logout on 401 response
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw (new System.Exception("Error Code 401. Authorization denied."));
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw (new System.Exception("Error Code 404.Requested resource does not exist."));
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (result != null && result != String.Empty)
                    {
                        throw new Exception(result);
                    }
                    else
                    {
                        throw (new System.Exception("Error Code 400.Bad Request."));
                    }
                }

                // throw exception on error response
                if (response.IsSuccessStatusCode == false)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    throw new Exception(result);
                }

                try
                {
                    var responseResult =
                        await response.Content.ReadAsAsync<O>();//.ReadFromJsonAsync<O>();

                    return responseResult;
                }
                catch
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (result == null || result.Length == 0 || result.ToString() == "[]")
                    {
                        throw (new System.Exception("Error Code 204.No Content."));
                    }

                    var resultObj = JsonConvert.DeserializeObject<O>(result);

                    return resultObj;
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                System.Console.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Web Api Methods...

        public async Task<O> GetAsync<O>()
        {
            return await GetAsync<O>("");
        }
        public async Task<O> GetByIdAsync<O>(string id)
        {
            return await GetAsync<O>(id);
        }

        public async Task<O> GetAsync<O>(string methodName, params string[] args)
        {
            string currentRequest = $"{RequestUri}/{methodName}";

            if (args.Length > 0)
            {
                if (args != null)
                    currentRequest = $"{currentRequest}/{args[0]}";
            }

            for (int i = 1; i < args.Length; i++)
            {
                if (args != null)
                    currentRequest = $"{currentRequest}/{args[i]}";
            }

            var request =
                new HttpRequestMessage(HttpMethod.Get, currentRequest);

            return await SendRequest<O>(request);
        }

        public async Task<O> PostAsync<I, O>(I viewModel)
        {
            return await PostAsync<I, O>("", viewModel);
        }

        public async Task<O> PostAsync<I, O>(string methodName, I viewModel)
        {
            var request =
                new HttpRequestMessage(HttpMethod.Post, $"{RequestUri}/{methodName}");

            request.Content =
                new StringContent(System.Text.Json.JsonSerializer.Serialize(viewModel), Encoding.UTF8, AcceptMethod);

            return await SendRequest<O>(request);
        }

        public async Task<O> PutAsync<I, O>(I viewModel)
        {
            return await PutAsync<I, O>("", viewModel);
        }

        public async Task<O> PutAsync<I, O>(string methodName, I viewModel)
        {
            var request =
                new HttpRequestMessage(HttpMethod.Put, $"{RequestUri}/{methodName}");

            request.Content =
                new StringContent(System.Text.Json.JsonSerializer.Serialize(viewModel), Encoding.UTF8, AcceptMethod);

            return await SendRequest<O>(request);
        }

        public virtual async System.Threading.Tasks.Task<bool> DeleteAsync(string methodName, string id)
        {
            var requestUri =
                $"{RequestUri}";

            if (methodName != null && methodName.Trim() != string.Empty)
            {
                requestUri += $"/{methodName}/{id}";
            }
            else
            {
                requestUri += $"/{id}";
            }

            var request =
                new HttpRequestMessage(HttpMethod.Delete, $"{RequestUri}/{methodName}/{id}");

            return await SendRequest<bool>(request);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await DeleteAsync("", id);
        }

        #endregion
    }
}
