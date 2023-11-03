using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }
        public void RegisterListener()
        {
            foreach (var service in services)
            {
                service.Initialize();
                service.RegisterListener();
            }
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