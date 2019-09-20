using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace LogCreator
{
    public static class FileManager
    {
        public static string OutputFileName = string.Empty;
        static string Source_File_Name = string.Empty;
        public static byte[] FileDataBytes = null;
        public static List<string> FileLines = null; 
        public static void ReadFile(string path)
        {
            //string data = string.Empty;
            if (File.Exists(path))
            {
                FileLines = File.ReadAllLines(path).OfType<string>().ToList();
            }
            //return data;
        }

        public static bool CreateLogFile(bool isMultipleFileSelected)
        {
            bool Result = true;
            try
            {
                if (!isMultipleFileSelected || string.IsNullOrEmpty(OutputFileName))
                    OutputFileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles\UpdatedLog_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_fff") + ".log";

                string path = Path.GetDirectoryName(OutputFileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                byte[] header = Encoding.UTF8.GetBytes("Source_File_Name|Time|Number|ProcessID|Message_type|MethodName|RequestID|IsBlock|Listings|ContentSize|URL|Message\n");
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(OutputFileName))
                {
                    if (!isMultipleFileSelected)
                    {
                        File.Delete(OutputFileName);
                        using (FileStream fs = File.Create(OutputFileName))
                        {
                            fs.Write(header, 0, header.Length);
                            fs.Write(FileDataBytes, 0, FileDataBytes.Length);
                        }
                    }
                    else
                    {
                        using (FileStream fs = File.Open(OutputFileName, FileMode.Append))
                        {
                            fs.Write(FileDataBytes, 0, FileDataBytes.Length);
                        }
                    }
                }
                else
                {
                    using (FileStream fs = File.Create(OutputFileName))
                    {
                        fs.Write(header, 0, header.Length);
                        fs.Write(FileDataBytes, 0, FileDataBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Result = false;
            }
            finally
            {
                FileDataBytes = null;
            }
            return Result;
        }

        public async static /*void*/ Task<bool> ProcessData(string FileName)
        {
            //string FinalText = string.Empty;
            Source_File_Name = FileName;
            try
            {
                //string[] columns = Regex.Split(data, @"\t");
                //FinalText = string.Join(",", columns);

                //data = data.Replace("\r\n", "\n");
                //get the list of string from single string
                //var result = data.Split('\n').ToList();
                //split the string in different section and modify
                await Task.Run(() =>
                {
                    try
                    {
                        for (int i = 0; i < FileLines.Count; i++)
                        {
                            if (string.IsNullOrEmpty(FileLines[i]))
                            {
                                FileLines.RemoveAt(i);
                                i--;
                                continue;
                            }
                                

                            var match = Regex.Match(FileLines[i], @"^([\d]{1,2}:[\d]{1,2}:[\d]{1,2}\s[AaPp][Mm])");
                            if (match.Success)
                            {
                                FileLines[i] = /*await*/ UpdateLog(i);
                            }
                            else
                            {
                                FileLines[i - 1] += " " + FileLines[i];
                                FileLines.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //
                    }
                    
                });
                FileDataBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, FileLines.ToArray()));
                FileLines = null;
                // FinalText = string.Join(Environment.NewLine, FileLines.ToArray());
            }
            catch (Exception ex)
            {
                //error
            }
            return true;
        }
        /*async*/ static string /*Task<string>*/ UpdateLog(int index)
        {
            string Result = string.Empty;
            try
            {
                //get the time
                var time = Regex.Matches(FileLines[index], @"^([\d]{1,2}:[\d]{1,2}:[\d]{1,2}\s[AaPp][Mm])")
                .Cast<Match>()
                .Select(m => m.Value).First()
                .ToString();

                //get the number
                var Number = Regex.Matches(FileLines[index], @"\[[\d]+\]")
                .Cast<Match>()
                .Select(m => m.Value).First()
                .ToString();

                //get the process id
                var ProcessID = Regex.Matches(FileLines[index], @"\([\d]+:[\d]+\)")
               .Cast<Match>()
               .Select(m => m.Value).First()
               .ToString();

                //get the tag and details
                var match = Regex.Match(FileLines[index], @"\([\d]+:[\d]+\)");
                var tmp = FileLines[index].Remove(0, match.Index + match.Length);
                match = Regex.Match(tmp, @"[a-zA-z]");
                tmp = tmp.Remove(0, match.Index);

                var Message_type = Regex.Matches(tmp, @"^[\w]+")
                    .Cast<Match>()
                    .Select(m => m.Value).First()
                    .ToString();
                var Message = tmp.Substring(Message_type.Length);
                match = Regex.Match(Message, @"[a-zA-z]");
                Message = Message.Remove(0, match.Index);

                //get MethodName
                string MethodName = "NA";
                //MethodName = Message.Substring(0, Message.IndexOf(" "));
                match = Regex.Match(Message, @"^[\w]+\(?[\w\d]*\)?:");
                if (match.Success)
                {
                    MethodName = Message.Substring(0, match.Length - 1);
                }
                if (string.IsNullOrEmpty(MethodName))
                    MethodName = "NA";

                //get RequestID
                match = Regex.Match(Message, @"ID[\s]?:");
                string RequestID = "NA";
                if (match.Success)
                {
                    RequestID = Message.Remove(0, match.Index + match.Length);
                    match = Regex.Match(RequestID, @"[\d]+");
                    RequestID = RequestID.Substring(match.Index, match.Length);
                }
                if (string.IsNullOrEmpty(RequestID))
                    RequestID = "NA";
                

                //get IsBlock
                string IsBlock = "NA";
                if (MethodName.ToLower().Contains("DetectBlock".ToLower()))
                {
                    if (Message.ToLower().Contains("is blocked.".ToLower()))
                    {
                        IsBlock = "true";
                    }
                    else
                    {
                        IsBlock = "false";
                    }
                }
                else
                {
                    match = Regex.Match(Message, @"isBlock[\s]?:");
                    if (match.Success)
                    {
                        IsBlock = Message.Remove(0, match.Index + match.Length);
                        match = Regex.Match(IsBlock, @"[a-zA-z]");
                        IsBlock = IsBlock.Remove(0, match.Index);
                        match = Regex.Match(IsBlock, @"[a-zA-z]{4,5}");
                        IsBlock = IsBlock.Substring(match.Index, match.Length);
                    }
                }
                if (string.IsNullOrEmpty(IsBlock))
                    IsBlock = "NA";

                //get Listing
                string Listing = "NA";
                match = Regex.Match(Message, @"([\d]+[\s]listings)");
                if (match.Success)
                {
                    Listing = match.Value;
                    match = Regex.Match(Listing, @"^[\d]+");
                    Listing = match.Value;
                }
                if (string.IsNullOrEmpty(Listing))
                    Listing = "NA";

                //get ContentSize
                string ContentSize = "NA";
                if (MethodName.ToLower().Contains("ProcessRequest".ToLower()))
                {
                    match = Regex.Match(Message, @"(len [\d]+)");
                    if (match.Success)
                    {
                        ContentSize = match.Value;
                        match = Regex.Match(ContentSize, @"[\d]+");
                        ContentSize = match.Value;
                    }
                }
                else if (MethodName.ToLower().Contains("PreProcessContent".ToLower()))
                {
                    if (Message.ToLower().Contains("got empty content".ToLower()))
                    {
                        ContentSize = "0";
                    }
                }
                if (string.IsNullOrEmpty(ContentSize))
                    ContentSize = "NA";

                //get URL
                match = Regex.Match(Message, @"url[\s]?:");
                string URL = "NA";
                if (match.Success)
                {
                    URL = Message.Remove(0, match.Index + match.Length);
                    match = Regex.Match(URL, @"[a-zA-z]");
                    URL = URL.Remove(0, match.Index);
                }
                else
                {
                    match = Regex.Match(Message, @"url[\s]+");
                    if (match.Success)
                    {
                        URL = Message.Remove(0, match.Index + match.Length);
                        match = Regex.Match(URL, @"[a-zA-z]");
                        URL = URL.Remove(0, match.Index);
                        match = Regex.Match(URL, @"[\s]+");
                        if (match.Success)
                        {
                            URL = URL.Substring(0, match.Index).Trim();
                            if (URL.EndsWith(","))
                            {
                                URL = URL.Remove(URL.Length - 1);
                            }
                            URL = GetUrlDecodeString(URL);
                            URL = GetUrlDecodeString(URL);
                        }
                        //match = Regex.Match(URL, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
                        //if (!match.Success)
                        //{
                        //    URL = "NA";
                        //}
                    }
                }
                if (string.IsNullOrEmpty(URL))
                    URL = "NA";

                //Source_File_Name|Time|Number|ProcessID|Message_type|MethodName|RequestID|IsBlock|URL|Message
                Result = Source_File_Name + "|" + time + "|" + Number + "|" + ProcessID + "|" + Message_type + "|" + MethodName + "|" + RequestID + "|" + IsBlock + "|" + Listing + "|" + ContentSize + "|" + URL + "|" + Message;
                //await Task.Run(() =>
                //{
                //});
            }
            catch (Exception e)
            {
                //e.message;
            }

            return Result;
        }

        public static string GetUrlDecodeString(this string url)
        {
            if (!string.IsNullOrEmpty(url))
                return HttpUtility.UrlDecode(url, Encoding.UTF8);
            return null;
        }

        static List<string> ColumnName = new List<string>
        {
            //"Serial number",
            "Source_File_Name",
            "Time",
            "Number",
            "ProcessID",
            "Message_type",
            "MethodName",
            "RequestID",
            "IsBlock",
            "Listings",
            "ContentSize",
            "URL",
            "Message"
        };
        public static DataTable ConvertToDataTable(string filePath)
        {
            DataTable tbl = new DataTable();
            for (int i = 0; i < ColumnName.Count; i++)
            {
                if (ColumnName[i].Equals("Time"))
                {
                    tbl.Columns.Add(ColumnName[i], typeof(DateTime));
                }
                else
                    tbl.Columns.Add(ColumnName[i]);
            }

            ReadFile(filePath);
            for (int i = 1; i < FileLines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(FileLines[i]))
                {
                    continue;
                }
                var cols = FileLines[i].Split(new[] { '|' } , tbl.Columns.Count);

                DataRow dr = tbl.NewRow();
               // dr[0] = i;
                for (int cIndex = 0; cIndex < cols.Length; cIndex++)
                {
                    //dr[cIndex+1] = cols[cIndex];
                    dr[cIndex] = cols[cIndex];
                }

                tbl.Rows.Add(dr);
            }

            return tbl;
        }

        public static void ConvertDataTableToSortedFile(DataTable dt, string fileName)
        {
            try
            {
                string filePath = string.Format(@"{0}\LogDataFiles\{1}_{2}.log", Path.GetDirectoryName(Application.ExecutablePath), fileName, DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_fff"));

                string headerNames = string.Join("|", ColumnName.ToArray());

                StringBuilder sortedFileText = new StringBuilder(headerNames + Environment.NewLine);

                for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                {
                    sortedFileText.AppendLine(string.Join("|", dt.Rows[rowIndex].ItemArray.Cast<string>()));
                }

                File.WriteAllText(filePath, sortedFileText.ToString());

                MessageBox.Show(null, "Sorted file generated Successsfully!!", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, "Error occurred while generating Sorted file!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
