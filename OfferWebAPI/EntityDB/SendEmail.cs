using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net.Mail;
/// <summary>
/// Summary description for SendEmail
/// </summary>
public static class SendEmail
{
    private static string senderEmail = "allinoneofferbd@gmail.com";
    private static string senderEmailPassword = "qaws0987";
    private static string senderMailMask = "Discount Buzz";
    public static void SendMail(string Subject, string strTo, string strBody)
    {
        
        try
        {
            MailMessage mail = new MailMessage();
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            mail.To.Add(strTo);
            // mail.To.Add("mzaman.it.chq@onebankbd.com");
            // mail.CC.Add("");
            //mail.Bcc.Add("amir.hossain@onebank.com.bd");
            //mail.From = new MailAddress("mail@discountbuzzbd.info", "Discount Buzz");
            //SmtpClient smtp = new SmtpClient("mail.kelp.arvixe.com", 26);
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential("mail@discountbuzzbd.info", "InfoDiscount25");

            mail.From = new MailAddress(senderEmail, senderMailMask);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;
            smtp.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }
    public static void SendMail(string Subject, string strTo, string strCC, string strBody)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            mail.To.Add(strTo);
            // mail.To.Add("mzaman.it.chq@onebankbd.com");
             mail.CC.Add(strCC);
            mail.From = new MailAddress(senderEmail, senderMailMask);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;
            smtp.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }
    public static void SendMail(string strAttach,string Subject, string strTo, string strCC, string strBody)
    {
        try
        {
            Attachment oAttach1 = new Attachment(strAttach);
            MailMessage mail = new MailMessage();
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            mail.To.Add(strTo);
            mail.Attachments.Add(oAttach1);
            // mail.To.Add("mzaman.it.chq@onebankbd.com");
            if(!string.IsNullOrEmpty(strCC))
            mail.CC.Add(strCC);

            mail.From = new MailAddress(senderEmail, senderMailMask);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;
            smtp.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }

    public static void SendMail(string strAttach, string Subject, List<string> strToList, string strCC, string strBody)
    {
        try
        {
            Attachment oAttach1 = new Attachment(strAttach);
            MailMessage mail = new MailMessage();
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            foreach (string toMail in strToList)
            {
                mail.To.Add(toMail);
            }
            mail.Attachments.Add(oAttach1);
            // mail.To.Add("mzaman.it.chq@onebankbd.com");
            if (!string.IsNullOrEmpty(strCC))
                mail.CC.Add(strCC);
            mail.From = new MailAddress(senderEmail, senderMailMask);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;
            smtp.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }

    public static void SendMail(string Subject, List<string> strToList, string strCC, string strBody)
    {
        try
        {   
            MailMessage mail = new MailMessage();
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            foreach (string toMail in strToList)
            {
                mail.To.Add(toMail);
            }
            
            // mail.To.Add("mzaman.it.chq@onebankbd.com");
            if (!string.IsNullOrEmpty(strCC))
                mail.CC.Add(strCC);
            mail.From = new MailAddress(senderEmail, senderMailMask);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;
            smtp.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }

    public static void SendMail(string Subject, List<string> strToList,  string strBody)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            foreach (string toMail in strToList)
            {
                mail.To.Add(toMail);
            }

            mail.From = new MailAddress(senderEmail, senderMailMask);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;
            smtp.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }
}