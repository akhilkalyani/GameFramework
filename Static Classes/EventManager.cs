using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GF
{
    /// <summary>
    /// Manages All Events in the game.
    /// </summary>
    public class EventManager
    {
        private static EventManager _instance = null;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                }
                return _instance;
            }
        }
        public Action OnSynchronizeEventComplete;
        private readonly Dictionary<Type, EventListener> eventListeners = new Dictionary<Type, EventListener>();
        private readonly Queue<GameEvent> queueEvents = new Queue<GameEvent>();
        /// <summary>
        /// Add particular Evnet to event dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        public void AddListener<T>(EventListener.EventHandler<T> eventHandler) where T : GameEvent
        {
            if (!eventListeners.TryGetValue(typeof(T), out EventListener invoker))
            {
                invoker = new EventListener();
                eventListeners.Add(typeof(T), invoker);
            }
            invoker.eventHandler += (e) => eventHandler((T)e);
        }

        public void Start()
        {
            OnSynchronizeEventComplete += CompletedQueueEvent;   
        }
        public void ExecuteQueueEvent()
        {
            if(queueEvents.Count>0)
                TriggerEvent(queueEvents.Dequeue());
        }
        private void CompletedQueueEvent()
        {
            ExecuteQueueEvent();
        }
        public void QueueEvent(GameEvent ev)
        {
            queueEvents.Enqueue(ev);
        }
        /// <summary>
        /// Remove particular Evnet from event dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        public void RemoveListener<T>(EventListener.EventHandler<T> eventHandler) where T : GameEvent
        {
            if (eventListeners.TryGetValue(typeof(T), out EventListener invoker))
            {
                invoker.eventHandler -= (e) => eventHandler((T)e);
                eventListeners.Remove(typeof(T));
            }
        }
        /// <summary>
        /// Check Evnet in Event dictionry.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool HasListener(Type eventType)
        {
            return eventListeners.ContainsKey(eventType);
        }

        /// <summary>
        /// Triger particular event.
        /// </summary>
        /// <param name="evt"></param>
        public void TriggerEvent(GameEvent evt)
        {
            if (eventListeners.TryGetValue(evt.GetType(), out EventListener invoker))
            {
                invoker.Invoke(evt);
            }
        }
       
        /// <summary>
        /// Release all events.
        /// </summary>
        public void ReleaseEvents()
        {
            foreach (EventListener value in eventListeners.Values)
            {
                value.Clear();
            }
            eventListeners.Clear();
            OnSynchronizeEventComplete -= CompletedQueueEvent;
        }
    }
    /// <summary>
    /// Hold Evnet in it
    /// </summary>
    public class EventListener
    {
        public delegate void EventHandler(GameEvent ge);
        public delegate void EventHandler<T>(T e) where T : GameEvent;

        public EventHandler eventHandler;

        public void Invoke(GameEvent ge)
        {
            eventHandler?.Invoke(ge);
        }

        public void Clear()
        {
            eventHandler = null;
        }
    }
    /// <summary>
    /// Super class for event classes.
    /// Any class can inherit and become class event driver.
    /// </summary>
    public class GameEvent
    {

    }
}
