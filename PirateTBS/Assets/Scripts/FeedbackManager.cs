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

    public Dropdown FeedbackTypeSelection;          //Reference to the feedback type selection
    public InputField FeedbackSubjectInput;         //Reference to the feedback subject box
    public InputField FeedbackInput;                //Reference to the feedback input box
    public DialogueBox MessageSuccessPanel;         //Reference to dialogue box showing a success message

    SmtpClient client;                              //Email client
    string feedback_email;                          //Email address of client
    string feedback_password;                       //Email password of client

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

    /// <summary>
    /// Open feedback panel
    /// </summary>
    public void OpenFeedback()
    {
        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Close feedback panel
    /// </summary>
    public void CloseFeedback()
    {
        PlayerScript.MyPlayer.OpenUI = null;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Send player feedback to devs
    /// </summary>
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

    /// <summary>
    /// Callback for message sent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMessageSent(object sender, AsyncCompletedEventArgs e)
    {
        DialogueBox message_success = Instantiate(MessageSuccessPanel).GetComponent<DialogueBox>();
        message_success.NewDialogue("Feedback Sent!", 2.0f);
    }
}
