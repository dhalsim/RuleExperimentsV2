using HaveBox.Configuration;
using System;
using System.Collections.Generic;

namespace HaveBox
{
    internal class Instantiation : IInstantiation
    {
        private TypeDetails _typeDetails;
        private DelegateFactory.CreateInterceptedInstance _createInterceptetInstance;
        private DelegateFactory.CreateInstance _createInstance;
        private object[] _instancesToBeInjected;
        internal IEnumerable<Type> argTypes;
        internal TypeDetails[] DependenciesTypeDetails;
        internal object instance;

        public Instantiation(TypeDetails typeDetails, DelegateFactory.CreateInstance createInstance, DelegateFactory.CreateInterceptedInstance createInterceptetInstance)
        {
            _typeDetails = typeDetails;
            _createInterceptetInstance = createInterceptetInstance;
            _createInstance = createInstance;
        }

        public void Proceed()
        {
            if (_instancesToBeInjected == null)
            {
                _createInstance(_typeDetails, out instance);
            }
            else
            {
                _createInterceptetInstance(_instancesToBeInjected, out instance);
            }
        }

        public IEnumerable<Type> Arguments
        {
            get { return argTypes; }
        }

        public object[] InstancesToBeInjected
        {
            get
            {
                return _instancesToBeInjected = _instancesToBeInjected ?? CreateDependencyObjects();
            }
            set
            {
                _instancesToBeInjected = value;
            }
        }

        private object[] CreateDependencyObjects()
        {
            var instanceObjects = new List<object>();
            DependenciesTypeDetails.Each(typeDetails =>
                {
                    object instance;
                    typeDetails.CreateInstanceDelegate(typeDetails, out instance);
                    instanceObjects.Add(instance);
                });

            return instanceObjects.ToArray();
        }
    }
}
