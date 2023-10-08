using Fiddler;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace bloodhunt_queue_sniper
{
    public class Listener
    {
        private static readonly string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void StartFiddlerCore()
        {
            FiddlerCoreStartupSettings startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .RegisterAsSystemProxy()
                .DecryptSSL()
                .MonitorAllConnections()
                .Build();

            FiddlerApplication.Startup(startupSettings);
        }

        public static void AttachEventListeners()
        {
            FiddlerApplication.BeforeRequest += BeforeRequest;
        }

        private static async void BeforeRequest(Session session)
        {
            session.bBufferResponse = true;
            if (session.uriContains("https://gw.release.live.r1.g.sharkmob.cloud"))
            {
                if (session.url.Equals("gw.release.live.r1.g.sharkmob.cloud/session/sessions/placements"))
                {
                    JObject requestObject = JObject.Parse(session.GetRequestBodyAsString());
                    if (requestObject["gAMEMODEId"].ToString().ToLower().Contains("elysium"))
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(Main.PartyKey) && Main.PartyKey != "null")
                    {
                        string partyResponseAsString = await Main.Instane.Worker.CheckPartyKeyAsync(Main.PartyKey);

                        if (partyResponseAsString == "null")
                        {
                            MessageBox.Show("Looks like the Party you were in ended!");
                            Util.EnableAllPartyManagingKeys();
                            Main.PartyKey = "";
                            return;
                        }

                        JObject partyResponse = JObject.Parse(partyResponseAsString);

                        if (partyResponse["leader"].ToString() == Util.GetHardwareID())
                        {
                            string gameMode = requestObject.ContainsKey("gAMEMODEId") ?
                            (requestObject["gAMEMODEId"].ToString().ToLower().Contains("trio") ? "Trios" :
                            requestObject["gAMEMODEId"].ToString().ToLower().Contains("duo") ? "Duos" :
                            requestObject["gAMEMODEId"].ToString().ToLower().Contains("main") ? "Solo" :
                            requestObject["gAMEMODEId"].ToString()) : "null";

                            await Main.Instane.Worker.QueuePartyAsync(Main.PartyKey, gameMode);
                        }
                    }
                }
                else if (session.url.Equals("gw.release.live.r1.g.sharkmob.cloud/session/sessions/placements/cancel"))
                {
                    JObject requestObject = JObject.Parse(session.GetRequestBodyAsString());
                    if (requestObject["gAMEMODEId"].ToString().ToLower().Contains("elysium"))
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(Main.PartyKey) && Main.PartyKey != "null")
                    {
                        string partyResponseAsString = await Main.Instane.Worker.CheckPartyKeyAsync(Main.PartyKey);

                        if (partyResponseAsString == "null")
                        {
                            MessageBox.Show("Looks like the Party you were in ended!");
                            Util.EnableAllPartyManagingKeys();
                            Main.PartyKey = "";
                            return;
                        }

                        JObject partyResponse = JObject.Parse(partyResponseAsString);

                        if (partyResponse["leader"].ToString() == Util.GetHardwareID())
                        {
                            string gameMode = requestObject.ContainsKey("gAMEMODEId") ?
                            (requestObject["gAMEMODEId"].ToString().ToLower().Contains("trio") ? "Trios" :
                            requestObject["gAMEMODEId"].ToString().ToLower().Contains("duo") ? "Duos" :
                            requestObject["gAMEMODEId"].ToString().ToLower().Contains("main") ? "Solo" :
                            requestObject["gAMEMODEId"].ToString()) : "null";

                            await Main.Instane.Worker.QueuePartyAsync(Main.PartyKey, gameMode);
                        }
                    }
                }
            }
        }

        public static void ClearProxy()
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            const string keyName = userRoot + "\\" + subkey;

            int proxyEnable = (int)Registry.GetValue(keyName, "ProxyEnable", 0);

            if (proxyEnable == 1)
            {
                Registry.SetValue(keyName, "ProxyEnable", 0, RegistryValueKind.DWord);
            }
        }

        public static void StopCore()
        {
            ClearProxy();

            if (FiddlerApplication.IsStarted())
            {
                FiddlerApplication.Shutdown();
            }
        }

        public static void EnsureRootCertificate()
        {
            try
            {
                BCCertMaker.BCCertMaker certProvider = new BCCertMaker.BCCertMaker();
                CertMaker.oCertProvider = certProvider;

                string rootCertificatePath = Path.Combine(assemblyDirectory, "..", "..", "RootCertificate.p12");
                string rootCertificatePassword = "S0m3T0pS3cr3tP4ssw0rd";
                if (!File.Exists(rootCertificatePath))
                {
                    certProvider.CreateRootCertificate();
                    certProvider.WriteRootCertificateAndPrivateKeyToPkcs12File(rootCertificatePath, rootCertificatePassword);
                }
                else
                {
                    certProvider.ReadRootCertificateAndPrivateKeyFromPkcs12File(rootCertificatePath, rootCertificatePassword);
                }

                if (!CertMaker.rootCertIsTrusted())
                {
                    CertMaker.trustRootCert();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fiddler Error: EnsureRootCertificate", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
