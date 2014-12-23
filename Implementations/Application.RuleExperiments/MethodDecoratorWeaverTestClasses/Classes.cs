using Domain.RuleExperiments.Attributes;
using System;
using Domain.RuleExperiments.Exceptions;

namespace Application.RuleExperiments.MethodDecoratorWeaverTestClasses
{
	[MethodDecorator]
	public class TestClass
	{
		public void Test()
		{
            
		}
	}

	public class TestClass2
	{
		[MethodDecorator]
		public void Test()
		{
			throw new Exception();
		}
	}

    public class TestTarget
    {
        public void Test()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                throw new SystemLevelException("Error Occured", e);
            }
        }
    }
}