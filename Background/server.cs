﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace Background
{
    public sealed class server
    {
        private IPAddress ip;
        private string port;
        private string name;
        private string map;
        private string game;
        private string players_maxplayers;
        private Byte players;
        private Byte maxplayers;
        private string player_list;
        private static int client_port = 25001;
        private static int max_pings = 500;
        private UdpClient udp;

        public server(string ip, string port)
        {
            this.Ip = ip;
            this.Port = port;
        }
        public server()
        {
            ;
        }
        public string Ip
        {
            get
            {
                return this.ip.ToString();
            }
            set
            {
                this.ip = IPAddress.Parse(value);
            }
        }
        public string Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = value;
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    
                }
            }
        }
        public string Map
        {
            get { return map; }
            set
            {
                if (map != value)
                {
                    map = value;
                    
                }
            }
        }
        public string Game
        {
            get { return game; }
            set
            {
                if (game != value)
                {
                    game = value;
                    
                }
            }
        }
        public string Players_maxplayers
        {
            get { return players_maxplayers; }
            set
            {
                if (players_maxplayers != value)
                {
                    players_maxplayers = value;
                }
            }
        }
        
        public string Player_list
        {
            get { return player_list; }
            set
            {
                if (player_list != value)
                {
                    player_list = value;

                }
            }
        }
        public void recv_data()
        {
            //Process.WaitForExit(recv_data);
        }
        public void connect_server()
        {
            try
            {
                udp = new UdpClient(client_port);
                udp.Connect(ip, int.Parse(Port));
                udp.Client.ReceiveTimeout = max_pings;
                Byte[] Request_INFO = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };
                udp.Send(Request_INFO, Request_INFO.Length);
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(ip, 0);
                Byte[] Request_response;
                Request_response = udp.Receive(ref RemoteIpEndPoint);
                info_update(Request_response);
                udp.Close();
            }
            catch (System.Net.Sockets.SocketException)
            {
                udp.Close();
                this.Game = "超时！";
                this.Name = "";
                this.Map = "";
                this.players = 0;
                this.maxplayers = 0;
                this.players_maxplayers = "";
            }
        }
        private void info_update(Byte[] info)
        {
            Byte[] name = new Byte[info.Length];
            int i = 6;
            int j = 0;
            while (info[i] != 0x00)
            {
                name[j] = info[i];
                i++;
                j++;
            }
            j = 0;
            i++;
            this.Name = Encoding.UTF8.GetString(name);
            Byte[] map = new Byte[info.Length - i];
            while (info[i] != 0x00)
            {
                map[j] = info[i];
                i++;
                j++;
            }
            j = 0;
            i++;
            this.Map = Encoding.ASCII.GetString(map);
            Byte[] game = new Byte[info.Length - i];
            while (info[i] != 0x00)
            {
                i++;
            }
            i++;
            while (info[i] != 0x00)
            {
                game[j] = info[i];
                i++;
                j++;
            }
            this.Game = Encoding.UTF8.GetString(game);
            i = i + 3;
            this.players = info[i];
            i++;
            this.maxplayers = info[i];
            this.Players_maxplayers = this.players.ToString() + "/" + this.maxplayers.ToString();
        }
        public void update_player_list()
        {
            player_list = "";
            Byte[] Request_response = send_playerlist_udp();
            if (Request_response.Length != 0)
                player_list_update(Request_response);
            else
                player_list = "";
        }
        private void player_list_update(Byte[] info)
        {
            int i = 7; int j = 0; int k = 0;
            k = info[5];
            while (k != 0)
            {
                Byte[] name = new byte[info.Length];
                while (info[i] != 0x00)
                {
                    name[j] = info[i];
                    j++; i++;
                }
                player_list += (Encoding.UTF8.GetString(name)).TrimEnd('\0') + "、";
                i = i + 10; j = 0;
                k--;
            }
        }
        private  Byte[] send_playerlist_udp()
        {
            Byte[] Request_response = new Byte[0];
            try
            {
                udp = new UdpClient(client_port);
                udp.Connect(ip, int.Parse(port));
                udp.Client.ReceiveTimeout = max_pings;
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(ip, 0);
                Byte[] Request_INFO = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF };
                udp.Send(Request_INFO, Request_INFO.Length);
                Request_response = udp.Receive(ref RemoteIpEndPoint);
                Request_INFO = update_challenge_number(Request_response);
                udp.Send(Request_INFO, Request_INFO.Length);    //请求玩家名称
                Request_response = udp.Receive(ref RemoteIpEndPoint);
                udp.Close();
            }
            catch (System.Net.Sockets.SocketException)
            {
                udp.Close();
                Request_response = new Byte[0];
            }
            return Request_response;
        }
        private byte[] update_challenge_number(byte[] received_info)
        {
            byte[] new_request_info = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0x00, 0x00, 0x00, 0x00 };
            for (int i = 5; i < 9; i++)
            {
                new_request_info[i] = received_info[i];
            }
            return new_request_info;
        }
    }
}
