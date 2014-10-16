using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Moonmile.ExDoc;

namespace EnqueteCheck
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        async Task<XDocument> GetXHTML(string url)
        {
            var cl = new HttpClient();
            var text = await cl.GetStringAsync(url);
            text = HtmlConv.HTMLtoXHTML(text);
            text = text.Replace("=hidden", "=\"hidden\"")
                .Replace("=checkbox", "=\"checkbox\"")
                .Replace("=radio", "='radio'")
                .Replace("=image", "='image'")
                .Replace("value=0", "value='0'")
                .Replace("value=1", "value='1'")
                .Replace("value=2", "value='2'")
                .Replace("value=3", "value='3'")
                .Replace("value=4", "value='4'")
                .Replace("value=5", "value='5'")
                .Replace("value=6", "value='6'")
                .Replace("value=7", "value='7'")
                .Replace("value=8", "value='8'")
                .Replace("value=9", "value='9'")
                .Replace("&", "&amp;")
                ;
            textOut.Text = text;


            var st = new StringReader(text);
            try
            {
                var doc = XDocument.Load(st);
                return doc;
            }
            catch (Exception ex)
            {
                textError.Text = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// チェックボックスの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        string CheckboxToXaml(ExElement el)
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
                    var ee = new XElement("CheckBox",
                        new XAttribute("Content", node.Value));
                    panel.Add(ee);
                }
            }
            return hub.ToString(SaveOptions.None);
        }

        /// <summary>
        /// ラジオボタンの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        string RadioToXaml(ExElement el)
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
                    var ee = new XElement("RadioButton",
                        new XAttribute("Content", node.Value));
                    panel.Add(ee);
                }
            }
            return hub.ToString(SaveOptions.None);
        }

        /// <summary>
        /// テキストボックスの質問を作成
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        string TextareaToXaml(ExElement el)
        {
            string qtext = ((ExElement)(el * "td" % "class" == "qtext")).Value;
            string qbody = ((ExElement)(el * "td" % "class" == "qbody")).Value;
            var lst = el * "input" % "type" == "textarea";

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
                if ((it.Parent % "id").Value != "")
                {
                    {
                        panel.Add(
                            new XElement("TextBox",
                            new XAttribute("Height", "200"),
                            new XAttribute("AcceptsReturn", "True")));
                        panel.Add(
                            new XElement("TextBlock",
                            new XAttribute("Text", "あと400文字入力できます")));
                    }
                }
            }
            return hub.ToString(SaveOptions.None);
        }

        public async Task<List<string>> MakeHubSection( string id )
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
            var hubs = new List<string>();
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
        private async void OnClickCheck(object sender, RoutedEventArgs e)
        {
            string id = this.textID.Text;
            var hubs = await MakeHubSection(id);
        }
    }
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
