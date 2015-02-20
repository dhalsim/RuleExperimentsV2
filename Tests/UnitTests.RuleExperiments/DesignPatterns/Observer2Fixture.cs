using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.DP
{
    //3 hisse senedi A B C
    //A da %10 dan fazla değişiklik olursa haber ver
    //B de değişiklik olursa haber ver

    public interface IObserver
    {
        void Update(int oldValue, Subject observer);
    }

    public abstract class Subject
    {
        private int _amount;
        public string Name { get; set; }

        public List<IObserver> Observers { get; set; }

        public void Register(IObserver subscriber)
        {
            Observers.Add(subscriber);
        }

        public void UnRegister(IObserver subscriber)
        {
            Observers.Remove(subscriber);
        }

        public void Notify(int oldValue)
        {
            foreach (var subscriber in Observers)
            {
                subscriber.Update(oldValue, this);
            }
        }

        public virtual int Amount
        {
            get { return _amount; }
            set
            {
                int oldValue = _amount;
                _amount = value;

                Notify(oldValue);
            }
        }

        protected Subject(string name)
        {
            Name = name;
            Observers = new List<IObserver>();
        }

        public Subject GetState()
        {
            return this;
        }
    }

    public class Hisse : Subject
    {
        public Hisse(string name) : base(name)
        {
            
        }
    }

    public class ClientA : IObserver
    {
        public void Update(int oldValue, Subject observer)
        {
            double percentage;

            if (oldValue == 0)
            {
                percentage = Math.Abs(observer.Amount - (double) oldValue);
            }
            else
            {
                percentage = (Math.Abs(observer.Amount - (double) oldValue)/oldValue)*100;
            }

            if (percentage > 20)
            {
                Debug.WriteLine("Updated {0}, old amount: {1}, new amount {2}, percentage {3}", 
                    observer.Name, oldValue, observer.Amount, percentage);
            }
        }
    }

    public class ClientB : IObserver
    {
        public void Update(int oldValue, Subject observer)
        {
            Debug.WriteLine("Updated {0}, new amount {1}", observer.Name, observer.Amount);
        }
    }

    [TestFixture]
    public class Observer2Fixture
    {
        [Test]
        public void Test()
        {
            ClientA clientA = new ClientA();
            
            Hisse hisseA = new Hisse("A");
            Hisse hisseB = new Hisse("B");

            hisseA.Register(clientA);
            hisseB.Register(clientA);

            hisseA.Amount = 100;
            hisseA.Amount = 121;
            hisseA.Amount = 122;
            hisseA.Amount = 300;

            hisseB.Amount = 0;
            hisseB.Amount = 100;
        }
    }
}