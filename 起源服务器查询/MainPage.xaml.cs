using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Realms;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Core;
using System.Threading;
// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 起源服务器查询
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static server _storedItem;
        Windows.Storage.ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public MainPage()
        {
            this.InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            Window.Current.SetTitleBar(realTileBar);
            
            //data_trans.load_();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView()
                .PrepareToAnimate("forwardAnimation", main);
            // You don't need to explicitly set the Configuration property because
            // the recommended Gravity configuration is default.
            // For custom animation, use:
            // animation.Configuration = new BasicConnectedAnimationConfiguration();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            //data_trans.server_list.Clear();
            
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.RegisterBackgroundTask();

            servers.ItemsSource = data_trans.server_list;
            foreach (server server in data_trans.server_list)
            {
                await server.connect_server();   //这里要等待每个udp完成连接 否则每个list中的server都会同时占据一个udp端口
            }
            if (e.Parameter is List<string>)
            {
                List<string> info = (e.Parameter as List<string>);
                if (info.Count != 0)
                {
                    server new_server = new server();
                    new_server.ip = IPAddress.Parse(info[0]);
                    new_server.port = info[1];
                    new_server.connect_server();
                    //重复的不添加
                    if (data_trans.server_list.Any(s => s.Ip == new_server.Ip && s.Port == new_server.Port) == false)
                        data_trans.server_list.Add(new_server);
                    ip.Text = "";
                }
            }
        }

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }

        private const string taskName = "server_qurey";
        private const string taskEntryPoint = "Background.server_qurey";
        
        private void add_server_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(query_server_list),ip.Text, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void fresh_Click(object sender, RoutedEventArgs e)
        {
            fresh.IsEnabled = false;
            refresh_server();
            fresh.IsEnabled = true;
        }

        private async static void refresh_server()
        {
            foreach (server server in data_trans.server_list)
            {
                await server.connect_server();
                await server.update_player_list();
                string game = server.Game.TrimEnd('\0');
                string name = server.Name.TrimEnd('\0');
                string map = server.Map.TrimEnd('\0');
                string players = server.Players_maxplayers.TrimEnd('\0');
                string tileId = server.Ip + server.Port;
                string player_list = "";
                foreach (var player_name in server.Player_list)
                {
                    player_list += player_name.TrimEnd('\0') + "、";
                }
                player_list.TrimEnd('、');
                var tileNotif = tile_content.update_info(game, name, map, players, player_list.TrimEnd('、'));
                tileNotif.Tag = "info";
                if (SecondaryTile.Exists(tileId))
                {
                    // Get its updater
                    var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);
                    updater.EnableNotificationQueue(true);

                    // And send the notification
                    updater.Update(tileNotif);
                }
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (servers.SelectedItem != null)
                data_trans.server_list.Remove(
                (servers.SelectedItem as server));
        }

        private void client_port_TextChanged(object sender, TextChangedEventArgs e)
        {
            server.client_port = Int32.Parse(client_port.Text);
            localSettings.Values["user_port_setting"] = client_port.Text;
        }

        private void server_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem connectItem = new MenuFlyoutItem { Text = "连接" };
            MenuFlyoutItem deleteItem = new MenuFlyoutItem { Text = "删除" };
            MenuFlyoutItem detialsItem = new MenuFlyoutItem { Text = "详情" };
            MenuFlyoutSeparator separator = new MenuFlyoutSeparator();
            MenuFlyoutItem pinItem = new MenuFlyoutItem { Text = "固定到开始菜单" };
            connectItem.Click += connectItem_Click;
            pinItem.Click += pinItem_Click;
            deleteItem.Click += deleteItem_Click;
            detialsItem.Click += detialsItem_Click;
            myFlyout.Items.Add(connectItem);
            myFlyout.Items.Add(deleteItem);
            myFlyout.Items.Add(detialsItem);
            myFlyout.Items.Add(separator);
            myFlyout.Items.Add(pinItem);

            //if you only want to show in left or buttom 
            //myFlyout.Placement = FlyoutPlacementMode.Left;

            FrameworkElement senderElement = sender as FrameworkElement;

            //the code can show the flyout in your mouse click 
            myFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private async void detialsItem_Click(object sender, RoutedEventArgs e)
        {
            if (servers.SelectedItem != null)
            {
                ConnectedAnimation animation = null;
                _storedItem = servers.SelectedItem as server;
                await _storedItem.update_player_list();    //
                game_selected.Text = _storedItem.Game.TrimEnd('\0');
                name_selected.Text = _storedItem.Name.TrimEnd('\0');
                map_selected.Text = _storedItem.Map.TrimEnd('\0');
                players_selected.Text = "Players:" + _storedItem.Players_maxplayers.TrimEnd('\0');
                List<string> Player_list = new List<string>();
                Player_list = _storedItem.Player_list.Select(x => x.TrimEnd('\0')).ToList<string>();
                player_list.ItemsSource = _storedItem.Player_list;
                animation = servers.PrepareConnectedAnimation("forwardAnimation", _storedItem, "connectedElement");

                SmokeGrid.Visibility = Visibility.Visible;

                animation.TryStart(destinationElement);
            }
        }

        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (servers.SelectedItem != null)
                data_trans.server_list.Remove(
                (servers.SelectedItem as server));
        }

        private async void pinItem_Click(object sender, RoutedEventArgs e)
        {
            if (servers.SelectedItem != null)
            {
                var server = servers.SelectedItem as server;
                await server.update_player_list();
                string game = server.Game.TrimEnd('\0');
                string name = server.Name.TrimEnd('\0');
                string map = server.Map.TrimEnd('\0');
                string players = server.Players_maxplayers.TrimEnd('\0');
                string tileId = server.Ip + server.Port;
                string displayName = server.Ip + ":" + server.Port;
                string arguments = server.Ip + ":" + server.Port;
                string player_list = " ";
                foreach (var player_name in server.Player_list)
                {
                    player_list += player_name.TrimEnd('\0') + "、";
                }
                SecondaryTile tile = new SecondaryTile(
                tileId,
                displayName,
                arguments,
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-100.png"),
                Windows.UI.StartScreen.TileSize.Default
                );
                tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-100.png");
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/LargeTile.scale-100.png");
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                bool fin = await tile.RequestCreateAsync();
                var tileNotif = tile_content.update_info(game, name, map, players,player_list.TrimEnd('、'));
                tileNotif.Tag = "info";
                if (SecondaryTile.Exists(tileId))
                {
                    // Get its updater
                    var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);
                    updater.EnableNotificationQueue(true);

                    // And send the notification
                    updater.Update(tileNotif);
                }

            }
        }
        private void connectItem_Click(object sender, RoutedEventArgs e)
        {
            if (servers.SelectedItem != null)
            {
                var server = servers.SelectedItem as server;
                string server_ip_port = server.Ip + ":" + server.Port;
                connectAsync("steam://connect/" + server_ip_port);
            }
        }
        private async void connectAsync(string path)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(path));
        }

        private async void DisplaysaveDialog()
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Content = "保存成功！",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();
        }

        private void max_pings_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (max_pings.Text != "") server.max_pings = Int32.Parse(max_pings.Text);
            else server.max_pings = 500;
        }

        private async void back_to_main_Click(object sender, RoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", destinationElement);

            // Collapse the smoke when the animation completes.
            animation.Completed += Animation_Completed;

            // If the connected item appears outside the viewport, scroll it into view.
            servers.ScrollIntoView(_storedItem, ScrollIntoViewAlignment.Default);
            servers.UpdateLayout();

            // Use the Direct configuration to go back (if the API is available). 
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }

            // Play the second connected animation. 
            await servers.TryStartConnectedAnimationAsync(animation, _storedItem, "connectedElement");
        }
        private void Animation_Completed(ConnectedAnimation sender, object args)
        {
            SmokeGrid.Visibility = Visibility.Collapsed;
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            string server_ip = _storedItem.Ip + ":" + _storedItem.Port;
            connectAsync("steam://connect/" + server_ip);
        }

        private void save_list_Click(object sender, RoutedEventArgs e)
        {
            data_trans.save_();
            DisplaysaveDialog();
        }
        /*
        private void add_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(query_server_list));
        }*/
    }
    public class data_trans
    {
        public static ObservableCollection<server> server_list = new ObservableCollection<server>();
        public static void save_()
        {
            var realmDB = Realm.GetInstance("server_list.realm");
            var servers = realmDB.All<server_saved>().ToList();
            int maxserverId = 0;
            if (servers.Count != 0)
            {
                realmDB.Write(() =>
                {
                    realmDB.RemoveAll();
                });
            }
            foreach (server server in server_list)
            {
                server_saved wserver = new server_saved();
                wserver.Id = maxserverId;
                wserver.Ip = server.Ip;
                wserver.Port = server.Port;
                realmDB.Write(() =>
                {
                    realmDB.Add(wserver);
                });
                maxserverId++;
            }
        }
        public async static void load_()
        {
            await Task.Run(() =>
            {
                Realm realmDB;
                realmDB = Realm.GetInstance("server_list.realm");
                var serverList = realmDB.All<server_saved>().ToList();
                foreach (server_saved server_saved in serverList)
                {
                    server new_server = new server();
                    new_server.Ip = server_saved.Ip;
                    new_server.Port = server_saved.Port;
                    server_list.Add(new_server);
                }
            });
            /*
            Realm realmDB;
            realmDB = Realm.GetInstance("server_list.realm");
            var serverList = realmDB.All<server_saved>().ToList();
            foreach (server_saved server_saved in serverList)
            {
                server new_server = new server();
                new_server.Ip = server_saved.Ip;
                new_server.Port = server_saved.Port;
                server_list.Add(new_server);
            }*/
        }
    }
}


