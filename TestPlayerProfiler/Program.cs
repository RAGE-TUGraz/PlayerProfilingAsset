/*
  Copyright 2016 TUGraz, http://www.tugraz.at/
  
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  This project has received funding from the European Union’s Horizon
  2020 research and innovation programme under grant agreement No 644187.
  You may obtain a copy of the License at
  
      http://www.apache.org/licenses/LICENSE-2.0
  
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
  
  This software has been created in the context of the EU-funded RAGE project.
  Realising and Applied Gaming Eco-System (RAGE), Grant agreement No 644187, 
  http://rageproject.eu/

  Development was done by Cognitive Science Section (CSS) 
  at Knowledge Technologies Institute (KTI)at Graz University of Technology (TUGraz).
  http://kti.tugraz.at/css/

  Created by: Matthias Maurer, TUGraz <mmaurer@tugraz.at>
  Changed by: Matthias Maurer, TUGraz <mmaurer@tugraz.at>
*/

using AssetManagerPackage;
using AssetPackage;
using PlayerProfilingAssetNameSpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using UnitTestPlayerProfiler;

namespace TestPlayerProfiler
{
    class Program
    {
        static void Main(string[] args)
        {
            AssetManager am = AssetManager.Instance;
            am.Bridge = new Bridge();

            PlayerProfilingAsset ppa = new PlayerProfilingAsset();


            TestPlayProfilingAsset tppa = new TestPlayProfilingAsset();
            tppa.performAllTests();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

        }
        
    }

    class TestPlayProfilingAsset
    {
        #region HelperMethods

        /// <summary>
        /// Logging functionality for the Tests
        /// </summary>
        /// <param name="msg"> Message to be logged </param>
        public void log(String msg, Severity severity = Severity.Information)
        {
            AssetManager.Instance.Log(severity, "[PPA Test]: {0}", msg);
        }

        /// <summary>
        /// Method returning theAsset
        /// </summary>
        /// <returns> The Asset</returns>
        public PlayerProfilingAsset getPPA()
        {
            return (PlayerProfilingAsset)AssetManager.Instance.findAssetByClass("PlayerProfilingAsset");
        }

        /// <summary>
        /// Method creating an example Questionnaire datastructure for test purpose4s.
        /// </summary>
        /// <returns> example Questionnaire datastructure </returns>
        internal QuestionnaireData createExampleQuestionnaireData()
        {
            QuestionItem q1 = new QuestionItem(1, "How do you feel?");
            q1.group = "A";
            QuestionItem q2 = new QuestionItem(2, "Do you like games?");
            q2.group = "A";
            QuestionItem q3 = new QuestionItem(3, "Do you like fast cars?");
            q3.group = "A";
            QuestionItem q4 = new QuestionItem(4, "Do you enjoy silence?");
            q4.group = "A";
            QuestionItem q5 = new QuestionItem(5, "Do you like swimming?");
            q5.group = "A";
            QuestionItem[] qia = { q1, q2, q3, q4, q5 };
            List<QuestionItem> qil = new List<QuestionItem>(qia);

            ChoiceItem c1 = new ChoiceItem(0, "Very good/much.");
            ChoiceItem c2 = new ChoiceItem(1, "I do not know.");
            ChoiceItem c3 = new ChoiceItem(2, "Very bad/Not very much.");
            ChoiceItem[] cia = { c1, c2, c3 };
            List<ChoiceItem> cil = new List<ChoiceItem>(cia);

            QuestionnaireData qd = new QuestionnaireData(qil, cil);
            qd.title = "Fancy questionnaire:";
            qd.instructions = "Please fill in the following form.";
            qd.groupList = new QuestionnaireGroupList();

            QuestionnaireGroup qg1 = new QuestionnaireGroup("A","SUM/5");
            QuestionnaireGroup[] qga = { qg1};
            qd.groupList.groups = new List<QuestionnaireGroup>(qga);

            return qd;
        }

        /// <summary>
        /// Method for storing Questionnaire data as html in a File.
        /// </summary>
        /// 
        /// <param name="qd"> Questionnaire data to store. </param>
        /// <param name="fileId"> String containing the file identification. </param>
        internal void writeQuestionnaireDataToHTMLFile(QuestionnaireData qd, String fileId)
        {
            IDataStorage ids = (IDataStorage)AssetManager.Instance.Bridge;
            if (ids != null)
            {
                log("Storing Questionnaire data to HTML File.");
                //testid given from program!
                ids.Save(fileId, qd.toHTMLString("testid"));
            }
            else
                log("No IDataStorage - Bridge implemented!", Severity.Warning);
        }

        /// <summary>
        /// Method for deserialization of a XML-String to the coressponding QuestionnaireAnswerData.
        /// </summary>
        /// 
        /// <param name="str"> String containing the XML-QuestionnaireAnswerData for deserialization. </param>
        ///
        /// <returns>
        /// QuestionnaireAnswerData-type coressponding to the parameter "str" after deserialization.
        /// </returns>
        internal QuestionnaireAnswerData getQuestionnaireAnswerDataFromXmlString(String str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireAnswerData));
            using (TextReader reader = new StringReader(str))
            {
                QuestionnaireAnswerData result = (QuestionnaireAnswerData)serializer.Deserialize(reader);
                return (result);
            }
        }

        /// <summary>
        /// Method for deserialization of a XML-String to the coressponding QuestionnaireData.
        /// </summary>
        /// 
        /// <param name="str"> String containing the XML-QuestionnaireData for deserialization. </param>
        ///
        /// <returns>
        /// QuestionnaireData-type coressponding to the parameter "str" after deserialization.
        /// </returns>
        internal QuestionnaireData getQuestionnaireDataFromXmlString(String str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireData));
            using (TextReader reader = new StringReader(str))
            {
                QuestionnaireData result = (QuestionnaireData)serializer.Deserialize(reader);
                return (result);
            }
        }

        internal void setQuestionnaireXMLData(QuestionnaireData qd)
        {
            string fileId = "QuestionnaireDataFileIdTest.xml";

            //store dm to file
            IDataStorage ids = (IDataStorage)AssetManager.Instance.Bridge;
            if (ids != null)
            {
                log("Storing DomainModel to File.");
                ids.Save(fileId, qd.toXmlString());
            }
            else
                log("No IDataStorage - Bridge implemented!", Severity.Warning);

            PlayerProfilingAssetSettings ppas = new PlayerProfilingAssetSettings();
            ppas.QuestionnaireDataXMLFileId = fileId;
            getPPA().Settings = ppas;
        }

        #endregion HelperMethods
        #region TestMethods

        /// <summary>
        /// Method calling all Tests of this Class.
        /// </summary>
        internal void performAllTests()
        {
            log("*****************************************************************");
            log("Calling all tests (PlayerProfilingAsset):");
            performTest1();
            performTest2();
            performTest3();
            performTest4();
            performTest5();
            log("Tests PlayerProfilingAsset - done!");
            log("*****************************************************************");
        }

        /// <summary>
        /// Method creating example questionnaire data and outputting it.
        /// </summary>
        internal void performTest1()
        {
            log("Start Test 1");
            QuestionnaireData qd = createExampleQuestionnaireData();
            log(qd.toXmlString());
            writeQuestionnaireDataToHTMLFile(qd, "QuestionnaireDataTest1.html");
            log("End test 1");
        }

        /// <summary>
        /// Method loading Questionnaire data and outputting it.
        /// </summary>
        internal void performTest2()
        {
            log("Start Test 2");
            //QuestionnaireData qd = createExampleQuestionnaireData();
            //writeQuestionnaireDataToXMLFile(qd,"QuestionnaireData");
            setQuestionnaireXMLData(createExampleQuestionnaireData());
            string fileId = getPPA().getQuestionnaireFileId();
            log("FileId for created HTML: " + fileId);
            log("End test 2");
        }

        /// <summary>
        /// Method for testing answer-xml deserilization.
        /// </summary>
        internal void performTest3()
        {
            log("Start Test 3");
            String xmlAnswer = @"<?xml version=""1.0"" encoding=""utf-16""?><questionnaireanswers xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">";
            xmlAnswer += "<groups><group><name>A</name><rating>7</rating></group><group><name>B</name><rating>8</rating></group></groups>";
            xmlAnswer += "<questionnaireid>testid</questionnaireid></questionnaireanswers>";

            QuestionnaireAnswerData qad = this.getQuestionnaireAnswerDataFromXmlString(xmlAnswer);

            if (qad.toXmlString().Equals(xmlAnswer))
            {
                log("Serilization and deserilization successful!");
            }
            else
            {
                log("Serilization and deserilization failed!");
                log(qad.toXmlString());
                log(xmlAnswer);
            }

            log("End test 3");
        }

        /// <summary>
        /// Method requesting and printing out the questionnaire xml.
        /// </summary>
        internal void performTest4()
        {
            log("Start Test 4");
            setQuestionnaireXMLData(createExampleQuestionnaireData());
            String xml = getPPA().getQuestionnaireXML();
            log("XML:\n" + xml);
            log("End test 4");
        }

        /// <summary>
        /// Method requesting the questionnaire xml and returning the results locally.
        /// </summary>
        internal void performTest5()
        {
            log("Start Test 5");
            setQuestionnaireXMLData(createExampleQuestionnaireData());
            String xml = getPPA().getQuestionnaireXML();

            QuestionnaireData qd = getQuestionnaireDataFromXmlString(xml);
            int numberOfChoices = qd.choiceList.choiceItemList.Count;
            Dictionary<string, int> answers = new Dictionary<string, int>();
            int i = 0;
            foreach (QuestionItem qi in qd.questionList.questionItemList)
            {
                if (i % 2 == 0)
                    answers.Add(qi.question, numberOfChoices - 2);
                else
                    answers.Add(qi.question, numberOfChoices - 1);
                i++;
            }
            getPPA().setQuestionnaireAnswers(answers);

            Dictionary<string, double> results = getPPA().getResults();
            foreach (String groupName in results.Keys)
                log(groupName + ": " + results[groupName]);
            log("A" + ": " + results["A"]);
            log("End test 5");
        }


        #endregion TestMethods
    }

}
