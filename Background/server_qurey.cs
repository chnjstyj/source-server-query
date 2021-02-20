using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Background
{
    public sealed class server_qurey : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            var tiles = await SecondaryTile.FindAllAsync();
            foreach (var tile in tiles)
            {
                string ip_port = tile.Arguments;
                int pos = ip_port.IndexOf(':');
                Char[] ip = new Char[ip_port.Length];
                Char[] port = new Char[ip_port.Length];
                ip_port.CopyTo(0, ip, 0, pos);
                ip_port.CopyTo(pos + 1, port, 0, ip_port.Length - (pos+1));
                string ip_str = new string(ip);
                string port_str = new string(port);
                ip_str = ip_str.TrimEnd('\0');
                port_str = port_str.TrimEnd('\0');
                server server = new server(ip_str, port_str);
                server.connect_server();
                if (server.Game != "超时！")
                {
                    var notifacation_info = update_info(server.Game.TrimEnd('\0'), server.Name.TrimEnd('\0'), server.Map.TrimEnd('\0'), server.Players_maxplayers.TrimEnd('\0'));
                    //var notifacation_players = update_players();
                    //var notifacation_players1 = update_players1();
                    notifacation_info.Tag = "info";
                    //notifacation_players.Tag = "players";
                    //notifacation_players1.Tag = "2";
                    var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
                    //updater.EnableNotificationQueue(true);
                    //updater.Update(notifacation_players1);
                    //updater.Update(notifacation_players);
                    updater.Update(notifacation_info);
                }
                else
                {
                    var notifacation_info = update_info("超时！", "", "", "");
                    //var notifacation_players = update_players();
                    //var notifacation_players1 = update_players1();
                    notifacation_info.Tag = "info";
                    //notifacation_players.Tag = "players";
                    //notifacation_players1.Tag = "2";
                    var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
                    //updater.EnableNotificationQueue(true);
                    //updater.Update(notifacation_players1);
                    //updater.Update(notifacation_players);
                    updater.Update(notifacation_info);
                }
            }
            deferral.Complete();
        }

        private TileNotification update_players()
        {
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Logo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "测试1"
                                }
                            }
                        }
                    }
                }
            };
            var tileNotif = new TileNotification(tileContent.GetXml());
            return tileNotif;
        }

        private TileNotification update_players1()
        {
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Logo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "测试2"
                                }
                            }
                        }
                    }
                }
            };
            var tileNotif = new TileNotification(tileContent.GetXml());
            return tileNotif;
        }

        private TileNotification update_info(string game,string name,string map,string players)
        {
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = game,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = name,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = map,
                                        HintWrap = false,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = players,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        }
                    },

                }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = game,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = name,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = map,
                                        HintWrap = false,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = players,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        }
                    }
                }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = game,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = name,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = map,
                                        HintWrap = false,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = players,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        }
                    }
                }
                        }
                    }
                }
            };


            // Create the tile notification
            var tileNotif = new TileNotification(tileContent.GetXml());
            return tileNotif;
        }
    }
}
