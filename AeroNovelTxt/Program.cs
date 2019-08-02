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
                    string txt = "";

                    string contents = Path.Combine(args[0], "contents.txt");

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
                                string body = Gen(lines);
                                txt += chaptitle + "\r\n" + body + "\r\n=====================\r\n";
                                Console.WriteLine("Added " + chaptitle);
                            }
                            else
                            {
                                if (f.Length > 0)
                                    Console.WriteLine("Cannot find " + f);
                            }

                        }
                    }
                    File.WriteAllText("test.txt", txt);
                }

        }
        public static string Gen(string[] txt)
        {
            var regs = new string[]{
                "\\[align=(.*?)\\](.*?)\\[\\/align\\]",
                "\\[note\\]",
                "\\[note=(.*?)\\]",
                "\\[img\\](.*?)\\[\\/img\\]",
                "\\[b\\](.*?)\\[\\/b\\]",
                "\\[title\\](.*?)\\[\\/title\\]",
                "\\[ruby=(.*?)\\](.*?)\\[\\/ruby\\]",
                "\\[pagebreak\\]",
                "/\\*.*?\\*/"
                };

            var repls = new string[]{
                "$2",
                "[注]",
                "[$1]",
                "[图片：$1]",
                "$1",
                "$1",
                "$2（$1）",
                "",
                ""
                };

            string html = "";
            foreach (string line in txt)
            {
                string r = line;
                Match m = Regex.Match("", "1");
                do
                {
                    for (int i = 0; i < regs.Length; i++)
                    {
                        m = Regex.Match(r, regs[i]);
                        if (m.Success)
                        {
                            Regex reg = new Regex(regs[i]);
                            switch (i)
                            {
                                case 3://img
                                    string src = Path.GetFileName(m.Groups[1].Value);
                                    string img_temp = "图片：{0}";
                                    r = reg.Replace(r, string.Format(img_temp, src), 1);
                                    break;
                                default:
                                    r = reg.Replace(r, repls[i]);
                                    break;
                            }
                            break;
                        }

                    }
                } while (m.Success);
                html += "　　" + r + "\r\n";
            }


            return html;
        }

    }
}
