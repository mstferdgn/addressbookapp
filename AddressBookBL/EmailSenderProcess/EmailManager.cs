using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.EmailSenderProcess
{
    public class EmailManager : IEmailManager
    {

        private readonly IConfiguration _configuration;

        public EmailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(EmailMessageModel model)
        {
            try
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("hgyazilimsinifi@outlook.com");
                mail.To.Add(new MailAddress(model.To));
                mail.Subject = model.Subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.Body = model.Body;
                //Not: CC olacaksa buraya kodları eklememiz gerekiyor
                //Not: Bcc olacaksa buraya kodları eklememiz gerekiyor

                SmtpClient client = new SmtpClient();
                //Not: mayıs  2022 tarihine kadar gmail için de aynısını yapardık
                //Ama sistemi güvenlik nedeniyle değiştirdiler
                //Gmail kullanabilmemiz için gmailden token almamız gerekli

                //Not: Güvenlik nedeniyle hesabın şifresini ve adını böyle yazmamlıyız.
                //Veri tabanında Parameters ya da Degerler tablosu şeklinde bir tabloda tutabiliriz.
                client.Credentials = new System.Net.NetworkCredential("hgyazilimsinifi@outlook.com", "betulkadikoy2023");
                client.Port = 587; //25 
                client.Host = "smtp-mail.outlook.com";
                client.EnableSsl = true;


                client.Send(mail);
                return true;
            }
            catch (Exception)
            {

                return false;

            }

        }

        public bool SendEmailviaGmail(EmailMessageModel model)
        {
            try
            {
                //appsettings içindeki gmail bilgilerini alacağım

                var gmailaddress = _configuration.GetSection("GmailSettings:GmailAddress").Value;
                var token = _configuration.GetSection("GmailSettings:GmailToken").Value;
                var host = _configuration.GetSection("GmailSettings:GmailHost").Value;
                var port = Convert.ToInt32(_configuration.GetSection("GmailSettings:GmailPort").Value);
                var CC = _configuration.GetSection("CC").Value.Split(",");


                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_configuration.GetSection("GmailSettings:GmailAddress").Value);
                mail.To.Add(new MailAddress(model.To));
                mail.Subject = model.Subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.Body = model.Body;
                //Not: CC olacaksa buraya kodları eklememiz gerekiyor
                if (CC!=null && CC.Length>0)
                {
                    foreach (var item in CC)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }
                SmtpClient client = new SmtpClient(host, port);
                client.Credentials = new System.Net.NetworkCredential(gmailaddress,token);
                client.EnableSsl = true;
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        //public Task SendEmailviaGmailAsync(EmailMessageModel model)
        //{
        //    try
        //    {
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ret
        //    }
        //}

        public async Task SendMailAsync(EmailMessageModel model)
        {
            try
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("hgyazilimsinifi@outlook.com");
                mail.To.Add(new MailAddress(model.To));
                mail.Subject = model.Subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.Body = model.Body;
                //Not: CC olacaksa buraya kodları eklememiz gerekiyor
                //Not: Bcc olacaksa buraya kodları eklememiz gerekiyor

                SmtpClient client = new SmtpClient();
                //Not: mayıs  2022 tarihine kadar gmail için de aynısını yapardık
                //Ama sistemi güvenlik nedeniyle değiştirdiler
                //Gmail kullanabilmemiz için gmailden token almamız gerekli

                //Not: Güvenlik nedeniyle hesabın şifresini ve adını böyle yazmamlıyız.
                //Veri tabanında Parameters ya da Degerler tablosu şeklinde bir tabloda tutabiliriz.
                client.Credentials = new System.Net.NetworkCredential("hgyazilimsinifi@outlook.com", "betulkadikoy2023");
                client.Port = 587; //25 
                client.Host = "smtp-mail.outlook.com";
                client.EnableSsl = true;


                //client.SendAsync(mail, null); // void işaretlediğiniz metot ile kullabnılabilir
                await client.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                //logtablea kayıt atılabilir 

            }
        }


    }
}
