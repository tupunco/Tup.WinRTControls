using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Tup.WinRTControls
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var ran = new Random();
            var list = new List<Pair<string, string>>();
            for (int gIndex = 0; gIndex < 20; gIndex++)
            {
                var cTotal = ran.Next(10, 50);
                var cGroup = "group " + gIndex;
                for (int cIndex = 0; cIndex < cTotal; cIndex++)
                {
                    list.Add(new Pair<string, string>(cGroup, string.Format("item {0}-{1}", gIndex, cIndex)));
                }
            }
            var groupList = list.GroupBy(x => x.Key);
            this.LongListSelector.ItemsSource = groupList;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.ProgressOverlay.Show();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.ProgressOverlay.Hide();
        }
    }

    public class Pair<TKey, TValue>
    {
        public Pair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }
        public override string ToString()
        {
            return string.Format("[Pair Key:{0} Value:{1}]", Key, Value);
        }
    }
}
