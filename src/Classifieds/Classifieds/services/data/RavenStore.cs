using System.Collections.Concurrent;
using Raven.Client.Document;

namespace Classifieds.services.data
{
    public static class RavenStore
    {
        private static readonly ConcurrentDictionary<string, DocumentStore> _Instance = new ConcurrentDictionary<string, DocumentStore>();
        private static readonly object Padlock = new object();

        public static DocumentStore Instance(string database)
        {
            DocumentStore store;
            if (_Instance.TryGetValue(database, out store))
            {
                return store;
            }

            lock (Padlock)
            {
                if (!_Instance.TryGetValue(database, out store))
                {
                    var thisInstance = new DocumentStore { ConnectionStringName = "raven", DefaultDatabase = database };
                    thisInstance.Initialize();
                    _Instance[database] = thisInstance;
                    return thisInstance;
                }

                return store;
            }
        }

        public static void Dispose()
        {
            if (_Instance == null) return;
            foreach (var documentStore in _Instance)
            {
                documentStore.Value.Dispose();
            }
        }
    }
}