using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 起源服务器查询
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary> 
    public sealed partial class query_server_list : Page
    {
        string ip;
        string port;
        string server_query_http;
        const string steam_web_api = "http://api.steampowered.com/ISteamApps/GetServersAtAddress/v0001?addr=";
        List<show_server> servers = new List<show_server>();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                ip = e.Parameter.ToString();
                server_query_http = steam_web_api + e.Parameter.ToString();
            }
            else
            {
                server_query_http = steam_web_api + "127.0.0.1";
            }
            
            var query_result = await GetServers(server_query_http);

            server_list.ItemsSource = query_result.response.servers;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            Window.Current.SetTitleBar(realTileBar);
        }

        public static async Task<query_server.root_message> GetServers(string server_query_http)
        {
            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            var response = await httpClient.GetAsync(new Uri(server_query_http));
            var jsonMessage = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(query_server.root_message));
            var message = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonMessage));
            var result = serializer.ReadObject(message) as query_server.root_message;
            return result;
        }

        public query_server_list()
        {
            this.InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            Window.Current.SetTitleBar(realTileBar);
        }

        private void back_to_main_Click(object sender, RoutedEventArgs e)
        {
            TryGoBack();
        }

        private static bool TryGoBack()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                return true;
            }
            return false;
        }

        private void server_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //game.Text += server_list.SelectedItem.GetType().ToString();
            var server = (server_list.SelectedItem as query_server.Server);
            int port_index = server.addr.IndexOf(':');
            port = server.addr.Remove(0,port_index+1);
            server select_server = new server(ip,port);
            if (select_server.connect_server())
            {
                game.Text = select_server.Game;
                name.Text = select_server.Name;
                map.Text = select_server.Map;
                players.Text = select_server.Players_maxplayers;
                add_button.Visibility = Visibility.Visible;
            }
            else
            {
                game.Text = "连接失败";
                name.Text = "";
                map.Text = "";
                players.Text = "";
                add_button.Visibility = Visibility.Collapsed;
            }
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            List<string> info = new List<string>();  //0: ip  1: port
            info.Add(ip);
            info.Add(port);
            this.Frame.Navigate(typeof(MainPage), info);
        }
    }
    class show_server
    {
        private string gamedir;
        private string addr;
        private int gameport;
        public string Gamedir
        {
            get
            {
                return this.gamedir;
            }
            set
            {
                this.gamedir = value;
            }
        }
        public string Addr
        {
            get
            {
                return this.addr;
            }
            set
            {
                this.addr = value;
            }
        }
        public int Gameport
        {
            get
            {
                return this.gameport;
            }
            set
            {
                this.gameport = value;
            }
        }
    }
}
