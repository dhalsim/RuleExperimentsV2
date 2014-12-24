using Domain.RuleExperiments.Attributes;
using System;
using Domain.RuleExperiments.Exceptions;

namespace Application.RuleExperiments.MethodDecoratorWeaverTestClasses
{
    public abstract class BaseTestClass : MarshalByRefObject
    {
        public abstract void Test();
        public abstract bool Test2();
    }

	[TryCatchDecorator]
	public class TestClass : BaseTestClass
	{
        [TryCatchDecorator]
		public override void Test()
        {
            int b = 0;
            int a = 10 / b;
        }

        [TryCatchDecorator]
	    public override bool Test2()
	    {
            int b = 0;
            int a = 10 / b;
            return true;
	    }
	}

	public class TestClass2 : BaseTestClass
	{
		[TryCatchDecorator]
        public override void Test()
		{
            throw new Exception();
		}

        [TryCatchDecorator]
	    public override bool Test2()
	    {
            int b = 0;
            int a = 10 / b;
            return true;
	    }
	}

    public class TestClass3 : BaseTestClass
    {
        [LoggerDecorator]
        public override void Test()
        {
            
        }

        [LoggerDecorator]
        public override bool Test2()
        {
            return true;
        }
    }
}