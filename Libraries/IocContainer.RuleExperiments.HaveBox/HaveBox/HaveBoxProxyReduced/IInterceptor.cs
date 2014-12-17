
namespace HaveBox.HaveBoxProxy
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}
