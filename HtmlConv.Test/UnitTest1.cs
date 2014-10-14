using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnqueteCheck;

namespace HtmlConvTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimpleHTML()
        {
            string html =
"<html>" +
"<head>" +
"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=Shift_JIS\">" +
"<title>2013年9月 .NETラボ勉強会についてのアンケート</title>" +
"<link rel=\"stylesheet\" href=\"/css.php?id=style_enq\" type=\"text/css\">" +
"<script src=\"/js/jquery.js\" type=\"text/javascript\"></script>" +
"<script src=\"/js/scripts.js\" type=\"text/javascript\"></script>" +
"</head>" +
"</html>";

            string xhtml =
"<html>" +
"<head>" +
"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=Shift_JIS\"/>" +
"<title>2013年9月 .NETラボ勉強会についてのアンケート</title>" +
"<link rel=\"stylesheet\" href=\"/css.php?id=style_enq\" type=\"text/css\"/>" +
"<script src=\"/js/jquery.js\" type=\"text/javascript\"></script>" +
"<script src=\"/js/scripts.js\" type=\"text/javascript\"></script>" +
"</head>" +
"</html>";

            string res = HtmlConv.HTMLtoXHTML(html);
            Assert.AreEqual(xhtml, res);
        }
        [TestMethod]
        public void SimpleHTML2()
        {
            string html =
"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"" +
"\"http://www.w3.org/TR/html4/loose.dtd\">" +
"<html>" +
"<head>" +
"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=Shift_JIS\">" +
"<title>2013年9月 .NETラボ勉強会についてのアンケート</title>" +
"<link rel=\"stylesheet\" href=\"/css.php?id=style_enq\" type=\"text/css\">" +
"<script src=\"/js/jquery.js\" type=\"text/javascript\"></script>" +
"<script src=\"/js/scripts.js\" type=\"text/javascript\"></script>" +
"</head>" +
"</html>";

            string xhtml =
"<html>" +
"<head>" +
"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=Shift_JIS\"/>" +
"<title>2013年9月 .NETラボ勉強会についてのアンケート</title>" +
"<link rel=\"stylesheet\" href=\"/css.php?id=style_enq\" type=\"text/css\"/>" +
"<script src=\"/js/jquery.js\" type=\"text/javascript\"></script>" +
"<script src=\"/js/scripts.js\" type=\"text/javascript\"></script>" +
"</head>" +
"</html>";

            string res = HtmlConv.HTMLtoXHTML(html);
            Assert.AreEqual(xhtml, res);
        }
    }
}
