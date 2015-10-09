using RestSharp;
using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace OneTapCheckinBackgroundTask
{
    public sealed class LocationTask : IBackgroundTask
    {
        static string TaskName = "OneTapCheckinLocationTask";
        private string ClientId = "PT5MAKB0JYPZ0MNBETTAUOLRYNAE0HNM0JCUXQJQNWYXJBEO";
        private string ClientSecret = "NZP1MHSW5W2C1YBXARQRM3GIAMCN2V2254C3DMKLJPQRDYYQ";
        private static string AccessToken { get; set; }
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            // Uncomment this to utilize async/await in the task
            var deferral = taskInstance.GetDeferral();


            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (InternetConnectionProfile == null)
            {
                //NotifyUser("Not connected to Internet\n");
            }
            else
            {



            AccessToken = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["AccessToken"];




            // simple example with a Toast, to enable this go to manifest file
            // and mark App as TastCapable - it won't work without this
            // The Task will start but there will be no Toast.
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList textElements = toastXml.GetElementsByTagName("text");
            textElements[0].AppendChild(toastXml.CreateTextNode("My first Task - Yeah"));
            textElements[1].AppendChild(toastXml.CreateTextNode("I'm a message from your background task!"));
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));




            
/*
            String fsClient = ClientId;
            String fssecret = ClientSecret;

            var client = new RestClient("https://api.foursquare.com/");


            var request = new RestRequest("v2/checkins/add", Method.POST);

            client.Authenticator = new HttpBasicAuthenticator(fsClient, ClientSecret);
            request.AddParameter("client_id", fsClient, ParameterType.GetOrPost);
            request.AddParameter("client_secret", fssecret, ParameterType.GetOrPost);
            request.AddParameter("v", "20140806", ParameterType.GetOrPost);
            request.AddParameter("m", "swarm", ParameterType.GetOrPost);
            request.AddParameter("oauth_token", AccessToken, ParameterType.GetOrPost);
            request.AddParameter("venueId", report.Geofence.Id.ToString(), ParameterType.GetOrPost);


            client.ExecuteAsync(request, response =>
            {

                // Create a toast notification to show a geofence has been hit
                var toastXmlContent2 = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                var txtNodes2 = toastXmlContent2.GetElementsByTagName("text");
                txtNodes2[0].AppendChild(toastXmlContent2.CreateTextNode("Checkin Created!"));
                txtNodes2[1].AppendChild(toastXmlContent2.CreateTextNode(report.Geofence.Id));

                var toast2 = new ToastNotification(toastXmlContent2);
                var toastNotifier2 = ToastNotificationManager.CreateToastNotifier();
                toastNotifier2.Show(toast2);

            });

*/
            





            }
            // Uncomment this to utilize async/await in the task
            deferral.Complete();

        }

        public async static void Register()
        {


            string myTaskName = TaskName;

            // check if task is already registered
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == myTaskName)
                {
                    await (new MessageDialog("Task already registered")).ShowAsync();
                    return;
                }
       /*     await (new MessageDialog("1Task registered")).ShowAsync();

            // Windows Phone app must call this to use trigger types (see MSDN)
            await BackgroundExecutionManager.RequestAccessAsync();
            await (new MessageDialog("T2ask registered")).ShowAsync();

            // register a new task
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder { Name = myTaskName, TaskEntryPoint = "BackgroundTask.LocationTask" };
            await (new MessageDialog("Ta3sk registered")).ShowAsync();
            taskBuilder.SetTrigger(new TimeTrigger(15, true));
            await (new MessageDialog("Tas4k registered")).ShowAsync();
            taskBuilder.Register();

            await (new MessageDialog("Task registered")).ShowAsync();
            */



            
                var result = await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();

                builder.Name = TaskName;
                builder.TaskEntryPoint = "BackgroundTask.LocationTask";
                //builder.SetTrigger(new TimeTrigger(30, false));
                builder.SetTrigger(new LocationTrigger(LocationTriggerType.Geofence));


                //SystemCondition condition = new SystemCondition(SystemConditionType.InternetAvailable);
                //builder.AddCondition(condition);

                builder.Register();


                await new MessageDialog("Task registered!").ShowAsync();




            /*
            if (!IsTaskRegistered())
            {

                var result = await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();

                builder.Name = TaskName;
                builder.TaskEntryPoint = "BackgroundTask.LocationTask";
                builder.SetTrigger(new LocationTrigger(LocationTriggerType.Geofence));


                //SystemCondition condition = new SystemCondition(SystemConditionType.InternetAvailable);
                //builder.AddCondition(condition);

                builder.Register();


                await new MessageDialog("Task registered!").ShowAsync();


            }
            else
            {
            }
             */
        }

        public static void Unregister()
        {


            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(kvp => kvp.Value.Name == TaskName);

            if (entry.Value != null)
                entry.Value.Unregister(true);

        }

        public static bool IsTaskRegistered()
        {
            var taskRegistered = false;
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(kvp => kvp.Value.Name == TaskName);

            if (entry.Value != null)
                taskRegistered = true;

            return taskRegistered;
        }
    }
}
