namespace Fanex.Alpha.Before.Common.Repositories
{
    using System;
    using Data;

    public static class ObjectDbFactory
    {
        [ThreadStatic]
        private static Func<string, IObjectDb> createInstance;

        public static Func<string, IObjectDb> CreateInstance
        {
            get { return createInstance ?? ((settingKey) => new ObjectDb(settingKey)); }
            internal set { createInstance = value; }
        }
    }
}
