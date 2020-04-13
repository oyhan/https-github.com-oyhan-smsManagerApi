using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.Tracking;

namespace PSYCO.SmsManager.Tracking
{
    public class SentSmsTracker : GenericTracker<int>
    {
        private AppDbContext _db;

        public SentSmsTracker(AppDbContext db)
        {
            _db = db;
        }
        public override IDisposable Subscribe(IObserver<int> observer)
        {
            observer.OnNext(_db.SentSms.Count(s => s.IsSent));
            return base.Subscribe(observer);
        }
    }
}
