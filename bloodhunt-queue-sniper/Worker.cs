using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace bloodhunt_queue_sniper
{
    public class Worker
    {
        public static string WorkersUrl = "https://queue.bloodhunt.community/";

        public async Task<string> DeletePartyAsync(string key)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(WorkersUrl + "delete?key=" + key);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return "null";
                    }
                }
            }
            catch (Exception ex)
            {
                return "null";
            }
        }

        public async Task<string> QueuePartyAsync(string key, string mode)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(WorkersUrl + "queue?key=" + key + "&mode=" + mode);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return "null";
                    }
                }
            }
            catch (Exception ex)
            {
                return "null";
            }
        }

        public async Task<string> CreatePartyKeyAsync(string leader)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(WorkersUrl + "generate?leader=" + leader);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return "null";
                    }
                }
            }
            catch (Exception ex)
            {
                return "null";
            }
        }

        public async Task<string> CheckPartyKeyAsync(string key)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(WorkersUrl + "status?key=" + key);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return "null";
                    }
                }
            }
            catch (Exception ex)
            {
                return "null";
            }
        }
    }
}
