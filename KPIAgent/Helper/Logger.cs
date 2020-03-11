using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
namespace KPIAgent
{
    class Logger
    {
        private void Log(string log, string type)
        {
            FileStream objFile;
            StreamWriter objWriter;

            string datetime = "[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]";

            log = datetime + " " + type + " : " + log;

            try
            {
                //string foldername = "C:\\BOS KP Integration\\" + filename;

                string foldername = Application.StartupPath + "\\Logs\\";

                string filename = foldername + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }

                if (File.Exists(filename))
                {
                    objFile = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                else
                {
                    objFile = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write);
                }

                objWriter = new StreamWriter(objFile);

                objWriter.BaseStream.Seek(0, SeekOrigin.End);
                objWriter.WriteLine(log);
                objWriter.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Creating Logs: " + ex.ToString(), "Error");
            }

        }
        public void Info(string log)
        {
            try
            {
                string type = "[INFO]";

                Log(log, type);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Creating Logs: " + ex.ToString(), "Error");
            }
        }

        public void Error(string log)
        {
            try
            {
                string type = "[ERROR]";

                Log(log, type);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Creating Logs: " + ex.ToString(), "Error");
            }
        }
    }
}
