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
            EventManager.Instance.AddListener<AddServiceEvent>(AddService);
            EventManager.Instance.AddListener<RemoveServiceEvent>(RemoveService);
        }
        public void AddService(AddServiceEvent serviceEvent)
        {
            var service = (IService)Activator.CreateInstance(serviceEvent.ServiceType);
            service.Initialize();
            service.RegisterListener();
            services.Add(serviceEvent.ServiceType, service);
        }
        public void RemoveService(RemoveServiceEvent serviceEvent)
        {
            if(services.TryGetValue(serviceEvent.ServiceType,out IService service))
            {
                services.Remove(serviceEvent.ServiceType);
                ((Service)service).Dispose();
            }
        }
        public void RemoveListener()
        {
            foreach (var service in services.Values)
            {
                service.RemoveListener();
            }
            EventManager.Instance.AddListener<AddServiceEvent>(AddService);
            EventManager.Instance.AddListener<RemoveServiceEvent>(RemoveService);
        }
    }
}