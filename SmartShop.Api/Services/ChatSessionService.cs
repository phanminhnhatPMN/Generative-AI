using System.Collections.Concurrent;

namespace SmartShop.Api.Services
{
    public interface IChatSessionService
    {
        List<(string Role, string Text)> GetHistory(string sessionId);
        void AddMessage(string sessionId, string role, string text);
        void Clear(string sessionId);
    }

    public class ChatSessionService : IChatSessionService
    {
        private readonly ConcurrentDictionary<string, List<(string Role, string Text)>> _sessions = new();

        public List<(string Role, string Text)> GetHistory(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return new List<(string Role, string Text)>();

            return _sessions.GetOrAdd(sessionId, _ => new List<(string Role, string Text)>());
        }

        public void AddMessage(string sessionId, string role, string text)
        {
            if (string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(text))
                return;

            var history = _sessions.GetOrAdd(sessionId, _ => new List<(string Role, string Text)>());
            lock (history)
            {
                history.Add((role, text));
                // Keep max 20 turns to prevent context window explosion
                if (history.Count > 20)
                {
                    history.RemoveRange(0, history.Count - 20);
                }
            }
        }

        public void Clear(string sessionId)
        {
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                _sessions.TryRemove(sessionId, out _);
            }
        }
    }
}

