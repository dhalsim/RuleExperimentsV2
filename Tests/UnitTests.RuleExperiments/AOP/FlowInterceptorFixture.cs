using Domain.RuleExperiments;
using Domain.RuleExperiments.Attributes.Rule;
using Domain.RuleExperiments.ImplementationTypes;
using Domain.RuleExperiments.Interfaces;
using HaveBox;
using IocContainer.RuleExperiments.HaveBox;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.AOP
{
    public class FlowInterceptor : IInstantiationInterceptor
    {
        private IFlowLogger _flowLogger;

        public void Intercept(IInstantiation instantiation)
        {
            _flowLogger = IocContainerFactory.Current.GetInstance<IFlowLogger>();
            instantiation.Proceed();
        }
    }

    public interface ISearchFlight
    {
        [FlowRule("SearchProvider", typeof(SearchFlightTypes))]
        void Search();
    }

    public class AmadeusSearchFlight : ISearchFlight
    {
        public void Search()
        {
            
        }
    }

    public class ExternalSearchFlight : ISearchFlight
    {
        public void Search()
        {

        }
    }
    public class AmadeusAndExternalSearchFlight : ISearchFlight
    {
        public void Search()
        {

        }
    }

    //TODO: flow interceptor
    //[TestFixture]
    public class FlowInterceptorFixture
    {
        private IFlowLogger _flowLogger;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.For<ISearchFlight>().Use<AmadeusSearchFlight>().AndInterceptInstantiationWith<FlowInterceptor>();
            });

            IocContainerFactory.Current = new IocContainerImplementation(container);

            _flowLogger = IocContainerFactory.Current.GetInstance<IFlowLogger>();
        }

        //TODO: flow interceptor
        //[Test]
        //public void Should_create_flow_from_invocation()
        //{
        //    Assert.AreEqual(1, _flowLogger.GetFlowLogs().Count);
        //}
    }
}