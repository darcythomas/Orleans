using Microsoft.AspNet.SignalR;
using GPSTracker.Common;

namespace GPSTracker.Web.Controllers
{
    public class LocationHub : Hub
    {
        public void LocationUpdate(VelocityMessage message)
        {
            Clients.Group("BROWSERS").locationUpdate(message);
        }

        public void LocationUpdates(VelocityBatch messages)
        {
            Clients.Group("BROWSERS").locationUpdates(messages);
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            if (Context.Headers.Get("ORLEANS") != "GRAIN")
            {
                Groups.Add(Context.ConnectionId, "BROWSERS");
            }
            return base.OnConnected();
        }


    }
}