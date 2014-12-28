﻿using Application.RuleExperiments.Loggers;
using Application.RuleExperiments.RuleProviders;
using Domain.RuleExperiments;
using Domain.RuleExperiments.Interfaces;
using HaveBox;
using StateMachine;

namespace IocContainer.RuleExperiments.HaveBox
{
    public static class Initializer
    {
        public static void Initialize()
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.For<IRuleProvider>().Use<StaticRuleProvider>();
                config.For<IRuleProvider>().Use<BasicRuleProvider>();
                config.For<ILogger>().Use<StaticLogger>().AsSingleton();
                config.For<IStateMachine>().Use<SimpleStateMachine>().AsSingleton();
            });

            IocContainerFactory.Current = new IocContainerImplementation(container);
        }
    }
}