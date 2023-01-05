namespace Fanex.Alpha.Before.Models
{
    public class LimitedCountriesConfigModel
    {
        public LimitedCountriesConfigModel(
            string name,
            bool isTesting,
            string testingIp)
        {
            Name = name;
            IsTesting = isTesting;
            TestingIp = testingIp;
        }

        public string Name { get; }

        public bool IsTesting { get; }

        public string TestingIp { get; }
    }
}
