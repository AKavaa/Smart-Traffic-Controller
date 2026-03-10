public interface IEmailService
{
    string SendMail(string emailAddress, string subject, string message);

}
