using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace GF
{
    public class GameServiceController
    {
        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();   
        public void Initialize()
        {
            Console.Log(LogType.Log, "GameServiceController created");
            foreach (var type in GetAllTypesThatImplementInterface<IService>())
            {
                services.Add(type,(IService)Activator.CreateInstance(type));
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
            foreach (var service in services.Values)
            {
                service.Initialize();
                service.RegisterListener();
            }
        }
        public void AddService(Type type)
        {
            var service = (IService)Activator.CreateInstance(type);
            service.Initialize();
            service.RegisterListener();
            services.Add(type, service);
        }
        public void RemoveService(Type type)
        {
            if(services.TryGetValue(type,out IService service))
            {
                services.Remove(type);
                ((Service)service).Dispose();
            }
        }
        public void RemoveListener()
        {
            foreach (var service in services.Values)
            {
                service.RemoveListener();
            }
        }
    }
}