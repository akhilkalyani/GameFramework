using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace GF
{
    public class GameServiceController
    {
        private List<IService> services = new List<IService>();    
        public void Initialize()
        {
            Console.Log(LogType.Log, "GameServiceController created");
            foreach (var type in GetAllTypesThatImplementInterface<IService>())
            {
                services.Add((IService)Activator.CreateInstance(type));
            }
        }

        private IEnumerable<Type> GetAllTypesThatImplementInterface<T>()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type =>type.Namespace=="GF" && typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }
        public void RegisterListener()
        {
            foreach (var service in services)
            {
                service.Initialize();
                service.RegisterListener();
            }
        }
        public void AddService(IService service)
        {
            service.Initialize();
            service.RegisterListener();
            services.Add(service);
        }
        public void RemoveService(IService service)
        {
            int targetIndex = services.IndexOf(service);
            var targetService =services[targetIndex];
            services.RemoveAt(targetIndex);
            ((Service)targetService).Dispose();   
        }
        public void RemoveListener()
        {
            foreach (var service in services)
            {
                service.RemoveListener();
            }
        }
    }
}