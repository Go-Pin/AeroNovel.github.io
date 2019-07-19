using System;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
namespace AeroNovelEpub
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
                if (Directory.Exists(args[0]))
                {
                    string uid="urn:uuid:"+Guid.NewGuid().ToString();
                    string contents = Path.Combine(args[0], "contents.txt");
                    string xhtml_temp = File.ReadAllText(@"template\xhtml.txt");
                    if (Directory.Exists("temp"))
                        DeleteDir("temp");
                    if (!Directory.Exists("temp")) { Directory.CreateDirectory("temp"); }
                    if (!Directory.Exists("temp\\OEBPS")) { Directory.CreateDirectory("temp\\OEBPS"); }
                    if (!Directory.Exists("temp\\OEBPS\\Text")) { Directory.CreateDirectory("temp\\OEBPS\\Text"); }
                    if (!Directory.Exists("temp\\OEBPS\\Images")) { Directory.CreateDirectory("temp\\OEBPS\\Images"); }
                    if (!Directory.Exists("temp\\OEBPS\\Styles")) { Directory.CreateDirectory("temp\\OEBPS\\Styles"); }
                    string spine = "";
                    GenHtml genHtml=new GenHtml(args[0]);
                    if (File.Exists(contents))
                    {
                        string[] files = File.ReadAllLines(contents);
                        foreach (string f in files)
                        {
                            string filepath = Path.Combine(args[0], f);
                            if (File.Exists(filepath))
                            {
                                Match m = Regex.Match(f, "([0-9][0-9])(.*?).txt");
                                string no = m.Groups[1].Value;
                                string chaptitle = m.Groups[2].Value;
                                string[] lines = File.ReadAllLines(filepath);
                                string body = genHtml.Gen(lines);
                                if (f.Contains("info.txt"))
                                {
                                    body = "<div class=\"info\" epub:type=\"acknowledgements\">" + body + "<p>AeroNovel EPUB生成器by AE " + DateTime.Now + "</p><p>支持pop-up footnote</p><p>推荐使用阅读器:<br/>Apple Books<br/>Microsoft Edge (1809)<br/>Kindle(使用Kindlegen 转换)<br/>Lithium (Android)</p></div>";
                                    //File.WriteAllText("info.txt",body);
                                }
                                string xhtml = xhtml_temp.Replace("{❤title}", chaptitle).Replace("{❤body}", body);
                                File.WriteAllText("temp/OEBPS/Text/t" + no + ".xhtml", xhtml);
                                spine += string.Format("<itemref idref=\"{0}\"/>", "t" + no + ".xhtml", xhtml);
                                Console.WriteLine("Saved " + no);
                            }
                            else
                            {
                                if (f.Length > 0)
                                    Console.WriteLine("Cannot find " + f);
                            }

                        }
                    }
                    string img = Path.Combine(args[0], "Images");
                    if (Directory.Exists(img))
                    {
                        foreach (var f in Directory.GetFiles(img))
                        {
                            if(f.Contains(".jpg"))
                            File.Copy(f, Path.Combine("temp\\OEBPS\\Images", Path.GetFileName(f)));
                        }
                    }
                    else { Console.WriteLine("img?"); }
                    File.Copy(@"template\style.css", "temp\\OEBPS\\Styles\\Style.css");

                    string meta = File.ReadAllText(Path.Combine(args[0], "meta.txt"));
                    meta=meta.Replace("{urn:uuid}",uid);
                    string title = Regex.Match(meta, "<dc:title.*?>(.*?)</dc:title>").Groups[1].Value;
                    string toc = GenTOC.Gen(File.ReadAllLines(Path.Combine(args[0], "toc.txt")), uid, title);
                    File.WriteAllText("temp\\OEBPS\\toc.ncx", toc);
                    string items = "";
                    foreach (string f in Directory.GetFiles("temp\\OEBPS\\Text"))
                    {
                        items += string.Format("<item id=\"{0}\" href=\"Text/{0}\" media-type=\"application/xhtml+xml\"/>\n", Path.GetFileName(f));
                    }
                    foreach (string f in Directory.GetFiles("temp\\OEBPS\\Images"))
                    {
                        items += string.Format("<item id=\"{0}\" href=\"Images/{0}\" media-type=\"image/jpeg\"/>\n", Path.GetFileName(f));
                    }
                    string opf_temp = File.ReadAllText("template/opf.txt");
                    string opf = string.Format(opf_temp, meta, items, spine);
                    File.WriteAllText("temp\\OEBPS\\content.opf", opf);
                    File.WriteAllText("temp\\mimetype", "application/epub+zip");
                    Directory.CreateDirectory("temp\\META-INF");
                    File.Copy("template/container.xml", "temp\\META-INF\\container.xml");
                    if(File.Exists(title + ".epub"))File.Delete(title + ".epub");
                    ZipFile.CreateFromDirectory("temp", title + ".epub");
                    DeleteDir("temp");



                }

        }
        static void DeleteDir(string path)
        {
            foreach (string p in Directory.GetFiles(path)) File.Delete(p);
            foreach (string p in Directory.GetDirectories(path)) DeleteDir(p);
            Directory.Delete(path);
        }
    }
}
