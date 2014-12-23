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

	[MethodDecorator]
	public class TestClass : BaseTestClass
	{
        [MethodDecorator]
		public override void Test()
        {
            int b = 0;
            int a = 10 / b;
        }

        [MethodDecorator]
	    public override bool Test2()
	    {
            int b = 0;
            int a = 10 / b;
            return true;
	    }
	}

	public class TestClass2 : BaseTestClass
	{
		[MethodDecorator]
        public override void Test()
		{
            throw new Exception();
		}

        [MethodDecorator]
	    public override bool Test2()
	    {
            int b = 0;
            int a = 10 / b;
            return true;
	    }
	}
}