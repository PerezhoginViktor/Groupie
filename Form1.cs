using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SplitGroupie
{
    public partial class Form1 : Form
    {
        private BetterRandom random = new BetterRandom();
        private DiscordWebhook discordWebhook = new DiscordWebhook();
        private Defaults _Def = new Defaults();
        private WebClient _WC = new WebClient();
        private int rechecks = 0;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            discordWebhook.WebHook = "your discord webhook here";
            backgroundWorker1.RunWorkerAsync();
        }

        public string GetProxy()
        {
            /*MatchCollection _MC = _Def._REGEX.Matches(TB_ScrapeInput.Text);
            foreach (Match Match in _MC)
            {
                MessageBox.Show(Match.ToString());
                return Match.ToString();
            }
            return null;*/
            try
            {
                foreach (string PageToScrape in _Def.sources)
                {
                    string PageSource = _WC.DownloadString(PageToScrape);

                    MatchCollection _MC = _Def._REGEX.Matches(PageSource);
                    foreach (Match Match in _MC)
                    {
                        return Match.ToString();
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public GroupData GetGroupData(int groupId)
        {
            GroupData gData = new GroupData();
            try
            {
                gData.groupId = groupId;

                string groupUrl = string.Format("https://groups.roblox.com/v1/groups/{0}", groupId);
                string rbxUrl = string.Format("https://economy.roblox.com/v1/groups/{0}/currency", groupId);

                string robuxData = string.Empty;

                // Webclient
                WebClient client = new WebClient();
                /*string sProxy = string.Empty;
                while (true)
                {
                    sProxy = GetProxy();
                    if (sProxy == string.Empty)
                    {
                        continue;
                    }
                    break;
                }
                WebProxy proxy = new WebProxy(sProxy);
                client.Proxy = proxy;*/
                client.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
                string ownershipData = client.DownloadString(groupUrl);
                client.Dispose();

                /*if (!robuxData.Contains("{\"robux\":0}"))
                {
                    int temp = 0;
                    gData.gotRobux = true;
                    //gData.robux = int.TryParse(Regex.Match(robuxData, @"\d+").Value, out int temp);
                }-*/

                if (ownershipData.Contains("\"publicEntryAllowed\":true"))
                {
                    gData.canJoin = true;
                }

                if (ownershipData.Contains("\"owner\":null"))
                {
                    gData.hasOwner = false;
                }
                richTextBox1.Text = "Returning " + gData.groupId + '\n' + richTextBox1.Text;
                return gData;
            }
            catch
            {
                gData.failed = true;
                return gData;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            GroupData groupData = new GroupData();
            while (true)
            {
                if (rechecks >= 1)
                {
                    groupData.failed = false;
                }

                if (richTextBox1.Text.Length > 1000)
                {
                    richTextBox1.Text = string.Empty;
                }

                if (groupData.failed)
                {
                    richTextBox1.Text = "Rechecking " + groupData.groupId + '\n' + richTextBox1.Text;
                    rechecks += 1;
                    groupData = GetGroupData(groupData.groupId);
                }
                else
                {
                    rechecks = 0;
                    groupData = GetGroupData(random.NextInteger(100, 9097478));
                }
                
                if (groupData.hasOwner == false && groupData.failed == false && groupData.canJoin == true)
                {
                    discordWebhook.SendMessage(groupData.groupId.ToString());
                }
            }
        }
    }

    public class GroupData
    {
        public int groupId = 0;
        public bool canJoin = false;
        public bool hasOwner = true;
        public bool gotRobux = false;
        public int robux = 0;
        public bool failed = false;
    }

    public class BetterRandom
    {
        private Random random = new Random();
        private List<int> usedRandoms = new List<int>();
        public int NextInteger(int min, int max)
        {
            int value;
            while (true)
            {
                value = random.Next(min, max);
                if (!usedRandoms.Contains(value))
                {
                    break;
                }
                Thread.Sleep(1);
            }
            return value;
        }
    }

    public class CustomTimer
    {
        private int time = 0;

        public void Do(int ms)
        {
            while (time < ms)
            {
                Application.DoEvents();
                time++;
            }
        }
    }
}
