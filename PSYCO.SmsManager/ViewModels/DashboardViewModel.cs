using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetify;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.Services.Sms;

namespace PSYCO.SmsManager.ViewModels
{
    public class DashboardViewModel :BaseVM
    {
        public int SmsTariff { get; set; }

        public DashboardViewModel(
            AppDbContext db,
            ISmsService smsService

            )
        {

            AddProperty<int>("SentSms").SubscribeTo(smsService.SentSms).PropertyChanged += DashboardViewModel_PropertyChanged; 
            AddProperty<int>("FailedSms").SubscribeTo(smsService.FailedSms).PropertyChanged+= DashboardViewModel_PropertyChanged;
            SmsTariff = smsService.GetSmsTariff().Rate;
        }

        private void DashboardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PushUpdates();
        }
    }
}
