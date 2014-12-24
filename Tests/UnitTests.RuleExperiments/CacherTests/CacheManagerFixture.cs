using Application.RuleExperiments.Cachers;
using Domain.RuleExperiments.Attributes;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.CacherTests
{
    public class CacheObject
    {
        [CacheKeyAttribute]
        public string Prop1 { get; set; }

        [CacheKeyAttribute]
        public string Prop2 { get; set; }

        public CacheObject()
        {
            Prop1 = "prop1";
            Prop2 = "prop2";
        }
    }

    public class CacheObject2 : CacheObject
    {
        [CacheKeyAttribute]
        private string Prop3 { get; set; }

        public CacheObject2()
        {
            Prop3 = "prop3";
        }
    }

    [TestFixture]
    public class CacheManagerFixture
    {
        [Test]
        public void Should_get_cache_key()
        {
            CacheObject object1 = new CacheObject();
            string key = CacheManager.GetCacheStringFromObject(object1);

            Assert.AreEqual("prop1prop2", key);
        }

        [Test]
        public void Should_get_cache_key_with_inherited()
        {
            CacheObject object1 = new CacheObject2();
            string key = CacheManager.GetCacheStringFromObject(object1);

            Assert.AreEqual("prop3prop1prop2", key);
        }
    }
}