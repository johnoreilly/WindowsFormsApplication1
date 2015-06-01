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
    class WekaWinder
    {
        ArrayList treeList = new ArrayList();
        ArrayList resultList = new ArrayList();
        
        String summary = "";
        public WekaWinder()
        {
            //constructor
        }

        public ArrayList doWekaBoost(string boost_file_path, string base_file_path,TextBox t)
        {
            ArrayList evaluation = new ArrayList();
            try
            {
                ArrayList treeListBoost = new ArrayList();
                weka.core.converters.CSVLoader csvLoader = new weka.core.converters.CSVLoader();
                weka.core.Instances trainData;
                t.Text = "";
                csvLoader.setSource(new java.io.File(boost_file_path));
                trainData = csvLoader.getDataSet();
                int cIdx = trainData.numAttributes() - 1;
                trainData.setClassIndex(cIdx);
                summary = trainData.toSummaryString();
                t.Text = t.Text + "\r\n" + summary;
                for (int i = 0; i < 5; i++)
                {
                    //Decision tree
                    weka.classifiers.Classifier j48 = new weka.classifiers.trees.J48();
                    j48.buildClassifier(trainData);
                    treeListBoost.Add(j48);
                    //build linear regression model
                    // LinearRegression model = new LinearRegression();
                    // model.buildClassifier(trainData);
                    //treeListBoost.Add(model);
                }
                evaluation = evaluateCycle(treeListBoost,base_file_path,t);
            }
            catch (Exception eX)
            {
                string message = "Somethings up" + eX.ToString();
                string caption = "Error Detected in WekaWinder method doWeka()";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
            return evaluation;
        }

        public ArrayList evaluateCycle(ArrayList treeListBoost,string file_path, TextBox t)
        {
            ArrayList evalArray = new ArrayList();
            try
            {
                weka.core.converters.CSVLoader LoaderTest = new weka.core.converters.CSVLoader();
                weka.core.Instances testing_data;
                LoaderTest.setSource(new java.io.File(file_path));
                testing_data = LoaderTest.getDataSet();
                int cIdx = testing_data.numAttributes() - 1;
                testing_data.setClassIndex(cIdx);
                int totalRightGuesses = 0;
                int totalWrongGuesses = 0;
                for (int i = 0; i < testing_data.numInstances(); i++)
                {
                    int right1 = 0;
                    int wrong1 = 0;
                    for (int j = 0; j < treeListBoost.Count; j++)     //iterate through the models & take a vote
                    {
                        string sGuess;
                        string sAnswer;
                        weka.classifiers.Classifier j48 = (weka.classifiers.Classifier)treeListBoost[j];
                        double pred = j48.classifyInstance(testing_data.instance(i));
                        sAnswer = testing_data.classAttribute().value(
                                        (int)testing_data.instance(i).classValue());
                        sGuess = testing_data.classAttribute().value((int)pred);
                        t.Text = t.Text + "actual value: "
                                + sAnswer + " " +
                        "predicted value: "
                                + sGuess + "\r\n";
                        if (ballotBoxRight1(sGuess, sAnswer))
                        {
                            right1++;
                        }
                        else
                        {
                            wrong1++;
                        }
                        if (right1 > wrong1)
                        {
                            totalRightGuesses++;
                        }
                        else
                        {
                            totalWrongGuesses++;
                        }
                    }
                    evalArray.Insert(i, totalWrongGuesses - totalRightGuesses);
                }
                t.Text = "evaluateCycle complete\r\n" + shortReport(totalRightGuesses, totalWrongGuesses) + t.Text;

            }
            catch (Exception eX)
            {
                string message = "Somethings up" + eX.ToString();
                string caption = "Error Detected in WekaWinder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
            return evalArray;
        }

        public void doWeka(int fileCount, string path, TextBox t)
        {
            String file_path = "";
            try
            {
                weka.core.converters.CSVLoader csvLoader = new weka.core.converters.CSVLoader();
                weka.core.Instances trainData;
                t.Text = "";
                for (int i = 0; i < fileCount - 2; i++)
                {
                    file_path = path + "/TrainingSet" + i + ".csv";
                    csvLoader.setSource(new java.io.File(file_path));
                    trainData = csvLoader.getDataSet();
                    int cIdx = trainData.numAttributes() - 1;
                    trainData.setClassIndex(cIdx);
                    summary = trainData.toSummaryString();
                    t.Text = t.Text + "\r\n" + summary;
                    weka.classifiers.Classifier j48 = new weka.classifiers.trees.J48();
                    j48.buildClassifier(trainData);
                    //build linear regression model
                   // LinearRegression model = new LinearRegression();
                   // model.buildClassifier(trainData);
                    //treeList.Add(model);
                    treeList.Add(j48);
                }
            }
            catch (Exception eX)
            {
                string message = "Somethings up" + eX.ToString();
                string caption = "Error Detected in WekaWinder method doWeka()";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        public void runAgainstTest(string path, TextBox t)
        {
            String file_path = "";
            t.Text = "";
            try
            {
                weka.core.converters.CSVLoader LoaderTest = new weka.core.converters.CSVLoader();
                weka.core.Instances testing_data;
                file_path = path + "/TestData1.csv";
                LoaderTest.setSource(new java.io.File(file_path));
                testing_data = LoaderTest.getDataSet();
                int cIdx = testing_data.numAttributes() - 1;
                testing_data.setClassIndex(cIdx);
                int totalRightGuesses=0;
                int totalWrongGuesses=0;
                for (int i = 0; i < testing_data.numInstances(); i++)
                {
                    int right1=0;
                    int wrong1=0;
                    for (int j = 0; j < treeList.Count; j++)     //iterate through the models & take a vote
                    {
                        string sGuess;
                        string sAnswer;
                        weka.classifiers.Classifier j48 = (weka.classifiers.Classifier)treeList[j];
                        double pred = j48.classifyInstance(testing_data.instance(i));
                        sAnswer = testing_data.classAttribute().value(
                                        (int)testing_data.instance(i).classValue());
                        sGuess = testing_data.classAttribute().value((int)pred);
                        t.Text = t.Text + "actual value: "
                                + sAnswer + " " +
                        "predicted value: "
                                + sGuess + "\r\n";
                        if(ballotBoxRight1(sGuess,sAnswer)){
                            right1++;
                        }else{
                            wrong1++;
                        }
                    }
                    if(right1>wrong1){
                        totalRightGuesses++;
                    }else{
                        totalWrongGuesses++;
                    }
                }
                t.Text = shortReport(totalRightGuesses, totalWrongGuesses) + t.Text;
            }
            catch (Exception eX)
            {
                string message = "Somethings up" + eX.ToString();
                string caption = "Error Detected in WekaWinder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        private bool ballotBoxRight1(string guess, string answer)
        {
            bool result = false;
            if (guess == answer) result = true;
            return result;
        }

        private bool voteRight(int totalRight, int totalWrong)
        {
            bool right = false;
            if (totalRight>totalWrong) right = true;
            return right;
        }

        private string shortReport(int right, int wrong)
        {
            string result = "";
            int percent = 0;
            right++;
            wrong++;
            string r = "Right guesses: " + right + "\r\n " +
                "Wrong guesses: " + wrong +"\r\n";
            string tf = "";
            if (right > wrong){
                tf = "Model mostly right";
                percent = (int)(100.0 * wrong) / right;
            }else{
                tf = "Model mostly wrong";
                percent = (int)(100.0 * right) / wrong;
            }
            result=tf + "\r\n" + r + percent + "%\r\n";
            return result;
        }

    }
}
