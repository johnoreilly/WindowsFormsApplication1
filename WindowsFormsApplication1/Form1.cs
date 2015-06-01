using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;
using weka;



namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        ArrayList ListALL = new ArrayList();
        ArrayList ListTrain = new ArrayList();
        ArrayList ListTest = new ArrayList();
        int countFiles = 0;
        string path = "";
        int headerLength = 0;
        WekaWinder weka;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".csv";
                dlg.Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xls)|*.xls";
                dlg.InitialDirectory = "C:\\Users\\John\\Documents\\DIT\\ML\\Assignments";
                dlg.Title = "Open Data File";
                DialogResult result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string filePath = dlg.FileName;
                    string filename = dlg.SafeFileName;
                    label1.Text = filename;
                    string pathToDir = System.IO.Path.GetDirectoryName(dlg.FileName);
                    PopulateGridViewWithCSVFile(filename, pathToDir);
                    try
                    {
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            String line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                ListALL.Add(line);
                                string[] sp = line.Split(new Char[] { ',' });
                                headerLength = sp.Length;
                            }
                            int fLength = ListALL.Count;
                            string message = "File Read OK " + fLength + " Rows";
                            label2.Text = message;
                            button1.Enabled = false;
                            button1.Text = "Done";
                            splitDataTestTrain(fLength);
                        }
                    }
                    catch (Exception eX)
                    {
                        string message = "The file could not be read" + eX.ToString();
                        string caption = "Error Detected in Input";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        result = MessageBox.Show(message, caption, buttons);
                    }
                }
        }
        
        private void PopulateGridViewWithCSVFile(string csvFileName, string pathDataFile)
        {
            string conString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + pathDataFile + ";Extensions=asc,csv,tab,txt";
            DataSet ds = new DataSet("MyDataSet");
            OdbcDataAdapter adp = new OdbcDataAdapter();
            using (OdbcConnection conn = new OdbcConnection(conString))
            {
                using (OdbcCommand cmd = new OdbcCommand("SELECT * FROM " + csvFileName, conn))
                {
                    conn.Open();
                    adp.SelectCommand = cmd;
                    adp.Fill(ds);
                }
                conn.Close();
            }
            dataGridView1.DataSource = ds.Tables[0];//
        }

        private void splitDataTestTrain(int fLength)
        {
            int testSetLength = (int)(fLength * 0.3);   //30% for test set cast to int
            ArrayList newList = new ArrayList(ListALL);
            Random r = new Random();
            int randomIndex = 0;
            for(int i=0;i<testSetLength;i++)
            {
                randomIndex = r.Next(0, newList.Count); 
                ListTest.Add(newList[randomIndex]); // load test data set from this list of 30% original
                newList.RemoveAt(randomIndex);      // then remove it from training data list
            }
            ListTrain.AddRange(newList);
        }

        private List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();
            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                //don't remove to implement replacement
            }
            return randomList; //return the new random list
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Utils assistant = new Utils(ListTrain,ListTest,headerLength);
            bool bagsPacked = assistant.bagPacker();
            string[] fileEntries = assistant.showFiles();
            foreach (string fileName in fileEntries)
            {
                textBox1.Text =  textBox1.Text + fileName + "\r\n";
            }
            countFiles = fileEntries.Length;
            label3.Text = countFiles + " Files Created ";
            path = assistant.getTempDirPath;
            path = path.Replace(@"\\",@"/");
            label4.Text = path;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WekaWinder ww = new WekaWinder();
            ww.doWeka(countFiles, path, this.textBox1);
            this.weka = ww;
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            weka.runAgainstTest(path,this.textBox1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string Base_filePath;
            string Boost_filePath;
            ArrayList BoostedList = new ArrayList();
            ArrayList SelectedList = new ArrayList();
            Utils assistant = new Utils(headerLength,ListALL);
            Base_filePath=assistant.writeBaseFile(headerLength,ListALL);
            Boost_filePath=assistant.writeBoostFile(headerLength, ListALL); //initial boost file just a straight copy of original data
            WekaWinder ww = new WekaWinder();
            for(int i=0;i<5;i++){       //do it a few times
               BoostedList = ww.doWekaBoost(Boost_filePath, Base_filePath, this.textBox3);
               SelectedList = assistant.selectWithBoosting(BoostedList);
               Boost_filePath=assistant.writeBoostFile(headerLength, SelectedList);          //this is the boosted one
            }
            
        }

    }
}
