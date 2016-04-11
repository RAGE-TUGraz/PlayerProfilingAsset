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
  Changed on: 2016-02-22
*/


using AssetManagerPackage;
using AssetPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PlayerProfilingAssetNameSpace
{
    /// <summary>
    /// Singelton Class for handling PlayerProfilingAsset actions.
    /// </summary>
    class PlayerProfilerHandler
    {

        #region Fields

        /// <summary>
        /// Instance of the PlayerProfilingAsset
        /// </summary>
        internal PlayerProfilingAsset playerProfilingAsset = null;

        /// <summary>
        /// Instance of the class PlayerProfilerHandler - Singelton pattern
        /// </summary>
        private static PlayerProfilerHandler instance;

        #endregion Fields
        #region Constructors

        /// <summary>
        /// private PlayerProfilerHandler-ctor for Singelton-pattern 
        /// </summary>
        private PlayerProfilerHandler() { }

        #endregion Constructors
        #region Properties

        /// <summary>
        /// Getter for Instance of the MotivationAdaptionHandler - Singelton pattern
        /// </summary>
        public static PlayerProfilerHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerProfilerHandler();
                }
                return instance;
            }
        }

        #endregion Properties
        #region InternalMethods

        /// <summary>
        /// Method returning an instance of the PlayerProfilingAsset.
        /// </summary>
        /// <returns> Instance of the PlayerProfilingAsset </returns>
        internal PlayerProfilingAsset getPPA()
        {
            if (playerProfilingAsset == null)
                playerProfilingAsset = (PlayerProfilingAsset)AssetManager.Instance.findAssetByClass("PlayerProfilingAsset");
            return (playerProfilingAsset);
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
                loggingPPA("Storing Questionnaire data to HTML File.");
                ids.Save(fileId, qd.toHTMLString("testid"));
            }
            else
                loggingPPA("No IDataStorage - Bridge implemented!", Severity.Warning);
        }

        /// <summary>
        /// Method for returning QuestionnaireData.
        /// </summary>
        /// <returns> QuestionnaireData - datastructure </returns>
        internal QuestionnaireData getQuestionnaireData()
        {
            string fileId = getPPA().getPPASettings().QuestionnaireDataXMLFileId;

            IDataStorage ids = (IDataStorage)AssetManager.Instance.Bridge;
            if (ids != null)
            {
                if (!ids.Exists(fileId))
                {
                    loggingPPA("File " + fileId + " not found for loading QuestionnaireData.", Severity.Error);
                    throw new Exception("EXCEPTION: File " + fileId + " not found for loading QuestionnaireData.");
                }

                loggingPPA("Loading QuestionnaireData from File.");
                return (this.getQuestionnaireDataFromXmlString(ids.Load(fileId)));
            }
            else
            {
                loggingPPA("IDataStorage bridge absent for requested local loading method of the QuestionnaireData.", Severity.Error);
                throw new Exception("EXCEPTION: IDataStorage bridge absent for requested local loading method of the QuestionnaireData.");
            }
            
        }

        /// <summary>
        /// gets the questionnaire data und converts it to a html - string
        /// </summary>
        /// <returns> HTML representation of the questionnaire data. </returns>
        internal string getHTMLQuestionnaire()
        {
            string htmlFileId = getPPA().getPPASettings().HTMLQuestionnaireFileId;
            this.writeQuestionnaireDataToHTMLFile(getQuestionnaireData(), htmlFileId);
            return htmlFileId;
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
            loggingPPA(str);
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireData));
            using (TextReader reader = new StringReader(str))
            {
                QuestionnaireData result = (QuestionnaireData)serializer.Deserialize(reader);
                return (result);
            }
        }

        #endregion InternalMethods
        #region TestMethods

        /// <summary>
        /// Method for logging (Diagnostics).
        /// </summary>
        /// 
        /// <param name="msg"> Message to be logged. </param>
        internal void loggingPPA(String msg, Severity severity = Severity.Information)
        {
            getPPA().Log(severity, "[PPA]: " + msg);
        }

        /// <summary>
        /// Method calling all Tests of this Class.
        /// </summary>
        internal void performAllTests()
        {
            loggingPPA("*****************************************************************");
            loggingPPA("Calling all tests (PlayerProfilingAsset):");
            performTest1();
            performTest2();
            loggingPPA("Tests PlayerProfilingAsset - done!");
            loggingPPA("*****************************************************************");
        }

        /// <summary>
        /// Method creating example questionnaire data and outputting it.
        /// </summary>
        internal void performTest1()
        {
            loggingPPA("Start Test 1");
            QuestionnaireData qd = createExampleQuestionnaireData();
            loggingPPA(qd.toXmlString());
            writeQuestionnaireDataToHTMLFile(qd,"QuestionnaireDataTest1.html");
            loggingPPA("End test 1");
        }

        /// <summary>
        /// Method loading Questionnaire data and outputting it.
        /// </summary>
        internal void performTest2()
        {
            loggingPPA("Start Test 2");
            //QuestionnaireData qd = createExampleQuestionnaireData();
            //writeQuestionnaireDataToXMLFile(qd,"QuestionnaireData");
            string fileId = getPPA().getQuestionnaireFileId();
            loggingPPA("FileId for created HTML: " + fileId);
            loggingPPA("End test 2");
        }

        /// <summary>
        /// Method creating an example Questionnaire datastructure for test purpose4s.
        /// </summary>
        /// <returns> example Questionnaire datastructure </returns>
        internal QuestionnaireData createExampleQuestionnaireData()
        {
            QuestionItem q1 = new QuestionItem(1,"How do you feel?");
            QuestionItem q2 = new QuestionItem(2,"Do you like games?");
            QuestionItem q3 = new QuestionItem(3, "Do you like fast cars?");
            QuestionItem q4 = new QuestionItem(4, "Do you enjoy silence?");
            QuestionItem q5 = new QuestionItem(5, "Do you like swimming?");
            QuestionItem[] qia = { q1,q2,q3,q4,q5};
            List<QuestionItem> qil = new List<QuestionItem>(qia);

            ChoiceItem c1 = new ChoiceItem(0,"Very good/much.");
            ChoiceItem c2 = new ChoiceItem(1, "I do not know.");
            ChoiceItem c3 = new ChoiceItem(2, "Very bad/Not very much.");
            ChoiceItem[] cia = { c1, c2, c3 };
            List<ChoiceItem> cil = new List<ChoiceItem>(cia);

            QuestionnaireData qd = new QuestionnaireData(qil,cil);
            qd.title = "Fancy questionnaire:";
            qd.instructions = "Please fill in the following form.";

            return qd;
        }

        /// <summary>
        /// Method for storing Questionnaire data as xml in a File.
        /// </summary>
        /// 
        /// <param name="qd"> Questionnaire data to store. </param>
        /// <param name="fileId"> String containing the file identification. </param>
        internal void writeQuestionnaireDataToXMLFile(QuestionnaireData qd, String fileId)
        {
            IDataStorage ids = (IDataStorage)AssetManager.Instance.Bridge;
            if (ids != null)
            {
                loggingPPA("Storing Questionnaire data to XML File.");
                ids.Save(fileId + ".xml", qd.toXmlString());
            }
            else
                loggingPPA("No IDataStorage - Bridge implemented!", Severity.Warning);
        }

        #endregion TestMethods
    }


    /// <summary>
    /// Classes for Serialization
    /// </summary>
    #region Serialization

    /// <summary>
    /// Classe containing all relevant data for questionnaire and evaluation of questionnaire
    /// </summary>
    [XmlRoot("questionnairedata")]
    public class QuestionnaireData
    {
        #region Fields
        /// <summary>
        /// Class storing all questions.
        /// </summary>
        [XmlElement("questionitemlist")]
        public QuestionItemList questionList { get; set; }

        /// <summary>
        /// Names the number of chuices on the Likert scala
        /// </summary>
        [XmlElement("choiceitemlist")]
        public ChoiceItemList choiceList { set; get; }

        /// <summary>
        /// Questionnaire title
        /// </summary>
        [XmlElement("title")]
        public string title { set; get; }

        /// <summary>
        /// Questionnaire instructions
        /// </summary>
        [XmlElement("instructions")]
        public string instructions { set; get; }


        #endregion Fields
        #region Constructors

        public QuestionnaireData() { }

        public QuestionnaireData(List<QuestionItem> ql, List<ChoiceItem> cl)
        {
            QuestionItemList qil = new QuestionItemList();
            qil.questionItemList = ql;
            this.questionList = qil;

            ChoiceItemList cil = new ChoiceItemList();
            cil.choiceItemList = cl;
            this.choiceList = cil;
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Method for converting a motivation model to a xml string.
        /// </summary>
        /// 
        ///<returns>
        /// A string representing the motivation model.
        /// </returns>
        internal String toXmlString()
        {
            try
            {
                var xmlserializer = new XmlSerializer(typeof(QuestionnaireData));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, this);
                    String xml = stringWriter.ToString();

                    return (xml);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <summary>
        /// Method for creating a html file from the data structure.
        /// </summary>
        /// <returns></returns>
        internal string toHTMLString(String questionnaireId)
        {
            string html = "<!DOCTYPE html>  <html>";

            html += createHTMLHead();

            //start of the body
            html += "<body>";
            html += createHTMLHeading();
            html += createLikertRating(this.choiceList.choiceItemList);
            foreach (QuestionItem qi in this.questionList.questionItemList)
                html += createHTMLQuestionItem(qi,this.choiceList.choiceItemList);
            html += createSubmissionButton();
            html += createSubmissionScript(this.questionList.questionItemList.Count, this.choiceList.choiceItemList.Count, questionnaireId);

            return html+"</body>\n</html>";
        }

        /// <summary>
        /// Method for writing the html head
        /// </summary>
        /// <returns> html head</returns>
        internal string createHTMLHead()
        {
            string head = "<head><meta charset='UTF-8'> <title>Questionnaire</title>\n<style type='text/css'>";
            head += "h1 {color:red;}";
            head += "div.question {float: left; width: 25%;}";
            head += "div.choice { float: left; width: "+ 75/this.choiceList.choiceItemList.Count +"%; }";
            head += "div.questionItem {}";
            head += "div.submit {text-align:center;}";
            return head+"</style>\n</head>\n";
        }

        /// <summary>
        /// Method for creating the heading for the html questionnaire file
        /// </summary>
        /// <returns> Html heading for the questionnaire.</returns>
        internal string createHTMLHeading()
        {
            string heading = "<div>";
            heading += @"<h1>"+this.title+"</h1>";
            heading+="<p>"+this.instructions+"</p>";

            return heading+"</div>\n";
        }

        /// <summary>
        /// Creates likert scale rating
        /// </summary>
        /// <returns> html representation of the rating</returns>
        internal string createLikertRating(List<ChoiceItem> cil)
        {
            string rating = "<div>";
            rating += "<div class='question'> &nbsp; </div>";
            foreach (ChoiceItem ci in cil)
                rating += "<div class='choice'> " + ci.choice + "</div>";
            return rating + "<br></div>\n";
        }

        /// <summary>
        /// Method for creating a html question item for the html questionnaire file.
        /// </summary>
        /// <param name="qi"> Question item, which should be represented in html.</param>
        /// <returns> html representation of the question item.</returns>
        internal string createHTMLQuestionItem(QuestionItem qi, List<ChoiceItem> cil)
        {
            string questionItem = "<div class='questionItem'>";
            questionItem += "<div class='question'>" + qi.question + "</div>" ;
            foreach(ChoiceItem ci in cil)
                questionItem += "<div class='choice'> <input type='radio' name='" + qi.questionId + "' value='"+ ci.position + "'></div>";

            return questionItem+"<br></div>\n";
        }

        internal string createSubmissionButton()
        {
            string button= @"<div class='submit' id='submitbutton'> <button type='button'>Submit!</button> </div>" +"\n";
            return button;
        }

        internal string createSubmissionScript(int numberOfQuestions, int numberOfChoices, String questionnaireId)
        {
            string script = "<script>";
            script += "document.getElementById('submitbutton').onclick=function(){\n";
            script += "var xml=getXMLContent();\n";
            script += "alert('submitting:'+ xml);\n";
            script += "};\n\n";

            script += "function findSelection(field){\n";
            script += "var elements = document.getElementsByName(field);\n";
            script += "for(var i=0; i<elements.length;i++){\n";
            script += "if(elements[i].checked==true){\n";
            script += "return(elements[i].value);\n";
            script += "}\n";
            script += "}\n";
            script += "return('-1');\n";
            script += "};\n\n;";

            script += "function getXMLContent(){\n";
            script += @"var xml = """";"+"\n";
            script += "xml += '<questionnaireanswers>';\n";
            script += "xml += '<questionnaireid>"+ questionnaireId + "</questionnaireid>';\n";
            script += "xml += '<answerlist>';\n";
            for(int i=1; i<= numberOfQuestions; i++)
            {
                script += "xml += '<answer>';\n";
                script += "xml += '<questionid>";
                script += i;
                script += "</questionid>';\n";
                script += "xml += '<answerid>'";
                script += @"+findSelection(""" + i + @""")";
                script += "+'</answerid>';\n";
                script += "xml += '</answer>';\n";
            }
            script += "xml += '</answerlist>';\n";
            script += "xml +='</questionnaireanswers>';\n";
            script += "return(xml);";
            script += "}\n";
            script += "</script>\n";
            return (script);
        }

        #endregion Methods
    }

    /// <summary>
    /// Containing list with all possible choices on the Likert scale
    /// </summary>
    public class ChoiceItemList
    {
        #region Fields
        /// <summary>
        /// Structure holding one choice item
        /// </summary>
        [XmlElement("choiceitem")]
        public List<ChoiceItem> choiceItemList { set; get; }
        #endregion Fields
        #region Constructors
        #endregion Constructors
    }

    /// <summary>
    /// Structure containing all possible liker scale choices
    /// </summary>
    public class ChoiceItem
    {
        #region Fields
        /// <summary>
        /// possible choice for the Likert scale
        /// </summary>
        [XmlElement("choice")]
        public string choice;

        /// <summary>
        /// according position for the item, starting with 0 (left)
        /// </summary>
        [XmlElement("position")]
        public int position;
        #endregion Fields
        #region Constructors
        public ChoiceItem() { }

        public ChoiceItem(int position, string choice)
        {
            this.position = position;
            this.choice = choice;
        }
        #endregion Constructors
    }

    /// <summary>
    /// Datatype containing list of all questions.
    /// </summary>
    public class QuestionItemList
    {
        #region Fields

        /// <summary>
        /// List of all questions.
        /// </summary>
        [XmlElement("questionitem")]
        public List<QuestionItem> questionItemList { get; set; }

        #endregion Fields
        #region Constructors

        public QuestionItemList() { }

        public QuestionItemList(List<QuestionItem> qil)
        {
            this.questionItemList = qil;
        }

        #endregion Constructors
    }

    /// <summary>
    /// A question from the questionnaire.
    /// </summary>
    public class QuestionItem
    {
        #region Fields

        /// <summary>
        /// Actual question of the question item.
        /// </summary>
        [XmlElement("question")]
        public string question { get; set; }

        /// <summary>
        /// unique ID of the question
        /// </summary>
        [XmlElement("questionid")]
        public int questionId { get; set; }

        #endregion Fields
        #region Constructors

        public QuestionItem() { }

        public QuestionItem(int id, string question)
        {
            this.questionId = id;
            this.question = question;
        }

        #endregion Constructors
    }

    #endregion Serilization
}
