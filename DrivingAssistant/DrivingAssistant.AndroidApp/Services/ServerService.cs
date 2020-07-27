using System;
using System.Collections.Generic;
using System.Linq;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class ServerService
    {
        //============================================================
        public IEnumerable<HostServer> GetAll()
        {
            try
            {
                var servers = JsonConvert.DeserializeObject<IEnumerable<HostServer>>(CacheManager.Get("servers"));
                if (!servers.Any())
                {
                    CacheManager.Set("servers", JsonConvert.SerializeObject(new[] { HostServer.Default }, Formatting.Indented));
                    return new[] { HostServer.Default };
                }

                return servers;
            }
            catch (Exception)
            {
                CacheManager.Set("servers", JsonConvert.SerializeObject(new [] {HostServer.Default}, Formatting.Indented));
                return new[] { HostServer.Default };
            }
        }

        //============================================================
        public void Set(HostServer server)
        {
            var servers = GetAll().ToList();
            servers.Add(server);
            CacheManager.Set("servers", JsonConvert.SerializeObject(servers, Formatting.Indented));
        }

        //============================================================
        public void Delete(string name)
        {
            var servers = GetAll().ToList();
            servers.Remove(servers.First(x => x.Name == name));
            CacheManager.Set("servers", JsonConvert.SerializeObject(servers, Formatting.Indented));
        }
    }
}