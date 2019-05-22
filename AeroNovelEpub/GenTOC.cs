using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace AeroNovelEpub
{
    public  class GenTOC
    {

        public static string Gen(string[] lines,string uid,string title)
        {
            string temp=File.ReadAllText("template/toc.txt");
            string r="";
            List<string> label=new List<string>();
            int depth=1;
            int count=0;
            foreach(string line in lines)
            {
                Match m=Regex.Match(line,"\\[(.*?)\\]");
                if(m.Success)
                {
                    string tag=m.Groups[1].Value;
                    if(tag[0]=='/')
                    {
                        label.RemoveAt(label.Count-1);
                        r+="</navPoint>\n";
                    }else
                    {
                        label.Add(tag);
                        if(depth<label.Count+1){depth=label.Count+1;}
                        count++;
                        r+=string.Format("<navPoint id=\"navPoint-{0}\" playOrder=\"{0}\"><navLabel><text>{1}</text></navLabel><content src=\"dummylink\"/>\n",count,tag);
                    }
                    continue;
                }
                m=Regex.Match(line,"([0-9][0-9])(.*?).txt");
                if(m.Success)
                {
                count++;
                string link="Text/t"+m.Groups[1].Value+".xhtml";
                r+=string.Format("<navPoint id=\"navPoint-{0}\" playOrder=\"{0}\"><navLabel><text>{1}</text></navLabel><content src=\"{2}\"/></navPoint>\n",count,m.Groups[2].Value,link);
                r=r.Replace("dummylink",link);
                }
      
            }
            return string.Format(temp,uid,depth,title,r);
        }
    }
}