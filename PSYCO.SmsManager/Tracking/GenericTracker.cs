using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.Common.BaseModels;

namespace PSYCO.SmsManager.Tracking
{
    public class GenericTracker<T> :IObservable<T> 
    {

        private  static List<IObserver<T>> observers = new List<IObserver<T>>(); 

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);

        }


        public void Publish(T value)
        {
            foreach (var observer in observers)
            {
                if (value == null)
                    observer.OnError(new Exception());
                else
                    observer.OnNext(value);
            }
        }



        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
