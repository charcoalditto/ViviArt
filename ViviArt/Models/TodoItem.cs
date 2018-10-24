using System;
using SQLite;

namespace ViviArt
{
    public class TodoItem: DefaultModel
    {
        public string Title { get; set; }
        public string Why { get; set; }
        public string How { get; set; }
        public string Plan { get; set; }

        [Indexed]
        public string ExpiryDt { get; set; }
        public string CompleteDt { get; set; }
        public bool NoExpiryDt { get; set; }
    }
}
