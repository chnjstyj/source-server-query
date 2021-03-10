using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace 起源服务器查询
{
    class tile_content
    {
        public static TileNotification update_info(string game, string name, string map, string players,string player_list)
        {

            // Construct the tile content
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
                                                    HintWrap = false,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = name,
                                                    HintWrap = false,
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
                                                    HintWrap = false,
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
                                                    HintWrap = false,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = name,
                                                    HintWrap = false,
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
                                                    HintWrap = false,
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
                                                    HintWrap = true,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = players,
                                                    HintWrap = true,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = player_list,
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
        public static TileNotification update_players()
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
        public static TileNotification update_players1()
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
    }
}
