using System;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.DP
{
    public interface IChairClass
    {
        int Calculate();
    }

    public class EChairClass : IChairClass
    {
        public int Calculate()
        {
            return 10;
        }
    }

    public class CChairClass : IChairClass
    {
        public int Calculate()
        {
            return 20;
        }
    }

    public class BChairClass : IChairClass
    {
        public int Calculate()
        {
            return 30;
        }
    }

    public class FChairClass : IChairClass
    {
        public int Calculate()
        {
            return 40;
        }
    }

    public class Agent
    {
        public IChairClass ChairClass { get; set; }

        public Agent(string className)
        {
            ChairClass = GetChairClass(className);
        }

        public int Calculate()
        {
            return ChairClass.Calculate();
        }

        public static IChairClass GetChairClass(string className)
        {
            switch (className)
            {
                case "E":
                    return new EChairClass();
                case "C":
                    return new CChairClass();
                case "B":
                    return new BChairClass();
                case "F":
                    return new FChairClass();
                default:
                    throw new NotSupportedException(string.Format("{0} type class name is not supported.", className));
            }
        }
    }

    public class Client
    {
        public int GetAgentAmount(string className)
        {
            var agent = new Agent(className);
            return agent.Calculate();
        }
    }

    [TestFixture]
    public class StrategyFixture
    {
        [Test]
        public void CommissionAndAgent()
        {
            var client = new Client();
            Assert.AreEqual(client.GetAgentAmount("B"), 30);
        }
    }
}