using Application.RuleExperiments.Loggers;
using Domain.RuleExperiments;
using Domain.RuleExperiments.Attributes;
using Domain.RuleExperiments.Interfaces;
using HaveBox;
using IocContainer.RuleExperiments.HaveBox;
using IocContainer.RuleExperiments.HaveBox.Interceptors;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.AOP
{
    public abstract class InstantiableClass
    {
        [InvocationLogAttribute]
        public abstract void Method();
    }

    public class InstantiableClass2 : InstantiableClass
    {
        public override void Method()
        {
            
        }
    }

    [TestFixture]
    public class InvocationInterceptorFixture
    {
        private ILogger _logger;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.For<InstantiableClass2>().Use<InstantiableClass2>().AndInterceptMethodsWith<InvocationInterceptor>();
                config.For<ILogger>().Use<StaticLogger>().AsSingleton();
            });

            IocContainerFactory.Current = new IocContainerImplementation(container);

            _logger = IocContainerFactory.Current.GetInstance<ILogger>();
        }

        [Test]
        public void Should_create_proxy_with_interceptor()
        {
            var obj = IocContainerFactory.Current.GetInstance<InstantiableClass2>();
            Assert.IsNotNull(obj);
            obj.Method();

            Assert.AreEqual(2, _logger.GetLogs().Count);
            _logger.Clear();
        }
    }
}