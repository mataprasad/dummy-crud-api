using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseCloudImageStorage
    {
        private HttpClient httpClient;
        private FirebaseAuthOption firebaseAuthOption;

        public FirebaseCloudImageStorage(IHttpClientFactory httpClientFactory, FirebaseAuthOption firebaseAuthOption)
        {
            this.httpClient = httpClientFactory.CreateClient(DI.FIREBASE_HTTP_CLIENT);
            this.firebaseAuthOption = firebaseAuthOption;
        }

        public string Name => nameof(FirebaseCloudImageStorage);

        public bool Delete(string fileName)
        {
            try
            {
                var url = GetDownloadUrl(fileName);
                var http = httpClient;
                {
                    var result = http.DeleteAsync(url).Result;
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                    return (result.StatusCode == System.Net.HttpStatusCode.NoContent);
                }
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public Dictionary<string, object> Get(string fileName)
        {
            try
            {
                var url = GetDownloadUrl(fileName);
                var http = httpClient;
                {
                    var result = http.GetAsync(url).Result;
                    result.EnsureSuccessStatusCode();
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultContent);
                    return data;
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public Dictionary<string, object> Insert(string fileName, Stream imageStream)
        {
            try
            {
                var downloadUrl = GetFullDownloadUrl(fileName);
                var http = httpClient;
                {
                    var url = GetTargetUrl(fileName);
                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StreamContent(imageStream)
                    };
                    var extension = Path.GetExtension(fileName);
                    var mimeType = extension.ToLower() switch
                    {
                        ".jpg" => "image/jpeg",
                        ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        _ => null
                    };

                    if (!string.IsNullOrEmpty(mimeType))
                    {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                    }

                    var response = http.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    var resultContent = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultContent);
                    //return downloadUrl + data["downloadTokens"];
                    return data;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        private string GetTargetUrl(string fileName)
        {
            return $"{firebaseAuthOption.FirebaseStorageEndpoint}/{firebaseAuthOption.Bucket}/o?name={this.GetEscapedPath(fileName)}";
        }

        private string GetDownloadUrl(string fileName)
        {
            return $"{firebaseAuthOption.FirebaseStorageEndpoint}/{firebaseAuthOption.Bucket}/o/{this.GetEscapedPath(fileName)}";
        }

        private string GetFullDownloadUrl(string fileName)
        {
            return this.GetDownloadUrl(fileName) + "?alt=media&token=";
        }

        private string GetEscapedPath(string fileName)
        {
            return Uri.EscapeDataString(fileName);
        }
    }
}
