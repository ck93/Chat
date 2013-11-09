using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Chat
{
    class Mail
    {
        /// <summary> 
        /// 发送电子邮件 
        /// </summary> 
        /// <param name="MessageFrom">发件人邮箱地址</param> 
        /// <param name="MessageTo">收件人邮箱地址</param> 
        /// <param name="MessageSubject">邮件主题</param> 
        /// <param name="MessageBody">邮件内容</param> 
        /// <returns></returns> 
        public static bool Send(MailAddress MessageFrom, string MessageTo, string MessageSubject, string MessageBody)
        {
            MailMessage message = new MailMessage();

            message.From = MessageFrom;
            message.To.Add(MessageTo); //收件人邮箱地址可以是多个以实现群发 
            message.Subject = MessageSubject;
            message.Body = MessageBody;
            message.IsBodyHtml = false; //是否为html格式 
            message.Priority = MailPriority.High; //发送邮件的优先等级

            SmtpClient sc = new SmtpClient();
            sc.Host = "59.66.133.208"; //指定发送邮件的服务器地址或IP 
            sc.Port = 25; //指定发送邮件端口

            sc.Credentials = new System.Net.NetworkCredential("easychat@imchenkai.cn", "easychat"); //指定登录服务器的用户名和密码(发件人的邮箱登陆密码)

            try
            {
                sc.Send(message); //发送邮件 
            }
            catch (Exception ex)
            {
                //throw (ex);
                return false;
            }
            return true;
        }
    }
}
