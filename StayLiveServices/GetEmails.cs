using System;
using StayLiveServices.Model;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using StayLiveServices.Emails;
using System.Diagnostics;

namespace StayLiveServices
{
    public partial class GetEmails : ServiceBase
    {
        StayLiveEntities db = new StayLiveEntities();
        Timer timer = new Timer();

        Timer level = new Timer();
        public GetEmails()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(CheckDeployment);
            timer.Interval = 300000;//900000//180000//120000
            timer.Enabled = true;

            level.Elapsed += new ElapsedEventHandler(CheckLevels);
            level.Interval = 900000;//900000//180000//120000
            level.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            level.Enabled = false;
        }

        private void CheckDeployment(object source, ElapsedEventArgs e)
        {
            foreach (var company in db.Companies)
            {
                CheckEmails(company);
            }
        }

        
        private void CheckLevels(object source, ElapsedEventArgs e)
        {
            foreach (var company in db.Companies)
            {
                UpdateLevels(company);
            }
            db.SaveChanges();
        }

        private void CheckEmails(Company company)
        {
            try
            {
                using (Pop3Client client = new Pop3Client())
                {
                    client.Connect(company.Pop3Address, Convert.ToInt32(company.Pop3Port), (bool)company.EnableSsl);
                    client.Authenticate(company.EmailAddress, company.EmailPassword);
                    for (int i = client.GetMessageCount(); i > 0; i--)
                    {
                        MessageHeader headers = client.GetMessageHeaders(i);
                        RfcMailAddress from = headers.From;
                        Email email = new Email();
                        switch (from.Address.Split('@').Last())
                        {
                            case "gmail.com":
                            case "yahoo.com":
                                {
                                    email.GetEmails(i, "On ", company);
                                    break;
                                }
                            case "outlook.com":
                            case "hotmail.com":
                                {
                                    email.GetEmails(i, "__________", company);
                                    break;
                                };
                            default:
                                {
                                    email.GetEmails(i, "From", company);
                                    break;
                                }
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateLevels(Company company)
        {
            try
            {
                var now = DateTime.Now;
                var Level = db.Levels.Where(a => a.CompanyId == company.Id).FirstOrDefault();
                var Tickets = db.Tickets.Where(a => (a.Level == 1 || a.Level == 2) && a.CompanyId == company.Id && a.Status != 3).ToList();
                if (Tickets != null)
                {
                    foreach (var tk in Tickets)
                    {
                        if (tk.Level == 1 && now > tk.CreateDate.Value.AddHours(Level.FirstHours))
                        {
                            tk.Level = 2;
                        }
                        else if (tk.Level == 2 && now > tk.CreateDate.Value.AddHours(Level.FirstHours))
                        {
                            tk.Level = 3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
