using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace GestureService
{
    public partial class GestureService : ServiceBase
    {
        Timer timer1;
        public GestureService()
        {
            InitializeComponent();
            timer1 = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            //Debugger.Launch();
            try
            {
                DateTime dateNow = DateTime.Now;
                timer1.Enabled = true;
                timer1.Interval = getInterval(true);
                timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);

                Logger.WriteLog("OnStart"
                    , "Service started Successfully"
                    , "timer1 will call on: " + timer1.Interval.ToString() + " DateTime: " + DateTime.Now.AddMilliseconds(Convert.ToDouble(timer1.Interval.ToString())));
            }
            catch (Exception ex)
            {
                timer1.Enabled = true;
                Logger.WriteLog("OnStart", "Error occoured in Starting Service", "Error Message: '" + ex.ToString(), ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            try
            {
                timer1.Enabled = false;
                Logger.WriteLog("OnStop", "Service Stopped Successfully");
            }
            catch (Exception ex)
            {
                timer1.Enabled = true;
                Logger.WriteLog("OnStop", "Error occoured in stopping Service", "Error Message: '" + ex.ToString(), ex.StackTrace);
            }
        }

        #region Methods
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                Logger.WriteLog("timer1_Elapsed", "called");
                DeliverGestures();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("timer1_Elapsed", "Error Message: '" + ex.ToString(), ex.StackTrace);
            }
            finally
            {
                // 2. If tick for the first time, reset next run to every 24 hours
                if (timer1.Interval != 1800000)//43200000
                {
                    timer1.Interval = getInterval(false);
                }
                timer1.Enabled = true;
                Logger.WriteLog("timer1_Elapsed Finally", "Next Call" ,"timer1 will now call on interval: " + timer1.Interval.ToString() + " DateTime: " + DateTime.Now.AddMilliseconds(Convert.ToDouble(timer1.Interval.ToString())),"");
            }
        }
        private void DeliverGestures()
        {
            DAL dal = new DAL();

            #region SMS
            try
            {
                Logger.WriteLog("SMS Send", "SMS Sending");
                dal.SendSMS();
            }
            catch (Exception Error)
            {
                Logger.WriteLog("SMS Send", "SMS Send Error Occoured", Error.StackTrace);
            }
            #endregion

            #region Email
            try
            {
                Logger.WriteLog("Email Send", "Email Sending");
                dal.SendEmail();
            }
            catch (Exception Error)
            {
                Logger.WriteLog("Email Send", "Email Send Error Occoured", Error.StackTrace);
            }
            #endregion

            #region Notice
            //Ecard notice
            try
            {
                Logger.WriteLog("Notice Send", "Notice Sending");
                dal.SendNotice();
            }
            catch (Exception Error)
            {
                Logger.WriteLog("Notice Send", "Notice Send Error Occoured", Error.StackTrace);
            }
            #endregion

            #region APM
            int TotalUsers = 0;
            int TotalAPMGestures = 0;

            DataTable dtAPMGestureRequests = new DataTable();
            dtAPMGestureRequests = dal.GetPedingAPMRequests();

            DataView view = new DataView(dtAPMGestureRequests);
            DataTable distinctUsers = view.ToTable(true, "UserCode");

            TotalUsers = distinctUsers.Rows.Count;
            TotalAPMGestures = dtAPMGestureRequests.Rows.Count;

            Logger.WriteLog("APM Generate Gestures", "called", "Total Users: " + TotalUsers);
            Logger.WriteLog("APM Generate Gestures", "called", "Total Gestures: " + TotalAPMGestures);

            string ClientKey = string.Empty;
            ClientKey = Convert.ToString(ConfigurationManager.AppSettings["ShipmentClientKey"]);
            string[] Message;

            xshipment.ShipmentAPI obj = new xshipment.ShipmentAPI();

            for (int i = 0; i < TotalAPMGestures; i++)
            {
                int userId = Convert.ToInt32(dtAPMGestureRequests.Rows[i]["UserCode"].ToString());
                int GestureDetailCode = Convert.ToInt32(dtAPMGestureRequests.Rows[i]["GestureDetailCode"].ToString());
                int GestureCode = Convert.ToInt16(dtAPMGestureRequests.Rows[i]["GestureCode"].ToString());

                //Card APM //Letter APM
                if (GestureCode == Convert.ToInt16(OG_Gestures.Card) || GestureCode == Convert.ToInt16(OG_Gestures.Letter))
                {
                    try
                    {
                        Logger.WriteLog("APM Send", "APM Sending to User ID: " + userId, "", GestureDetailCode.ToString());
                        string DocumentIds = dtAPMGestureRequests.Rows[i]["DocumentIds"].ToString();

                        if (!string.IsNullOrEmpty(DocumentIds))
                        {
                            DataSet dsPrintDataset = dsPrintDataset = new DataSet();
                            dsPrintDataset = dal.GetDatasetForPrintingWebAPIInBulk(GestureDetailCode, DocumentIds);

                            if (dsPrintDataset != null && dsPrintDataset.Tables.Count > 3)
                            {                                
                                Message = obj.AddOrder(ClientKey, dsPrintDataset);

                                if (Message.Length >= 2)
                                {
                                    //error
                                    if (Convert.ToInt32(Message[0]) <= 0)
                                    {
                                        Logger.WriteLog("APM Send", "APM not send", Message[2].ToString(), GestureDetailCode.ToString());

                                        StringWriter sw = new StringWriter();
                                        dsPrintDataset.WriteXml(sw);
                                        string result = sw.ToString();
                                        Logger.WriteLog("User gesture detail code: " + GestureDetailCode, result);
                                    }
                                    else
                                    {
                                        dal.UpdateUserGestureDetail(GestureDetailCode, Convert.ToInt16(OG_GestureStatus.Pending), Convert.ToInt16(OG_PrintingStatus.SendForPrinting));
                                    }
                                }
                            }
                        }
                        else
                        {
                            Logger.WriteLog("APM Send", "APM not send", "Invalid Document Id", GestureDetailCode.ToString());
                        }
                    }
                    catch (Exception Error)
                    {
                        Logger.WriteLog("APM Send", "APM Send Error Occoured", Error.StackTrace, GestureDetailCode.ToString());
                    }

                }

            }
            #endregion
        }
        private double getInterval(bool isOnStart)
        {
            DateTime _scheduleTimeCards;
            DateTime dateNow = DateTime.Now;
            DateTime dateTomorrow = dateNow.AddDays(1);
            if (isOnStart)
            {
                _scheduleTimeCards = Convert.ToDateTime(dateNow.Month.ToString() + "-" + dateNow.Day.ToString() + "-" + dateNow.Year.ToString() + " " + ConfigurationManager.AppSettings["ServiceStartTime"].ToString());
                if (_scheduleTimeCards < dateNow)
                {
                    _scheduleTimeCards = Convert.ToDateTime(dateTomorrow.Month.ToString() + "-" + dateTomorrow.Day.ToString() + "-" + dateTomorrow.Year.ToString() + " " + ConfigurationManager.AppSettings["ServiceStartTime"].ToString());
                }
                return _scheduleTimeCards.Subtract(dateNow).TotalSeconds * 1000;
            }
            else
            {
                DateTime timeAfterHalfHour = dateNow.AddMinutes(30);
                return timeAfterHalfHour.Subtract(dateNow).TotalSeconds * 1000;
            }
            
        }

        private string getSubString(string Data, int maxLength)
        {
            return Data.Substring(0, Data.Length > maxLength ? maxLength : Data.Length);
        }
        #endregion
    }
}

