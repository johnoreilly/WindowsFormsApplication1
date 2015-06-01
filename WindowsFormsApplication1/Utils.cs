using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class Utils
    {
        private ArrayList ListAll = new ArrayList();
        private ArrayList ListTrain = new ArrayList();
        private ArrayList ListTest = new ArrayList();
        private string tempDirPath = @"C:\\Users\\John\\Documents\\DIT\\ML\\Assignments\\temp";
        private int headerLength = 0;


        public string getTempDirPath
        {
            get
            {
                return tempDirPath;
            }
        }

        public Utils(int h,ArrayList All)
        {
            this.ListAll.AddRange(All);
        }

        public Utils(ArrayList TrainingSet,ArrayList TestSet,int h)
        {
            this.ListTrain.AddRange(TrainingSet);
            this.ListTest.AddRange(TestSet);
            this.headerLength = h;
        }

        public bool bagPacker()
        {
            try
            {
                bool done = false;
                writeToFile(1, ListTest, "TestData");
                for (int i = 0; i < 11; i++)
                {
                    done = false;
                    ArrayList tempList = ShuffleList();
                    writeToFile(i, tempList,"TrainingSet");
                    done = true;
                }
                return done;
            }
            catch (Exception eX)
            {
                string message = "Somethings up" + eX.ToString();
                string caption = "Error Detected in Bag Packer";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
                return false;
            }
        }

        private void writeToFile(int j, ArrayList tempList,String fileName)
        {
            string n = Convert.ToString(j);
            String header = "Attribute0,";
            String classifier = "Classifier";
            for (int i = 0; i < headerLength-2; i++)
            {
                header = header + "Attribute" + (i + 1) + ",";
            }
            header = header + classifier;
            using (StreamWriter theWriter = new StreamWriter(tempDirPath + "\\" + fileName + n + ".csv"))
            {
                theWriter.WriteLine(header);
                foreach (string s in tempList)
                {
                    theWriter.WriteLine(s);
                }
                theWriter.Close();
            }
        }

        private ArrayList ShuffleList()
        {
            ArrayList randomList = new ArrayList();
            Random r = new Random();
            int randomIndex = 0;
            for (int i = 0; i < ListTrain.Count; i++)
            {
                randomIndex = r.Next(0, ListTrain.Count);
                randomList.Add(ListTrain[randomIndex]); 
            }
            return randomList;
        }

        public string[] showFiles()
        {
            string [] fileEntries = Directory.GetFiles(tempDirPath)
            .Select(path => Path.GetFileName(path))
                                     .ToArray();
            return fileEntries;
        }


        public string writeBaseFile(int headerLength,ArrayList BoostList)
        {
            String header = "Attribute0,";
            String classifier = "Classifier";
            string file_path = tempDirPath + "\\BaseFile.csv";
            for (int i = 0; i < headerLength - 2; i++)
            {
                header = header + "Attribute" + (i + 1) + ",";
            }
            header = header + classifier;
            using (StreamWriter theWriter = new StreamWriter(file_path))
            {
                theWriter.WriteLine(header);
                foreach (string s in BoostList)
                {
                    theWriter.WriteLine(s);
                }
                theWriter.Close();
            } 
            return file_path;
        }


        public ArrayList generateBoostFile(ArrayList BoostList)
        {
            ArrayList BoostedFile = new ArrayList();
            ArrayList WeightedFile = new ArrayList();
            for (int i = 0; i < BoostList.Count; i++)  //list of weights
            {
                int j = (int)BoostList[i];      //get the weight for each item
                for (int k = 1; k < j + 1; k++)
                {
                    BoostedFile.Add(ListAll[i]); //add so many to the boosted file so weighting the random selection
                }
            }

            return WeightedFile;
        }

        public ArrayList selectWithBoosting(ArrayList BoostList)
        {
            ArrayList randomList = new ArrayList();
            ArrayList SelectedList = new ArrayList();
            Random r = new Random();
            int randomIndex = 0;
            for (int i = 0; i < ListAll.Count;i++ )  //just do it for as many items as in original data
            {
                randomIndex = r.Next(0, BoostList.Count); //Choose a random object in the list
                randomList.Add(randomIndex); //add it to the new, random list
            }
            foreach (int j in randomList)
            {
                SelectedList.Add(ListAll[j]);       //choose items selected earlier for the weighted list
            }
            return SelectedList; //return the new random list
        }

        public string writeBoostFile(int headerLength, ArrayList BoostList)
        {
            String header = "Attribute0,";
            String classifier = "Classifier";
            string file_path = tempDirPath + "\\BoostFile.csv";
            for (int i = 0; i < headerLength - 2; i++)
            {
                header = header + "Attribute" + (i + 1) + ",";
            }
            header = header + classifier;
            using (StreamWriter theWriter = new StreamWriter(file_path))
            {
                theWriter.WriteLine(header);
                foreach (string s in BoostList)
                {
                    theWriter.WriteLine(s);
                }
                theWriter.Close();
            }
            return file_path;
        }
    }
}
