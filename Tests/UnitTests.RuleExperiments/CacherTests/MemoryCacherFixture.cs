using System;
using Application.RuleExperiments.Cachers;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.CacherTests
{
    [TestFixture]
    public class MemoryCacherFixture
    {
        [Test]
        public void Should_cache_and_get()
        {
            MemoryCacher cacher = new MemoryCacher();
            cacher.AddOrUpdate("mykey", cacher);

            var cached = cacher.Get<MemoryCacher>("mykey");

            Assert.IsNotNull(cached);
            Assert.AreSame(cached, cacher);
        }
    }
}