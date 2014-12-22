using Domain.RuleExperiments.Attributes;

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

        }
    }
}