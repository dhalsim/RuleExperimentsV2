using Application.RuleExperiments.Loggers;
using Application.RuleExperiments.RuleProviders;
using Domain.RuleExperiments;
using Domain.RuleExperiments.ImplementationTypes;
using Domain.RuleExperiments.Interfaces;
using IocContainer.RuleExperiments.HaveBox;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.IocContainerTests
{
    [TestFixture]
    public class ContainerFixture
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            Initializer.Initialize();
        }

        [Test]
        public void Should_get_staticruleprovider()
        {
            var ruleProvider = IocContainerFactory.Current.GetInstance<IRuleProvider>();
            Assert.IsNotNull(ruleProvider);
            Assert.IsInstanceOf<StaticRuleProvider>(ruleProvider);
        }

        [Test]
        public void Should_get_basicruleprovider()
        {
            var ruleProvider = IocContainerFactory.Current.GetInstance<IRuleProvider>(RuleProviderTypes.BasicRuleProvider);
            Assert.IsNotNull(ruleProvider);
            Assert.IsInstanceOf<BasicRuleProvider>(ruleProvider);
        }

        [Test]
        public void Should_get_staticlogger()
        {
            var logger = IocContainerFactory.Current.GetInstance<ILogger>(LoggerTypes.StaticLogger);
            Assert.IsNotNull(logger);
            Assert.IsInstanceOf<StaticLogger>(logger);
        }

        [Test]
        public void Should_get_staticlogger_with_string()
        {
            var logger = IocContainerFactory.Current.GetInstance<ILogger>("StaticLogger");
            Assert.IsNotNull(logger);
            Assert.IsInstanceOf<StaticLogger>(logger);
        }
    }
}