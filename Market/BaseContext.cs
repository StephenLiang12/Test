using System.Data.Entity;

namespace Market
{
    public class BaseContext<T> : DbContext where T: DbContext
    {
        static BaseContext()
        {
            Database.SetInitializer<T>(null);
        }

        public BaseContext(string database)
            : base(database)
        { }
    }
}
