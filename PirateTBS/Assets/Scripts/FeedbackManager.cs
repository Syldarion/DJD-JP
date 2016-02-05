using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Net.Mail;

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
            Port = 465,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = (ICredentialsByHost)new NetworkCredential(feedback_email, feedback_password),
            Timeout = 20000
        };

        MailAddress from = new MailAddress(feedback_email, "DJD Feedback", System.Text.Encoding.UTF8);
        MailAddress to = new MailAddress(receiver_email);

        MailMessage message = new MailMessage(from, to);
        message.Subject = string.Format("[{0}]-{1}", FeedbackTypeSelection.itemText, FeedbackSubjectInput.text);
        message.Body = FeedbackInput.text;

        client.SendAsync(message, "feedbacksender");

        message.Dispose();
    }
}
