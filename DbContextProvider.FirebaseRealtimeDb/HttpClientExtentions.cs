using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public static class HttpClientExtentions
    {
        public static async Task<IEnumerable<T>> SendAsync<T>(this HttpClient httpClient, HttpRequestMessage request, bool isList = false)
        {
            var resp = await httpClient.SendAsync(request);
            resp?.EnsureSuccessStatusCode();
            var body = await resp?.Content.ReadAsStringAsync();
            var raw = JToken.Parse(body);
            var list = new List<T>();
            if (String.IsNullOrWhiteSpace(body) || !raw.HasValues)
            {
                return list;
            }

            if (!isList)
            {
                list.Add(JsonConvert.DeserializeObject<T>(body));
            }
            else
            {
                foreach (var item in (raw as JObject))
                {
                    list.Add(item.Value.ToObject<T>());
                }
            };

            return list;
        }
    }
}
