using AssetManagerPackage;
using AssetPackage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerProfilingAssetNameSpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace UnitTestPlayerProfiler
{
    [TestClass]
    public class TestCasePP
    {
        #region HelperMethods

        /// <summary>
        /// Logging functionality for the Tests
        /// </summary>
        /// <param name="msg"> Message to be logged </param>
        public void log(String msg, Severity severity = Severity.Information)
        {
            ILog logger = (ILog)AssetManager.Instance.Bridge;
            logger.Log(severity, "[PPA Test]" + msg);
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

            QuestionnaireGroup qg1 = new QuestionnaireGroup("A", "SUM/5");
            QuestionnaireGroup[] qga = { qg1 };
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
        /// Method for initializing the assets
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            if (AssetManager.Instance.findAssetsByClass("PlayerProfilingAsset").Count == 0)
            {
#warning change bridge implementation (in UnitTestPlayerProfiler/Bridge.cs) for testing (IDataStoragePath and ILog - logging behaviour)
                //Adding the bridge
                AssetManager.Instance.Bridge = new Bridge();

                //creating the asset
                PlayerProfilingAsset ppa = new PlayerProfilingAsset();
            }
        }

        /// <summary>
        /// Method creating example questionnaire data and outputting it.
        /// </summary>
        [TestMethod]
        public void performTest1()
        {
            Initialize();
            log("Start Test 1");
            try
            {
                QuestionnaireData qd = createExampleQuestionnaireData();
                log(qd.toXmlString());
                writeQuestionnaireDataToHTMLFile(qd, "QuestionnaireDataTest1.html");
            }
            catch
            {
                Assert.Fail();
            }
            log("End test 1");
        }

        /// <summary>
        /// Method loading Questionnaire data and outputting it.
        /// </summary>
        [TestMethod]
        public void performTest2()
        {
            log("Start Test 2");
            try
            {
                string fileId = getPPA().getQuestionnaireFileId();
                log("FileId for created HTML: " + fileId);
            }
            catch
            {
                Assert.Fail();
            }
            log("End test 2");
        }

        /// <summary>
        /// Method for testing answer-xml deserilization.
        /// </summary>
        [TestMethod]
        public void performTest3()
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
                Assert.Fail();
            }

            log("End test 3");
        }

        /// <summary>
        /// Method requesting and printing out the questionnaire xml.
        /// </summary>
        [TestMethod]
        public void performTest4()
        {
            log("Start Test 4");
            try
            {
                String xml = getPPA().getQuestionnaireXML();
                log("XML:\n" + xml);
            }
            catch
            {
                Assert.Fail();
            }
            log("End test 4");
        }

        /// <summary>
        /// Method requesting the questionnaire xml and returning the results locally.
        /// </summary>
        [TestMethod]
        public void performTest5()
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


            Assert.AreEqual((double) 7/5, results["A"]);

            log("End test 5");
        }


        [TestMethod]
        public void performTest6()
        {
            log("Start Test 6");
            setQuestionnaireXMLData(createExampleQuestionnaireData());
            String xml = getPPA().getQuestionnaireXML();

            try
            {
                QuestionnaireData qd = QuestionnaireData.getQuestionnaireData(xml);
                if (!xml.Equals(qd.toXmlString()))
                    Assert.Fail();
            }
            catch
            {
                Assert.Fail();
            }

            log("End Test 6");
        }

        #endregion TestMethods
    }


}
