using System.Linq;
using System.Reflection;
using Domain.RuleExperiments;
using Domain.RuleExperiments.Attributes;
using Domain.RuleExperiments.Interfaces;
using Domain.RuleExperiments.Models.Log;
using HaveBox.HaveBoxProxy;

namespace IocContainer.RuleExperiments.HaveBox.Interceptors
{
    public class InvocationInterceptor : IInterceptor
    {
        private ILogger _logger;

        public void Log(Log invocationLog)
        {
            _logger.Log(invocationLog);
        }

        public void Intercept(IInvocation invocation)
        {
            _logger = IocContainerFactory.Current.GetInstance<ILogger>();

            MethodInfo method = invocation.Method;

            var attribute = method.GetCustomAttributes(typeof(InvocationLogAttribute), true).FirstOrDefault();
            if (attribute != null)
            {
                var methodName = method.Name;
                Log(new Log(LogLevel.Trace, string.Format("before call to {0}", methodName)));
                invocation.Proceed();
                Log(new Log(LogLevel.Trace, string.Format("after call to {0}", methodName)));
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}