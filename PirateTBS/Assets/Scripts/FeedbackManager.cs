using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

public class FeedbackManager : MonoBehaviour
{
    [HideInInspector]
    public static FeedbackManager Instance;

    public Dropdown FeedbackTypeSelection;
    public InputField FeedbackSubjectInput;
    public InputField FeedbackInput;
    public DialogueBox MessageSuccessPanel;

    SmtpClient client;

    string feedback_email;
    string feedback_password;

	void Start()
	{
        Instance = this;

        feedback_email = "djd.feedback@gmail.com";
        feedback_password = "fuckyoudontstealmypassword";

        client = new SmtpClient();
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.EnableSsl = true;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = (ICredentialsByHost)new NetworkCredential(feedback_email, feedback_password);
        client.Timeout = 5000;

        client.SendCompleted += OnMessageSent;
    }
	
	void Update()
	{

	}

    public void OpenFeedback()
    {
        PlayerScript.MyPlayer.UIOpen = true;

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    public void CloseFeedback()
    {
        PlayerScript.MyPlayer.UIOpen = false;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    public void SendFeedback()
    {
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

        MailMessage message = new MailMessage();
        message.From = new MailAddress(feedback_email);
        message.To.Add(new MailAddress("davyjonesdevelopment@gmail.com"));
        message.Subject = string.Format("[{0}]-{1}", FeedbackTypeSelection.captionText.text, FeedbackSubjectInput.text);
        message.Body = FeedbackInput.text;

        client.SendAsync(message, "feedbacksent");

        message.Dispose();

        FeedbackSubjectInput.text = string.Empty;
        FeedbackInput.text = string.Empty;
    }

    public void OnMessageSent(object sender, AsyncCompletedEventArgs e)
    {
        DialogueBox message_success = Instantiate(MessageSuccessPanel).GetComponent<DialogueBox>();
        message_success.NewDialogue("Feedback Sent!", 2.0f);
    }
}
