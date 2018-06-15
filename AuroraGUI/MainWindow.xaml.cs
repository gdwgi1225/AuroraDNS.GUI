﻿using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows;
using ARSoft.Tools.Net.Dns;

namespace AuroraGUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public static DnsServer MyDnsServer;
        public static IPAddress MyIPAddr;
        public static IPAddress LocIPAddr;

        public MainWindow()
        {
            InitializeComponent();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            LocIPAddr = IPAddress.Parse(IpTools.GetLocIp());
            if (Thread.CurrentThread.CurrentCulture.Name == "zh-CN")
                MyIPAddr = IPAddress.Parse(new WebClient().DownloadString("http://members.3322.org/dyndns/getip").Trim());
            else
                MyIPAddr = IPAddress.Parse(new WebClient().DownloadString("https://api.ipify.org").Trim());

            MyDnsServer = new DnsServer(DnsSettings.ListenIp, 10, 10);
            MyDnsServer.QueryReceived += QueryResolve.ServerOnQueryReceived;

            WindowBlur.SetEnabled(this, true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 5;
            Top = desktopWorkingArea.Bottom - Height - 5;

            Topmost = true;
            DnsEnable.IsChecked = true;
        }

        private void IsGlobal_Checked(object sender, RoutedEventArgs e)
        {
            DnsSettings.ListenIp = IPAddress.Any;
            DnsEnable.IsChecked = false;
        }

        private void IsGlobal_Unchecked(object sender, RoutedEventArgs e)
        {
            DnsSettings.ListenIp = IPAddress.Loopback;
            DnsEnable.IsChecked = false;
        }

        private void IsSysDns_Checked(object sender, RoutedEventArgs e) => SysDnsSet.SetDns("127.0.0.1","1.0.0.1");

        private void IsSysDns_Unchecked(object sender, RoutedEventArgs e) => SysDnsSet.ResetDns();

        private void IsLog_Checked(object sender, RoutedEventArgs e) => DnsSettings.DebugLog = true;

        private void IsLog_Unchecked(object sender, RoutedEventArgs e) => DnsSettings.DebugLog = false;

        private void DnsEnable_Checked(object sender, RoutedEventArgs e) => MyDnsServer.Start();

        private void DnsEnable_Unchecked(object sender, RoutedEventArgs e) => MyDnsServer.Stop();
    }
}
