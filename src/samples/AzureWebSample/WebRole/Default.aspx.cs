using System;
using System.IO;
using Orleans.Host.Azure.Client;
using Orleans.Host.Azure.Utils;
using HelloWorldInterfaces;

namespace Orleans.Azure.Samples.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (!OrleansAzureClient.IsInitialized)
                {
                    FileInfo clientConfigFile = AzureConfigUtils.ClientConfigFileLocation;
                    if (!clientConfigFile.Exists)
                    {
                        throw new FileNotFoundException(string.Format("Cannot find Orleans client config file for initialization at {0}", clientConfigFile.FullName), clientConfigFile.FullName);
                    }
                    OrleansAzureClient.Initialize(clientConfigFile);
                }
            }
        }

        protected async void ButtonSayHello_Click(object sender, EventArgs e)
        {
            this.ReplyText.Text = "Talking to Orleans";

            IHello grainRef = HelloFactory.GetGrain(0);

            try
            {
                string msg = await grainRef.SayHello(this.NameTextBox.Text);
                this.ReplyText.Text = "Orleans said: " + msg + " at " + DateTime.UtcNow + " UTC";
            }
            catch (Exception exc)
            {
                while (exc is AggregateException) exc = exc.InnerException;

                this.ReplyText.Text = "Error connecting to Orleans: " + exc + " at " + DateTime.Now;
            }
        }
    }
}
