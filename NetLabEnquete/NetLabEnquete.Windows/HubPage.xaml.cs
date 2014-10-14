using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NetLabEnquete.Data;
using NetLabEnquete.Common;
using Moonmile.ExDoc;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using Windows.UI.Xaml.Markup;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Windows.UI.Popups;


// ユニバーサル ハブ アプリケーション プロジェクト テンプレートについては、http://go.microsoft.com/fwlink/?LinkID=391955 を参照してください

namespace NetLabEnquete
{
    /// <summary>
    /// グループ化されたアイテムのコレクションを表示するページです。
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// ナビゲーションおよびプロセス継続時間管理を支援するために使用される NavigationHelper を取得します。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// DefaultViewModel を取得します。これは厳密に型指定されたビュー モデルに変更できます。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="sender">
        /// イベントのソース (通常、<see cref="NavigationHelper"/>)
        /// </param>
        /// <param name="e">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, object)"/> に渡されたナビゲーション パラメーターと、
        /// 前のセッションでこのページによって保存された状態の辞書を提供する
        /// イベント データ。ページに初めてアクセスするとき、状態は null になります。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: 対象となる問題領域に適したデータ モデルを作成し、サンプル データを置き換えます
        }

        /// <summary>
        /// HubSection ヘッダーがクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender">ヘッダーがクリックされた HubSection を含むハブ。</param>
        /// <param name="e">クリックがどのように開始されたかを説明するイベント データ。</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// セクション内のアイテムがクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender">GridView または ListView です。
        /// されている場合は ListView) です。</param>
        /// <param name="e">クリックされたアイテムを説明するイベント データ。</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 適切な移動先のページに移動し、新しいページを構成します。
            // このとき、必要な情報をナビゲーション パラメーターとして渡します
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper の登録

        /// <summary>
        /// このセクションに示したメソッドは、NavigationHelper がページの
        /// ナビゲーション メソッドに応答できるようにするためにのみ使用します。
        /// ページ固有のロジックは、
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// および <see cref="Common.NavigationHelper.SaveState"/> のイベント ハンドラーに配置する必要があります。
        /// LoadState メソッドでは、前のセッションで保存されたページの状態に加え、
        /// ナビゲーション パラメーターを使用できます。
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        public void LoadState(string args)
        {
            this.ID = args;
            this.textID.Text = this.ID;
            PageUpdate(this.ID);

        }
        /// <summary>
        /// テスト用のID
        /// </summary>
        private string ID = "6No8Z8f";

        /// <summary>
        /// 画面を更新
        /// </summary>
        /// <param name="id"></param>
        private async void PageUpdate( string id )
        {
            _names = new Dictionary<string, object>();
            var hubs = await MakeHubSection(this.ID);

            // xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
            // xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'

            // 最初と最後だけ残す
            var sec0 = this.hub.Sections[0];
            var sece = this.hub.Sections[this.hub.Sections.Count - 1];
            this.hub.Sections.Clear();
            this.hub.Sections.Add(sec0);

            foreach (var it in hubs)
            {
                it.SetAttributeValue("ns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                var xml = it.ToString(SaveOptions.None);
                xml = xml.Replace("ns=\"http", "xmlns=\"http");
                var sec = XamlReader.Load(xml) as HubSection;
                this.hub.Sections.Add(sec);

            }
            this.hub.Sections.Add(sece);

            // 画面を更新して _names を作る
            await Task.Delay(1);

            var names = new Dictionary<string, object>();
            foreach (var key in _names.Keys)
            {
                var obj = FindByName(key, this);
                names[key] = obj;
            }
            _names = names;
        }

        private void OnClickUpdate(object sender, RoutedEventArgs e)
        {
            this.ID = this.textID.Text;
            PageUpdate(this.ID);
        }

        Dictionary<string, object> _names = new Dictionary<string, object>();

        object FindByName(string name, DependencyObject el , int indent = 0)
        {
            string sp = "";
            for ( int i=0; i<indent; i++ ) sp += " ";
            // Debug.WriteLine(sp + el.GetType().Name);
            var fw = el as FrameworkElement;
            if (fw != null)
            {
                if (fw.Name == name)
                {
                    return fw;
                }
            }
            int cnt = VisualTreeHelper.GetChildrenCount( el );
            for (int i = 0; i < cnt; i++)
            {
                var child = VisualTreeHelper.GetChild(el, i);
                var obj = FindByName(name, child, indent + 1);
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        async Task<XDocument> GetXHTML(string url)
        {

            var cl = new HttpClient();
            var text = await cl.GetStringAsync(url);
            text = HtmlConv.HTMLtoXHTML(text);

            text = Regex.Replace(text, "type=(\\w+)", "type='$1'");
            text = Regex.Replace(text, "value=(\\w+)", "value='$1'");
            text = Regex.Replace(text, "size=(\\w+)", "size='$1'");
            text = Regex.Replace(text, "maxlength=(\\w+)", " maxlength='$1'");
            text = text.Replace("&", "&amp;");

            var st = new StringReader(text);
            try
            {
                var doc = XDocument.Load(st);
                return doc;
            }
            catch (Exception ex)
            {
                var dlg = new MessageDialog("XDocument のパースに失敗しました");
                dlg.ShowAsync();
            }
            return null;
        }

        /// <summary>
        /// チェックボックスの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        XElement CheckboxToXaml(ExElement el)
        {
            string qtext = ((ExElement)(el * "td" % "class" == "qtext")).Value;
            string qbody = ((ExElement)(el * "td" % "class" == "qbody")).Value;
            var lst = el * "input" % "type" == "checkbox";

            var panel = new XElement("StackPanel", new XAttribute("Grid.Row", "1"));
            var hub = new XElement("HubSection",
                new XAttribute("Width", "400"),
                new XAttribute("Header", qtext),
                 new XElement("DataTemplate",
                     new XElement("Grid",
                         new XElement("Grid.RowDefinitions",
                         new XElement("RowDefinition", new XAttribute("Height", "50")),
                         new XElement("RowDefinition", new XAttribute("Height", "*"))),
                         new XElement("TextBlock",
                             new XAttribute("Grid.Row", "0"),
                             new XAttribute("FontSize", "16"),
                             new XAttribute("TextWrapping", "NoWrap"),
                             new XAttribute("Text", qbody)),
                        panel)));
            foreach (var it in lst)
            {
                var node = it.Parent.XElement.NextNode as XText;
                if (node != null)
                {
                    string name = it.Parent % "name";
                    string value = it.Parent % "value";
                    string xname = name.Replace("[]","") + "-" + value;
                    _names[xname] = null;

                    var ee = new XElement("CheckBox",
                        new XAttribute("Content", node.Value),
                        new XAttribute("Name", xname));
                    panel.Add(ee);
                }
            }
            return hub;
        }

        /// <summary>
        /// ラジオボタンの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        XElement RadioToXaml(ExElement el)
        {
            string qtext = ((ExElement)(el * "td" % "class" == "qtext")).Value;
            string qbody = ((ExElement)(el * "td" % "class" == "qbody")).Value;
            var lst = el * "input" % "type" == "radio";

            var panel = new XElement("StackPanel", new XAttribute("Grid.Row", "1"));
            var hub = new XElement("HubSection",
                new XAttribute("Width", "400"),
                new XAttribute("Header", qtext),
                 new XElement("DataTemplate",
                     new XElement("Grid",
                         new XElement("Grid.RowDefinitions",
                         new XElement("RowDefinition", new XAttribute("Height", "50")),
                         new XElement("RowDefinition", new XAttribute("Height", "*"))),
                         new XElement("TextBlock",
                             new XAttribute("Grid.Row", "0"),
                             new XAttribute("FontSize", "16"),
                             new XAttribute("TextWrapping", "NoWrap"),
                             new XAttribute("Text", qbody)),
                        panel)));

            foreach (var it in lst)
            {
                var node = it.Parent.XElement.NextNode as XText;
                if (node != null)
                {
                    string name = it.Parent % "name";
                    string value = it.Parent % "value";
                    string xname = name + "-" + value;
                    _names[xname] = null;

                    var ee = new XElement("RadioButton",
                        new XAttribute("Content", node.Value),
                        new XAttribute("Name", xname )
                        );
                    panel.Add(ee);
                }
            }
            return hub;
        }

        /// <summary>
        /// 複数行テキストボックスの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        XElement TextareaToXaml(ExElement el)
        {
            string qtext = ((ExElement)(el * "td" % "class" == "qtext")).Value;
            string qbody = ((ExElement)(el * "td" % "class" == "qbody")).Value;
            var lst = el * "textarea";

            var panel = new XElement("StackPanel", new XAttribute("Grid.Row", "1"));
            var hub = new XElement("HubSection",
                new XAttribute("Width", "400"),
                new XAttribute("Header", qtext),
                 new XElement("DataTemplate",
                     new XElement("Grid",
                         new XElement("Grid.RowDefinitions",
                         new XElement("RowDefinition", new XAttribute("Height", "50")),
                         new XElement("RowDefinition", new XAttribute("Height", "*"))),
                         new XElement("TextBlock",
                             new XAttribute("Grid.Row", "0"),
                             new XAttribute("FontSize", "16"),
                             new XAttribute("TextWrapping", "NoWrap"),
                             new XAttribute("Width", "380"),
                             new XAttribute("Text", qbody)),
                        panel)));

            foreach (var it in lst)
            {
                if ((it % "name").Value != "")
                {
                    string name = it % "name";
                    string xname = name;
                    _names[xname] = null;

                    panel.Add(
                        new XElement("TextBox",
                            new XAttribute("Name", xname ),
                            new XAttribute("Height", "200"),
                            new XAttribute("AcceptsReturn", "True")));
                    panel.Add(
                        new XElement("TextBlock",
                        new XAttribute("Text", "あと400文字入力できます")));
                }
            }
            return hub;
        }

        /// <summary>
        /// 単数行テキストボックスの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        XElement TextToXaml(ExElement el)
        {
            string qtext = ((ExElement)(el * "td" % "class" == "qtext")).Value;
            string qbody = ((ExElement)(el * "td" % "class" == "qbody")).Value;
            var lst = el * "input";

            var panel = new XElement("StackPanel", new XAttribute("Grid.Row", "1"));
            var hub = new XElement("HubSection",
                new XAttribute("Width", "400"),
                new XAttribute("Header", qtext),
                 new XElement("DataTemplate",
                     new XElement("Grid",
                         new XElement("Grid.RowDefinitions",
                         new XElement("RowDefinition", new XAttribute("Height", "50")),
                         new XElement("RowDefinition", new XAttribute("Height", "*"))),
                         new XElement("TextBlock",
                             new XAttribute("Grid.Row", "0"),
                             new XAttribute("FontSize", "16"),
                             new XAttribute("TextWrapping", "NoWrap"),
                             new XAttribute("Width", "380"),
                             new XAttribute("Text", qbody)),
                        panel)));

            foreach (var it in lst)
            {
                if ((it % "name").Value != "")
                {
                    string name = it % "name";
                    string xname = name;
                    _names[xname] = null;
                    panel.Add(
                        new XElement("TextBox",
                            new XAttribute("Name", xname),
                            new XAttribute("AcceptsReturn", "False")));
                }
            }
            return hub;
        }


        public async Task<List<XElement>> MakeHubSection(string id)
        {
            string url = "http://enq-maker.com/" + id;
            XDocument xdoc = await GetXHTML(url);
            var doc = ExDocument.Load(xdoc);

            var lst = doc * "td" % "class" == "qtext";
            var lstb = doc * "td" % "class" == "qbody";
            if (lst.Count == 0) return null;
            if (lstb.Count == 0) return null;
            var el = lst[0];
            var elb = lstb[0];

            // 親のtable タグを取得
            var els = new ExElements();
            foreach (var it in lst)
            {
                els.Add(it.Parent.Parent.Parent.Parent.Parent.Parent);
            }

            // HubSection の XML を作成する
            var hubs = new List<XElement>();
            foreach (var it in els)
            {
                string attr = it * "input" % "type";
                if (attr == "checkbox")
                {
                    hubs.Add(CheckboxToXaml(it));
                }
                else if (attr == "radio")
                {
                    hubs.Add(RadioToXaml(it));
                }
                else if (attr == "text")
                {
                    hubs.Add(TextToXaml(it));
                }
                else if (attr == "")
                {
                    var ee = it * "textarea";
                    if (ee.Count > 0)
                    {
                        hubs.Add(TextareaToXaml(it));
                    }
                }
            }
            return hubs;
        }

        /// <summary>
        /// 投稿ボタンをクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickSubmit(object sender, RoutedEventArgs e)
        {
            /// 1.選択状態を取得
            /// 2.POSTデータを作成
            var cl = new HttpClient();
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("id", this.ID));

            /// 2.1 _names を参照してデータを作る
            foreach (var key in _names.Keys)
            {
                var obj = _names[key];
                if (obj is TextBox)
                {
                    var o = obj as TextBox;
                    dic.Add(new KeyValuePair<string, string>(key, o.Text));
                }
                else if (obj is CheckBox)
                {
                    var o = obj as CheckBox;
                    if (o.IsChecked == true)
                    {
                        var v = key.Split(new char[] { '-' });
                        dic.Add(new KeyValuePair<string, string>(v[0] + "[]", v[1]));
                    }
                }
                else if (obj is RadioButton)
                {
                    var o = obj as RadioButton;
                    if (o.IsChecked == true)
                    {
                        var v = key.Split(new char[] { '-' });
                        dic.Add(new KeyValuePair<string, string>(v[0] , v[1]));
                    }
                }
            }
            var cont = new FormUrlEncodedContent(dic);
            string url = "http://enq-maker.com/-/receiveAnswers"; //  +this.ID;
            /// 3.指定URLへポストする
            
            /// 3.1 UTF-8 から SJIS に直す
            var utf8e = await cont.ReadAsStringAsync();
            var utf8 = System.Net.WebUtility.UrlDecode(utf8e);
            var sjisb = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(utf8);
            var data = System.Net.WebUtility.UrlEncodeToBytes(sjisb, 0, sjisb.Length);
            var datas = System.Text.Encoding.GetEncoding("shift_jis").GetString(data, 0, data.Length);
            datas = datas.Replace("%3D", "=").Replace("%26", "&");
            var conts = new StringContent(datas, System.Text.Encoding.GetEncoding("shift_jis"), "application/x-www-form-urlencoded" );
            await cl.PostAsync(url, conts);

            // await cl.PostAsync(url, cont);
            /// 4.完了メッセージを表示
            var dlg = new MessageDialog("アンケート投稿をありがとうございました");
            dlg.Commands.Add(new UICommand("OK"));
            dlg.DefaultCommandIndex = 0;
            await dlg.ShowAsync();					
        }

    }

    /// <summary>
    /// おおざっぱに HTML形式を XHTML形式に変換するクラス
    /// </summary>
    public class HtmlConv
    {
        class Lex
        {
            int pos = -1;
            string _html = "";
            public Lex(string html) { _html = html; }
            public string getc()
            {
                if (pos < _html.Length - 1)
                {
                    try
                    {
                        pos++;
                        return _html.Substring(pos, 1);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            public void unget()
            {
                if (pos >= 0) pos--;
            }
            public string getw()
            {
                string ch = getc();
                if (ch == "") return "";
                switch (ch)
                {
                    case "<":
                        if (getc() == "!")
                        {
                            return "<!";
                        }
                        else
                        {
                            unget();
                            return "<";
                        }
                    case ">": return ">";
                    default:
                        string s = ch;
                        if ('A' <= ch[0] && ch[0] <= 'Z' ||
                             'a' <= ch[0] && ch[0] <= 'z' ||
                             ch[0] <= '_')
                        {
                            while (true)
                            {
                                ch = getc();
                                if (ch == "") break;
                                if ('0' <= ch[0] && ch[0] <= '9' ||
                                     'A' <= ch[0] && ch[0] <= 'Z' ||
                                     'a' <= ch[0] && ch[0] <= 'z' ||
                                     ch[0] == '_')
                                {
                                }
                                else
                                {
                                    unget();
                                    break;
                                }
                                s += ch;
                            }
                        }
                        return s;
                }
            }
        }

        public static string HTMLtoXHTML(string html)
        {
            string[] tags = { "meta", "link", "img", "br", "input" };
            var lex = new Lex(html);
            string xhtml = "";
            while (true)
            {
                string w = lex.getw();
                if (w == "") break;
                if (w == "<!")
                {
                    // > まで読み飛ばし
                    while (true)
                    {
                        w = lex.getw();
                        if (w == "") break;
                        if (w == ">") break;
                    }
                    continue;
                }
                xhtml += w;
                if (w == "<")
                {
                    w = lex.getw();
                    xhtml += w;
                    if (tags.Any(x => x == w))
                    {
                        while (true)
                        {
                            w = lex.getw();
                            if (w == "") break;
                            if (w == ">")
                            {
                                xhtml += "/>";
                                break;
                            }
                            xhtml += w;
                        }
                    }
                }
            }
            return xhtml;
        }
    }
}
