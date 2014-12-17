using System.Reflection;

namespace HaveBox.HaveBoxProxy
{
    public interface IInvocation
    {
        object[] Args { get; set; }
        MethodInfo Method { get; }
        object ReturnObject { get; set; }

        void Proceed();
    }
}
