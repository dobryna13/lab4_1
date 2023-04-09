public class EventBus
{
    private Dictionary<string, List<Delegate>> _eventHandlers = new Dictionary<string, List<Delegate>>();
    private Dictionary<string, DateTime> _lastEventSentTimes = new Dictionary<string, DateTime>();
    private Dictionary<string, int> _eventThrottlingLimits = new Dictionary<string, int>();

    public void Register(string eventName, Delegate handler)
    {
        if (!_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers.Add(eventName, new List<Delegate>());
        }
        _eventHandlers[eventName].Add(handler);
    }

    public void Unregister(string eventName, Delegate handler)
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName].Remove(handler);
        }
    }

    public void SetThrottlingLimit(string eventName, int limit)
    {
        _eventThrottlingLimits[eventName] = limit;
    }

    public void Trigger(string eventName, object sender, EventArgs args)
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            if (_eventThrottlingLimits.ContainsKey(eventName))
            {
                DateTime lastSentTime;
                _lastEventSentTimes.TryGetValue(eventName, out lastSentTime);

                int limit;
                _eventThrottlingLimits.TryGetValue(eventName, out limit);

                if ((DateTime.Now - lastSentTime).TotalMilliseconds < limit)
                {
                    return;
                }
                _lastEventSentTimes[eventName] = DateTime.Now;
            }

            foreach (Delegate handler in _eventHandlers[eventName])
            {
                handler.DynamicInvoke(sender, args);
            }
        }
    }
}

