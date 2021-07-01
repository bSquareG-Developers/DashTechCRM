using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;


namespace DashTechCRM.Models
{
    public class SMTPEmailSendingModel
    {
        public static bool Send(string to, string messageBody, string subject, string cc = "", string bcc = "")
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("reporting.alerts@dashtechinc.com");

                string[] emailsTo = to.Split(',');
                if (emailsTo.Length > 0)
                {
                    List<string> emlist = new List<string>();
                    foreach (var eml in emailsTo)
                    {
                        if (eml != "")
                        {
                            emlist.Add(eml);
                        }
                    }
                    string toAll = String.Join(", ", emlist.ToArray());
                    message.To.Add(new MailAddress(toAll));

                }
                else
                {
                    message.To.Add(new MailAddress(to));
                }
                if (cc != "" && cc.Length > 6)
                {
                    foreach (var item in cc.Split(','))
                    {
                        if (item == "") { continue; }

                        message.CC.Add(item);
                    }
                }
                if (bcc != "")
                {
                    foreach (var item in bcc.Split(','))
                    {
                        if (item == "") { continue; }
                        message.Bcc.Add(item);
                    }
                }
                message.Subject = subject;
                message.IsBodyHtml = true;

                message.Body = messageBody;
                //smtp.Port = 587;
                //smtp.Host = "smtp.gmail.com"; //for gmail host  
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential("dashtech.reports@gmail.com", "DashTech@007");
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Host = "smtp.zoho.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("reporting.alerts@dashtechinc.com", "d0X7CrVXwPuR");
                smtp.Send(message);
            }
            catch (Exception err)
            {
                return false;
            }
            return true;
        }
    }
}