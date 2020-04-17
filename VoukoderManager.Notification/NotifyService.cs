using System;
using System.Windows;
using System.Windows.Forms;

namespace VoukoderManager.Notify
{
    public class NotifyService
    {
        protected static NotifyIcon Icon;
        protected static ContextMenu Menu;

        private static Window _window;
        private static Action _callback;

        public static bool ShowNotifications { get; set; }

        public static int BalloonTimeout { get; set; } = 5;

        static NotifyService()
        {
            Icon = new NotifyIcon();
            Menu = new ContextMenu();
            Icon.Icon = Resource.logo;
            Icon.Text = "VoukoderManager";
            Icon.Visible = false;
            Icon.DoubleClick += Icon_DoubleClick;
            Icon.BalloonTipClicked += Icon_BalloonTipClicked;
            Icon.BalloonTipClosed += Icon_BalloonTipClosed;
            CreateMenuItems();
        }

        private static void Icon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (_window.IsVisible)
                Icon.Visible = false;
        }

        public NotifyService(Window window)
        {
            _window = window;
        }

        private static void Icon_BalloonTipClicked(object sender, EventArgs e)
        {
            _callback?.Invoke();
        }

        private static void CreateMenuItems()
        {
            var itemOpen = new MenuItem();
            itemOpen.Click += ItemOpen_Click;
            itemOpen.Text = "Open";

            var itemExit = new MenuItem();
            itemExit.Click += ItemExit_Click;
            itemExit.Text = "Exit";
            Menu.MenuItems.Add(itemOpen);
            Menu.MenuItems.Add(itemExit);

            Icon.ContextMenu = Menu;
        }

        public static void Notify(INotification notification)
        {
            if (!ShowNotifications)
                return;
            Icon.Visible = true;
            Icon.ShowBalloonTip(BalloonTimeout, notification.Title, notification.Message, ToolTipIcon.None);
        }

        public static void Notify(INotification notification, Action callback)
        {
            _callback = callback;
            Notify(notification);
        }

        public void ShowIcon()
        {
            Icon.Visible = true;
        }

        public void HideIcon()
        {
            Icon.Visible = false;
        }

        private static void ItemExit_Click(object sender, EventArgs e)
        {
            Icon.Visible = false;
            Environment.Exit(0);
        }

        private static void ItemOpen_Click(object sender, EventArgs e)
        {
            _window.Show();
            _window.WindowState = WindowState.Normal;
            Icon.Visible = false;
        }

        private static void Icon_DoubleClick(object sender, EventArgs e)
        {
            _window.Show();
            _window.WindowState = WindowState.Normal;
            Icon.Visible = false;
        }
    }
}