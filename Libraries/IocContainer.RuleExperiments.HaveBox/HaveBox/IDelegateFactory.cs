using HaveBox.Configuration;

namespace HaveBox
{
    internal interface IDelegateFactory
    {
        DelegateFactory.CreateInstance GetCreateInstanceFromLambdaFunction(TypeDetails typeDetails);
        DelegateFactory.CreateInstance GetCreateInstanceSingleton(TypeDetails typeDetails);
        DelegateFactory.CreateInstance GetCreateInstanceLazySingleton(TypeDetails typeDetailsLocal, DelegateFactory.CreateInstance instansiationDelegate);
        DelegateFactory.CreateInstance GetCreateInstanceDelegate(TypeDetails typeDetails);
        DelegateFactory.CreateInstance GetCreateInstanceWithInterception(TypeDetails typeDetailsLocal, DelegateFactory.CreateInstance delegateToBeIntercepted);
        DelegateFactory.CreateInstance GetCreateInstanceIEnumerable(TypeDetails typeDetailsLocal);
    }
}
