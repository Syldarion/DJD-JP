using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class FeedbackManager : MonoBehaviour
{
    public Dropdown FeedbackTypeSelection;
    public InputField FeedbackSubjectInput;
    public InputField FeedbackInput;

    string feedback_email;
    string feedback_password;
    string receiver_email;

	void Start()
	{
        feedback_email = "djd.feedback@gmail.com";
        feedback_password = "fuckyoudontstealmypassword";
        receiver_email = "makelacaleb@gmail.com";
	}
	
	void Update()
	{

	}

    public void SendFeedback()
    {
        SmtpClient client = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = (ICredentialsByHost)new NetworkCredential(feedback_email, feedback_password),
            Timeout = 20000
        };

        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

        MailMessage message = new MailMessage();
        message.From = new MailAddress(feedback_email);
        message.To.Add(new MailAddress("davyjonesdevelopment@gmail.com"));
        message.Subject = string.Format("[{0}]-{1}", FeedbackTypeSelection.captionText.text, FeedbackSubjectInput.text);
        message.Body = FeedbackInput.text;

        client.SendAsync(message, "feedbacksent");

        message.Dispose();
    }
}
