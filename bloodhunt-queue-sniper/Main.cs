using bloodhunt_queue_sniper.Properties;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace bloodhunt_queue_sniper
{
    public partial class Main : ApplicationContext
    {

        public static Main Instane;

        public NotifyIcon trayIcon { get; }

        public Worker Worker { get; } = new Worker();

        public Timer Timer { get; set; }

        public bool Queued { get; set; } = false;

        public static string PartyKey { get; set; } = "";

        public static string QueueKey = Util.ReadBloodhuntConfigToFindOutBinding("JoinPrague");

        public static string ChangeGameModekey = Util.ReadBloodhuntConfigToFindOutBinding("Elysium_ChangeGameMode");



        public Main()
        {
            if (QueueKey == "null")
            {
                MessageBox.Show("Couldn't find key JoinPrague in InputBindings.json");
                Application.Exit();
                return;
            }

            if (ChangeGameModekey == "null")
            {
                MessageBox.Show("Couldn't find key ChangeGameModekey in InputBindings.json");
                Application.Exit();
                return;
            }

            Instane = this;
            Bitmap BitMap = Resources.AppIcon;
            trayIcon = new NotifyIcon()
            {
                Text = "Bloodhunt Queue Sniper",
                Icon = Icon.FromHandle(BitMap.GetHicon()),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Party", new MenuItem[] {
                        new MenuItem("Create Party Key", CreatePartyKey) { Enabled = true },
                        new MenuItem("Join A Party", SetPartyKey) { Enabled = true },
                        new MenuItem("Leave Party", LeaveParty) { Enabled = false },
                    }),
                    new MenuItem("Developer", new MenuItem[] {
                        new MenuItem("Upload Logs", UploadLogs),
                    }),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
        }

        async void CreatePartyKey(object sender, EventArgs e)
        {
            PartyKey = await Worker.CreatePartyKeyAsync(Util.GetHardwareID());

            if (PartyKey == "null")
            {
                MessageBox.Show("This Service is currently down!");
                return;
            }

            Util.DisableAllPartyManagingKeys();
            Listener.AttachEventListeners();
            Listener.EnsureRootCertificate();
            Listener.StartFiddlerCore();
            Clipboard.SetText(PartyKey);
            MessageBox.Show("Your Party was created and the Code was copied to your clipboard!");
        }

        async void LeaveParty(object sender, EventArgs e)
        {
            string PartyResponseAsString = await Worker.CheckPartyKeyAsync(PartyKey);

            if (PartyResponseAsString != "null")
            {
                JObject PartyResponse = JObject.Parse(PartyResponseAsString);

                if (PartyResponse["leader"].ToString() == Util.GetHardwareID())
                {
                    DialogResult result = MessageBox.Show("Since you're the Party Leader the Party will end are you sure?", "Are you sure?", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        string response = await Worker.DeletePartyAsync(PartyKey);
                        if (response == "deleted")
                        {
                            Util.EnableAllPartyManagingKeys();
                            PartyKey = "";
                            MessageBox.Show("Deleted the Party!");
                        }
                        return;
                    }
                }
                else
                {
                    PartyKey = "";
                    MessageBox.Show("Left the Party");
                    Util.EnableAllPartyManagingKeys();
                    return;
                }
            }
            Util.EnableAllPartyManagingKeys();
            PartyKey = "";
        }

        async void SetPartyKey(object sender, EventArgs e)
        {
            string GivenPartyKey = Interaction.InputBox("", "Join a Party", "Party Key goes here");
            string PartyResponseAsString = await Worker.CheckPartyKeyAsync(GivenPartyKey);

            if (PartyResponseAsString == "null")
            {
                MessageBox.Show("This Party doesn't exist!");
                return;
            }

            JObject PartyResponse = JObject.Parse(PartyResponseAsString);

            if (PartyResponse["status"].ToString() == "NOT_QUEUED")
            {
                Util.DisableAllPartyManagingKeys();
                MessageBox.Show("You successfully joined the Party please stay active in the Bloodhunt Window to queue with the others, otherwise you might miss the train!");
                PartyKey = GivenPartyKey;
                Timer timer = new Timer(WaitForQueue, null, 0, 1000);
            }
            else
            {
                MessageBox.Show("The Party already started, you can't join now!");
            }
        }

        async void WaitForQueue(object state)
        {
            string PartyResponseAsString = await Worker.CheckPartyKeyAsync(PartyKey);
            if (PartyResponseAsString == "null")
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                MessageBox.Show("Looks like your Party just ended!");
                Util.EnableAllPartyManagingKeys();
                PartyKey = "";
                return;
            }
            else
            {
                JObject PartyResponse = JObject.Parse(PartyResponseAsString);
                if (PartyResponse["status"].ToString() == "NOT_QUEUED" && Queued)
                {
                    Queued = false;
                    Util.OpenBloodhuntAndPressDeQueueButton();
                }
                else if (PartyResponse["status"].ToString() == "QUEUED" && !Queued)
                {
                    Queued = true;
                    Util.OpenBloodhuntAndOpenGameModeWindowToQueue(PartyResponse["mode"].ToString());
                }
            }
        }

        void Exit(object sender, EventArgs e)
        {
            Listener.ClearProxy();
            Application.Exit();
        }

        void UploadLogs(object sender, EventArgs e)
        {
            MessageBox.Show("This function isn't done yet!");
        }
    }
}
