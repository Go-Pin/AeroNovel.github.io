using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace AeroNovelEpub
{
    public class GenHtml
    {
        public static string Gen(string[] txt)
        {
            int note_count = 0;
            List<string> notes = new List<string>();
            var regs = new string[]{
                "\\[align=(.*?)\\](.*?)\\[\\/align\\]",
                "\\[note\\]",
                "\\[note=(.*?)\\]",
                "\\[img\\](.*?)\\[\\/img\\]",
                "\\[b\\](.*?)\\[\\/b\\]"
                };

            var repls = new string[]{
                "<p class=\"aligned\" style=\"text-align:$1\">$2</p>",
                "",
                "",
                "",
                "<b>$1</b>"
                };

            string html = "";
            bool titled = false;
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
                                case 1://noteref
                                    string noteref_temp = "<a class=\"noteref\" epub:type=\"noteref\" href=\"#note{0}\" id=\"note_ref{0}\"><sup>[注]</sup></a>";
                                    r = reg.Replace(r, string.Format(noteref_temp, note_count), 1);
                                    note_count++;
                                    break;
                                case 2://note
                                    notes.Add(m.Groups[1].Value);
                                    r = reg.Replace(r, "", 1);
                                    break;
                                case 3://img
                                    string src = "../Images/" + Path.GetFileName(m.Groups[1].Value);
                                    string img_temp = "<p class=\"aligned illu\"><img class=\"illu\" src=\"{0}\" alt=\"\"/></p>";
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
                if (r.Length == 0) { r = "<br/>"; }
                if (!titled) { r = "<p class=\"title0\">" + r + "</p>"; titled = true; }
                if (!Regex.Match(r, "<p.*>").Success)
                {
                    if (r[0] == '（' || r[0] == '「' || r[0] == '『')
                    {
                        r = "<p class=\"drawout\">" + r + "</p>";
                    }
                    else
                        r = "<p>" + r + "</p>";
                }
                html += r + "\n";
            }
            string note_temp = "<aside epub:type=\"footnote\" id=\"note{0}\"><a href=\"#note_ref{0}\"></a><p class=\"pagebreak\">{1}</p></aside>\n";
            int count = 0;
            foreach (string note in notes)
            {
                html += string.Format(note_temp, count, note);
            }
            return html;
        }
    }
}