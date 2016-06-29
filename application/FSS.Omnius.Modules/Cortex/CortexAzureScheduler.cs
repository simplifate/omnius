using FSS.Omnius.Modules.Cortex.Interface;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System;
using System.Collections;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;
using Microsoft.Azure;
using System.Xml.Linq;
using System.Linq;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;

namespace FSS.Omnius.Modules.Cortex
{
    public class CortexAzureScheduler : ICortexAPI
    {
        private HttpRequestBase Request;

        private string subscriptionId;
        private string cloudServiceId;
        private string jobCollectionId;
        private string subscriptionCertificate;

        private string systemAccountName;
        private string systemAccountPass;

        public CortexAzureScheduler(HttpRequestBase request)
        {
            Request = request;
            GetSettings();
        }

        public string List()
        {
            SchedulerClient client = GetClient();
            JobListResponse result = client.Jobs.List(new JobListParameters());

            string o = "<ul>";
            foreach(Job j in result) {
                o += "<li>" + j.Id + "</li>";
            }
            o += "</ul>";

            return o;
        }

        public void Create(Task model)
        {
            SchedulerClient client = GetClient();
            JobCreateParameters p = GetParams(model);
            JobCreateResponse result = client.Jobs.Create(p);


        }

        public void Change(Task model, Task original)
        {
            Delete(original);
            Create(model);
        }

        public void Delete(Task model)
        {
            SchedulerClient client = GetClient();
            string jobId = GetJobId(model);
            var response = client.Jobs.Delete(jobId);
        }
        
        #region tools

        private void GetSettings()
        {
            subscriptionId = WebConfigurationManager.AppSettings["CortexAzureSubscriptionId"];
            cloudServiceId = WebConfigurationManager.AppSettings["CortexAzureCloudServiceId"];
            jobCollectionId = WebConfigurationManager.AppSettings["CortexAzureJobCollectionId"];
            subscriptionCertificate = WebConfigurationManager.AppSettings["CortexAzureSchedulerCertificate"];

            systemAccountName = WebConfigurationManager.AppSettings["SystemAccountName"];
            systemAccountPass = WebConfigurationManager.AppSettings["SystemAccountPass"];
        }

        private JobCreateParameters GetParams(Task model)
        {
            JobCreateParameters p = new JobCreateParameters();

            p.Action = new JobAction()
            {
                Type = JobActionType.Http,
                Request = new JobHttpRequest()
                {
                    Body = "",
                    Method = "GET",
                    Uri = new Uri("http://" + Request.Url.Host + model.Url)
                }
            };
            p.StartTime = model.Start_Date == null ? DateTime.UtcNow : model.Start_Date + model.Start_Time;

            JobRecurrence r = new JobRecurrence();

            switch(model.Type) {
                case ScheduleType.DAILY:
                    r.Frequency = JobRecurrenceFrequency.Day;
                    r.Interval = model.Daily_Repeat;
                    break;
                case ScheduleType.WEEKLY:
                    r.Frequency = JobRecurrenceFrequency.Week;
                    r.Interval = model.Weekly_Repeat;
                    r.Schedule = new JobRecurrenceSchedule()
                    {
                        Days = GetWeeklyDays(model.Weekly_Days),
                    };
                    break;
                case ScheduleType.MONTHLY:
                    r.Frequency = JobRecurrenceFrequency.Month;
                    r.Schedule = CreateMonthlySchedule(model);
                    break;
                case ScheduleType.ONCE:
                    r.Count = 1;
                    break;
                case ScheduleType.ONIDLE:
                    throw new NotImplementedException("Pro Azure scheduler nelze plánovat OnIdle úlohy");
            }

            if(model.End_Date != null) {
                r.EndTime = model.End_Date + model.End_Time;
            }
            
            p.Recurrence = r;

            return p;
        }

        private JobRecurrenceSchedule CreateMonthlySchedule(Task model)
        {
            JobRecurrenceSchedule s = new JobRecurrenceSchedule();

            if(model.Monthly_Type == MonthlyType.DAYS) {
                List<int> monthDays = new List<int>();
                int i = 1;
                foreach (DaysInMonth d in Enums<DaysInMonth>()) {
                    if(((DaysInMonth)model.Monthly_Days).HasFlag(d)) {
                        if(d != DaysInMonth.Last) {
                            monthDays.Add(i);
                        }
                        else {
                            monthDays.Add(-1);
                        }
                    }
                    i++;
                }
                if (monthDays.Count > 0) {
                    s.MonthDays = monthDays;
                }
            }
            if(model.Monthly_Type == MonthlyType.IN) {
                List<JobScheduleDay> days = GetWeeklyDays(model.Monthly_In_Days);
                if(days.Count > 0) {
                    s.Days = days;
                }

                List<JobScheduleMonthlyOccurrence> oList = new List<JobScheduleMonthlyOccurrence>();
                int mIndex = 1;
                foreach(InModifiers mod in Enums<InModifiers>()) {
                    if(((InModifiers)model.Monthly_In_Modifiers).HasFlag(mod)) {
                        foreach(JobScheduleDay d in days) {
                            JobScheduleMonthlyOccurrence o = new JobScheduleMonthlyOccurrence();
                            o.Day = d;
                            o.Occurrence = mod == InModifiers.LAST ? -1 : mIndex;
                            oList.Add(o);  
                        }
                    }
                    mIndex++;
                }

                if (oList.Count > 0) {
                    s.MonthlyOccurrences = oList;
                }
            }

            List<int> months = new List<int>();
            int m = 1;
            foreach(Months month in Enums<Months>()) {
                if (((Months)model.Monthly_Months).HasFlag(month)) months.Add(m);
                m++;
            }
            if(months.Count > 0) {
                s.Months = months;
            }



            return s;
        }

        private List<JobScheduleDay> GetWeeklyDays(int? days)
        {
            List<JobScheduleDay> list = new List<JobScheduleDay>();
            if(days != null) {
                int i = 0;
                foreach(Days day in Enums<Days>()) {
                    if (((Days)days).HasFlag(day)) list.Add((JobScheduleDay)i);
                    i++;
                }
            }
            return list;
        }

        private string GetJobId(Task model)
        {
            string jobId = string.Format("{0}_{1}", model.Id, model.Name);
            return ExtendMethods.URLSafeString(jobId);
        }

        private SchedulerClient GetClient()
        {
            var credentials = GetCertFromPublishSettingsFile();
            return new SchedulerClient(cloudServiceId, jobCollectionId, credentials);
        }

        public CertificateCloudCredentials GetCertFromPublishSettingsFile()
        {
            var certificate = new X509Certificate2(Convert.FromBase64String(subscriptionCertificate));
            return new CertificateCloudCredentials(subscriptionId, certificate);
        }

        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        #endregion
    }
}
