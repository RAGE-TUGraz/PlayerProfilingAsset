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

        /// <summary>
        /// Dictionary holding group-evaluation results after the evaluation of the questionnaire
        /// </summary>
        internal Dictionary<string,double> questionnaireResults = null;

        #endregion Fields
        #region Constructors

        /// <summary>
        /// private PlayerProfilerHandler-ctor for Singelton-pattern 
        /// </summary>
        private PlayerProfilerHandler() { }

        #endregion Constructors
        #region Properties

        /// <summary>
        /// Getter for Instance of the PlayerProfilingAssetHandler - Singelton pattern
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
                //testid given from program!
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
            XmlSerializer serializer = new XmlSerializer(typeof(QuestionnaireData));
            using (TextReader reader = new StringReader(str))
            {
                QuestionnaireData result = (QuestionnaireData)serializer.Deserialize(reader);
                return (result);
            }
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
        /// Method for retrieving the game storage - results of the questionnaire
        /// </summary>
        /// <returns> Dictionary containing the group as key and the value of the questionnaire </returns>
        internal Dictionary<string,double> getQuestionnaireResultFromGameStorage()
        {
            throw new NotImplementedException();
            /*
            String xmlStringQuestionnaireResults = "";
            questionnaireResults = getQuestionnaireAnswerDataFromXmlString(xmlStringQuestionnaireResults).getResults();
            return questionnaireResults;
            */
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

        /*
        /// <summary>
        /// Method creating an example Questionnaire datastructure for test purpose4s.
        /// </summary>
        /// <returns> example Questionnaire datastructure </returns>
        internal QuestionnaireData createExampleQuestionnaireData()
        {
            QuestionItem q1 = new QuestionItem(1, "How do you feel?");
            QuestionItem q2 = new QuestionItem(2, "Do you like games?");
            QuestionItem q3 = new QuestionItem(3, "Do you like fast cars?");
            QuestionItem q4 = new QuestionItem(4, "Do you enjoy silence?");
            QuestionItem q5 = new QuestionItem(5, "Do you like swimming?");
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
        */

        #endregion TestMethods
    }


    /// <summary>
    /// Classes for Serialization
    /// </summary>
    #region SerializationQuestionnaire

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

        /// <summary>
        /// Question groups
        /// </summary>
        [XmlElement("groups")]
        public QuestionnaireGroupList groupList { set; get; }


        /// <summary>
        /// default evaluation formula
        /// </summary>
        [XmlElement("defaultgroupformula")]
        public string defaultFormula { set; get; }

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

            this.groupList = new QuestionnaireGroupList();
            this.defaultFormula = "SUM";
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Method for checking if supplied answers have the correct format and to calculate the indicators.
        /// </summary>
        /// <param name="answers"> Dictionary containing answers and questions of the questionnaire.</param>
        /// <returns> True, if the data has the correct format - false otherwise.</returns>
        internal Boolean checkAnswerData(Dictionary<String, int> answers)
        {
            if(this.questionList.questionItemList.Count != answers.Count)
                return false;

            foreach(QuestionItem qi in questionList.questionItemList)
                if (!answers.ContainsKey(qi.question))
                    return false;

            //answer number ranging from 0 to maxIntRange
            int maxIntRange = 0;
            foreach (ChoiceItem ci in this.choiceList.choiceItemList)
                if (ci.position > maxIntRange)
                    maxIntRange = ci.position;

            //check if values are in the correct range
            foreach (QuestionItem qi in questionList.questionItemList)
                if (answers[qi.question] < 0 || answers[qi.question] > maxIntRange)
                    return false;

            //do calculation
            Dictionary<String, int> groupSums = new Dictionary<string, int>();
            Dictionary<String, String> groupFormulas = new Dictionary<string, string>();
            Dictionary<String, double> groupResult = new Dictionary<string, Double>();
            foreach (QuestionnaireGroup group in this.groupList.groups)
            {
                groupSums.Add(group.name, 0);
                groupFormulas.Add(group.name,group.formula);
            }

            foreach (QuestionItem qi in this.questionList.questionItemList)
                groupSums[qi.group] += getQuestionMappingFromAnswerId(qi.question,answers[qi.question]);

            foreach(QuestionnaireGroup group in this.groupList.groups)
            {
                groupFormulas[group.name] = groupFormulas[group.name].Replace("SUM", groupSums[group.name].ToString());
                groupResult.Add(group.name, MathInterpreter.eval(groupFormulas[group.name]));
            }


            PlayerProfilerHandler.Instance.questionnaireResults = groupResult;

            return true;
        }

        /// <summary>
        /// Method for getting the mapped value of a answer to a specific question
        /// </summary>
        /// 
        /// <param name="question">Specific question</param>
        /// <param name="answerId">Specific answer</param>
        /// <returns> Mapped value.</returns>
        internal int getQuestionMappingFromAnswerId(String question, int answerId)
        {
            AnswerMappingList relevantAnswerMappingList = null;
            foreach (QuestionItem qi in this.questionList.questionItemList)
                if (qi.question == question)
                {
                    relevantAnswerMappingList = qi.answerMappingList;
                    break;
                }

            foreach (AnswerMap am in relevantAnswerMappingList.answerMap)
                if (am.answerPosition == answerId)
                    return am.mappedValue;
            
            return 0;
        }

        /// <summary>
        /// Method for converting a motivation model to a xml string.
        /// </summary>
        /// 
        ///<returns>
        /// A string representing the motivation model.
        /// </returns>
        public String toXmlString()
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
        public string toHTMLString(String questionnaireId)
        {
            string html = "<!DOCTYPE html>  <html>";

            html += createHTMLHead();

            //start of the body
            html += "<body>";
            html += createHTMLHeading();
            html += createLikertRating(this.choiceList.choiceItemList);
            foreach (QuestionItem qi in this.questionList.questionItemList)
                html += createHTMLQuestionItem(qi, this.choiceList.choiceItemList);
            html += createSubmissionButton();
            html += createSubmissionScript(this.defaultFormula, this.groupList.groups ,this.questionList.questionItemList, this.choiceList.choiceItemList.Count, questionnaireId);

            return html + "</body>\n</html>";
        }

        /// <summary>
        /// Method for writing the html head
        /// </summary>
        /// <returns> html head</returns>
        internal string createHTMLHead()
        {
            string head = "<head><meta charset='UTF-8'> <title>Questionnaire</title>\n<style type='text/css'>";
            head += "h1 {color:black; text-align: center; }";
            head += "div.question {display:inline-block; width: 25%;}";
            head += "div.choice {display:inline-block; width: " + 75 / this.choiceList.choiceItemList.Count + "%; }";
            head += "div.questionItem {}";
            head += "div.submit {text-align:center;}";
            return head + "</style>\n</head>\n";
        }

        /// <summary>
        /// Method for creating the heading for the html questionnaire file
        /// </summary>
        /// <returns> Html heading for the questionnaire.</returns>
        internal string createHTMLHeading()
        {
            string heading = "<div>";
            heading += @"<h1>" + this.title + "</h1>";
            heading += "<p>" + this.instructions + "</p>";

            return heading + "</div>\n";
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
            string questionItem = "<div id='qi-"+ qi.questionId+"' class='questionItem";
            if (qi.group != null && qi.group != "")
                questionItem += " "+qi.group;
            questionItem += "'>";
            questionItem += "<div class='question'>" + qi.question + "</div>";
            foreach (ChoiceItem ci in cil)
            {   
                questionItem += "<div class='choice'> <input type='radio' name='" + qi.questionId + "' value='" + qi.map(ci.position) + "'></div>";
            }

            return questionItem + "<br></div>\n";
        }

        internal string createSubmissionButton()
        {
            string button = @"<div class='submit' id='submitbutton'> <button type='button'>Submit!</button> </div>" + "\n";
            return button;
        }

        internal string createSubmissionScript(String defaultGroupFormula, List<QuestionnaireGroup> groupList, List<QuestionItem> questionItemList, int numberOfChoices, String questionnaireId)
        {
            string script = "<script>\n";
            script += "groupnames=[];\n";
            script += "groupformulas=[];\n";
            script += "groupvalues=[];\n\n";

            script += "document.getElementById('submitbutton').onclick=function(){\n";
            script += "  if(done()){\n";
            script += "    calculatGroupValue();\n";
            script += "    var xml=getXMLContent();\n";
            script += "    submit(xml);\n";
            script += "  }else{\n";
            script += "    alert('Please answer all questions!');";
            script += "  };\n";
            script += "};\n\n";


            //method for checking if all questions are answered
            script += "function done(){\n";
            script += "  var elements = document.getElementsByClassName('questionItem');\n";
            script += "  for(var i=0;i<elements.length;i++){\n";
            script += "    var id = elements[i].id.substring(3,elements[i].id.length);\n";
            script += "    if(findSelection(id)=='')\n";
            script += "      return(false);\n";
            script += "  };\n";
            script += "  return(true);\n";
            script += "};\n\n";

            //method returning the answer value to given question item id
            script += "function findSelection(id){\n";
            script += "  var elements = document.getElementsByName(id);\n";
            script += "  for(var i=0; i<elements.length;i++){\n";
            script += "    if(elements[i].checked==true){\n";
            script += "      return(elements[i].value);\n";
            script += "    }\n";
            script += "  }\n";
            script += "  return('');\n";
            script += "};\n\n";

            //Method storing all groups into the array groupnames
            script += "function getGroups(){\n";
            script += "  groupnames=[];\n";
            script += "  groupformulas=[];\n";
            script += "  groupvalues = [];\n";
            List<String> alreadySeen = new List<string>();
            for(int i=0; i < questionItemList.Count; i++)
            {
                if (alreadySeen.Contains(questionItemList[i].group))
                    continue;
                alreadySeen.Add(questionItemList[i].group);
                script += "  groupnames.push('" + questionItemList[i].group + "');\n";
                script += "  groupvalues.push(0);\n";
                script += "  groupformulas.push('"+this.getFormulaByGroupName(questionItemList[i].group) +"');\n";
            }
            script += "};\n\n";

            //method for calculating the sum over all values of one group
            script += "function calculateGroupSum(){\n";
            script += "  getGroups();\n";
            script += "  var elements = document.getElementsByClassName('questionItem');\n";
            script += "  for(var i=0;i<elements.length;i++){\n";
            script += "    var id = elements[i].id.substring(3,elements[i].id.length);\n";
            script += "    var group='';\n";
            script += "    if(elements[i].className.length > 13)\n";
            script += "        group = elements[i].className.substring(13,elements[i].className.length);\n";
            script += "    groupvalues[groupnames.indexOf(group)] += parseInt(findSelection(id));\n";
            script += "  };\n";
            script += "};\n\n";

            //method for calculating the ratings for each group
            script += "function calculatGroupValue(){\n";
            script += "  calculateGroupSum();\n";
            script += "  for(var i=0;i<groupvalues.length;i++){\n";
            script += "    result = eval(groupformulas[i].replace(/SUM/g,groupvalues[i]));\n";
            script += "    groupvalues[i]=result;\n";
            script += "  };\n";
            script += "};\n\n";


            //math interpretet for *,/,+,-
            script += "function eval(term){\n";
            script += "  term = term.replace(/:/g,'/');\n";
            script += "  var numbers='0123456789';\n";
            script += "  var operators='-+*/';\n";
            //give back number (negative/positive)
            script += "  var onlyNumbersSoFar = true;\n";
            script += "  for(var i=0;i<term.length;i++){\n";
            script += "    if(numbers.indexOf(term[i])>-1 || (i==0  && term[i]=='-'))\n";
            script += "      continue;\n";
            script += "    onlyNumbersSoFar = false;\n";
            script += "  };\n";
            script += "  if(onlyNumbersSoFar)\n";
            script += "    return(parseInt(term));";
            //solve inner brackets
            script += "  var control=term.indexOf('(');\n";
            script += "  var control1;\n";
            script += "  var control2;\n";
            script += "  var term1;\n";
            script += "  var term2;\n";
            script += "  if(control>-1){\n";
            script += "    term1=term.substring(0,control+1);\n";
            script += "    term2=term.substring(control+1,term.length);\n";
            script += "    while(true){\n";
            script += "      control1 = term2.indexOf(')');\n";
            script += "      control2 = term2.indexOf('(');\n";
            script += "      if(control2==-1 || control1<control2){\n";
            script += "        //alert(term1.substring(0,term1.length-1) +'&'+term2.substring(0,control1)+'&'+term2.substring(control1+1,term2.length));\n";
            script += "        return(eval(term1.substring(0,term1.length-1)+eval(term2.substring(0,control1))+term2.substring(control1+1,term2.length)));\n";
            script += "      }else{\n";
            script += "        term1 += term2.substring(0,control2+1);\n";
            script += "        term2 = term2.substring(control2+1,term2.length);\n"; 
            script += "      };\n";
            script += "    };\n";
            script += "  };\n";
            //do multiplication
            script += "  control=term.indexOf('*');\n";
            script += "  if(control>-1){\n";
            script += "    return(eval(term.substring(0,control))*eval(term.substring(control+1,term.length)));\n";
            script += "  };\n";
            //do division
            script += "  control=term.indexOf('/');\n";
            script += "  if(control>-1){\n";
            script += "    return(eval(term.substring(0,control))/eval(term.substring(control+1,term.length)));\n";
            script += "  };\n";
            //do addition
            script += "  control=term.indexOf('+');\n";
            script += "  if(control>-1){\n";
            script += "    return(eval(term.substring(0,control))+eval(term.substring(control+1,term.length)));\n";
            script += "  };\n";
            //do substraction
            script += "  control=term.indexOf('-');\n";
            script += "  if(control>-1){\n";
            script += "    return(eval(term.substring(0,control))-eval(term.substring(control+1,term.length)));\n";
            script += "  };\n";
            script += "};\n\n";


            //method for creating xml
            script += "function getXMLContent(){\n";
            script += @"  var xml = """";" + "\n";
            script += "  xml += '<questionnaireanswers>';\n";
            script += "  xml += '<groups>';\n";
            script += "  for(var i=0;i<groupnames.length;i++){\n";
            script += "    xml += '<group>';\n";
            script += "    xml += '<name>'+groupnames[i]+'</name>';\n";
            script += "    xml += '<rating>'+groupvalues[i]+'</rating>';\n";
            script += "    xml += '</group>';\n";
            script += "  };\n";
            script += "  xml += '</groups>';\n";
            script += "  xml+= '<questionnaireid>"+ questionnaireId + "</questionnaireid>';\n";
            script += "  xml +='</questionnaireanswers>';\n";
            script += "  return(xml);\n";
            script += "};\n\n";


            //GET HEALTH REQUEST - A2
            script += "function getHealth(){\n";
            script += "  var httpGet  = new XMLHttpRequest();\n";
            script += "  httpGet.onreadystatechange = function(){\n";
            script += "    if(httpGet.readyState == 4 && httpGet.status == 200)\n";
            script += "      alert(httpGet.responseText);\n";
            script += "  }\n alert(\"http://192.168.222.166:3400/api/health\");";
            script += "  httpGet.open(\"GET\",\"http://192.168.222.166:3400/api/health\",true);\n";
            script += "  httpGet.send(null);\n";
            script += "};\n\n";


            //method for sending xml
            script += "function submit(xml){\n";
            script += "  getHealth();\n";
            script += "  alert(xml);\n";
            script += "};\n\n";


            script += "</script>\n";
            return (script);
        }
        
        public String getFormulaByGroupName(String groupName)
        {
            foreach (QuestionnaireGroup qg in this.groupList.groups)
            {
                if (qg.name == groupName)
                    return (qg.formula);
            }
            return this.defaultFormula;
        }

        #endregion Methods
    }

    public class QuestionnaireGroupList
    {
        #region Fields

        /// <summary>
        /// Structure holding group information
        /// </summary>
        [XmlElement("group")]
        public List<QuestionnaireGroup> groups { set; get; }

        #endregion Fields
        #region Constructors

        public QuestionnaireGroupList()
        {
            this.groups = new List<QuestionnaireGroup>();
        }


        #endregion Constructors
        #region Methods


        #endregion Methods
    }

    public class QuestionnaireGroup
    {
        #region Fields

        /// <summary>
        /// Group name
        /// </summary>
        [XmlElement("name")]
        public string name { set; get; }

        /// <summary>
        /// Group evaluatin formula
        /// </summary>
        [XmlElement("formula")]
        public string formula { set; get; }

        #endregion Fields
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

        /// <summary>
        /// contains the evaluation mapping of this question to answers
        /// </summary>
        [XmlElement("answermapping")]
        public AnswerMappingList answerMappingList { get; set; }

        /// <summary>
        /// contains the group of the question
        /// </summary>
        [XmlElement("group")]
        public String group { get; set; }

        #endregion Fields
        #region Constructors

        public QuestionItem() {
            this.answerMappingList = new AnswerMappingList(0);
        }

        public QuestionItem(int id, string question)
        {
            this.questionId = id;
            this.question = question;
            this.answerMappingList = new AnswerMappingList(3);
        }

        #endregion Constructors
        #region Methods

        public int map(int choice)
        {
            for(int i =0; i< this.answerMappingList.answerMap.Count; i++)
            {
                if (this.answerMappingList.answerMap[i].answerPosition == choice)
                    return (this.answerMappingList.answerMap[i].mappedValue);
            }
            return (0);
        }

        #endregion Methods
    }

    public class AnswerMappingList
    {
        #region Fields

        /// <summary>
        /// List of all 1:1 mappings
        /// </summary>
        [XmlElement("entry")]
        public List<AnswerMap> answerMap { get; set; }

        #endregion Filds
        #region Constructors

        public AnswerMappingList() { }

        public AnswerMappingList(int length)
        {
            this.answerMap = new List<AnswerMap>();
            for(int i=0;i< length; i++)
            {
                this.answerMap.Add(new AnswerMap(i));
            }
        }

        #endregion Constructors
    }

    public class AnswerMap
    {
        #region Fields

        /// <summary>
        /// unique answer position
        /// </summary>
        [XmlElement("from")]
        public int answerPosition { get; set; }

        /// <summary>
        /// translation of the answer to integer value
        /// </summary>
        [XmlElement("to")]
        public int mappedValue { get; set; }

        #endregion Fields
        #region Constructors

        public AnswerMap() { }

        public AnswerMap(int pos)
        {
            this.answerPosition = pos;
            this.mappedValue = pos;
        }

        #endregion Constructors
    }

    #endregion SerializationQuestionnaire

    #region SerilizationAnswer

    /// <summary>
    /// Classe containing all relevant data for questionnaire and evaluation of questionnaire
    /// </summary>
    [XmlRoot("questionnaireanswers")]
    public class QuestionnaireAnswerData
    {
        #region Fields


        /// <summary>
        /// Class storing all answers.
        /// </summary>
        [XmlElement("groups")]
        public AnswerGroupList answerGroupList { get; set; }


        /// <summary>
        /// Questionnaire id
        /// </summary>
        [XmlElement("questionnaireid")]
        public string id { set; get; }

        #endregion Fields
        #region Constructors

        public QuestionnaireAnswerData() { }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Method for converting  to a xml string.
        /// </summary>
        /// 
        ///<returns>
        /// A string representing of the structure.
        /// </returns>
        public String toXmlString()
        {
            try
            {
                var xmlserializer = new XmlSerializer(typeof(QuestionnaireAnswerData));
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
        /// Method for creating the scores for each group.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, double> getResults()
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (AnswerGroup ag in this.answerGroupList.groupList)
                result.Add(ag.groupName, ag.groupRating);
            return result;
        }

        #endregion Methods
    }

    public class AnswerGroupList
    {
        #region Fields
        /// <summary>
        /// Structure holding one answer
        /// </summary>
        [XmlElement("group")]
        public List<AnswerGroup> groupList { set; get; }

        #endregion Fields
    }

    public class AnswerGroup
    {
        /// <summary>
        /// Questionnaire id
        /// </summary>
        [XmlElement("name")]
        public string groupName { set; get; }

        /// <summary>
        /// Answer id
        /// </summary>
        [XmlElement("rating")]
        public double groupRating { set; get; }
    }

    #endregion SerilizationAnswer

    /// <summary>
    /// Class for doing simple math calculations
    /// </summary>
    public static class MathInterpreter
    {
        /// <summary>
        /// Evaluates a given Formula containing the operators +,*,-,/,(,)
        /// </summary>
        /// 
        /// <param name="str"> Formula to interpret </param>
        public static double eval(String str)
        {
            if (!checkInput(str))
                throw new Exception("Input corrupted!");


            if (isPlainNumber(str))
            {
                Double result;
                if (!Double.TryParse(str, out result))
                    throw new Exception("Input corrupted!");
                return (result);
            }

            while (str.Contains('('))
            {
                str = resolveBrackets(str);
            }

            return solveOperation(str);
        }

        /// <summary>
        /// Method for resolving one pair of brackets within a formula-string
        /// </summary>
        /// <param name="str"> formula with brackets</param>
        /// <returns> String with expression instead of one pair of brackets</returns>
        public static String resolveBrackets(String str)
        {
            int open = -2;
            int nextOpen = str.IndexOf('(');
            int close = nextOpen + 1;
            while (nextOpen < close && nextOpen != open)
            {
                open = nextOpen;
                close = 1 + open + str.Substring(open + 1).IndexOf(')');
                nextOpen = 1 + open + str.Substring(open + 1).IndexOf('(');
            }
            String inBrackets = str.Substring(open + 1, close - open - 1);
            String returnValue = str.Substring(0, open) + solveOperation(inBrackets) + str.Substring(close + 1);
            return returnValue;
        }

        /// <summary>
        /// Method for checking the input digits/operators
        /// </summary>
        /// <param name="str"> formula to evaluate</param>
        /// <returns> true if the string contains valid characters, false otherwise</returns>
        public static Boolean checkInput(String str)
        {
            String validOperators = "+-*/:().";
            String digits = "0123456789";

            for (int i = 0; i < str.Length; i++)
                if (!validOperators.Contains(str[i]) && !digits.Contains(str[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Method for identifying strings, which can be casted to numbers
        /// </summary>
        /// <param name="str"> formula to evaluate</param>
        /// <returns></returns>
        public static Boolean isPlainNumber(String str)
        {
            String digits = "0123456789";
            if (str.Length == 0 || (str.Length == 1 && str[0] == '-'))
                return false;
            if (str[0] == '-')
                str = str.Substring(1, str.Length - 1);
            if (str.Contains('-'))
                return false;
            if (str.Contains('.'))
            {
                String str1 = str.Substring(0, str.IndexOf('.'));
                String str2 = str.Substring(str.IndexOf('.') + 1, str.Length - str.IndexOf('.') - 1);
                str = str1 + str2;
            }
            for (int i = 0; i < str.Length; i++)
                if (!digits.Contains(str[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Method for solving an arithmetic formula containing +,-,*,/
        /// </summary>
        /// <param name="str">formula to solve</param>
        /// <returns>result of this formula</returns>
        public static Double solveOperation(String str)
        {
            str = str.Replace("-+", "-");
            //Console.WriteLine("::"+str);
            if (isPlainNumber(str))
            {
                Double result;
                if (!Double.TryParse(str, out result))
                    throw new Exception("Input corrupted!");
                return (result);
            }

            if (str.Contains('+'))
            {
                String str1 = str.Substring(0, str.IndexOf('+'));
                String str2 = str.Substring(str.IndexOf('+') + 1, str.Length - str.IndexOf('+') - 1);
                if (str1[str1.Length - 1] == '*' || str1[str1.Length - 1] == '/')
                    goto ContinuePlus;
                if (str1.Length == 0 || str2.Length == 0)
                    throw new Exception("Input corrupted!");
                return (solveOperation(str1) + solveOperation(str2));
            }

            ContinuePlus:

            if (str.Contains('-'))
            {
                String str1 = str.Substring(0, str.IndexOf('-'));
                String str2 = str.Substring(str.IndexOf('-') + 1, str.Length - str.IndexOf('-') - 1);
                if (str1.Length > 0 && (str1[str1.Length - 1] == '*' || str1[str1.Length - 1] == '/'))
                    goto ContinueMinus;
                if (str2.Length == 0)
                    throw new Exception("Input corrupted!");
                if (str1.Length == 0)
                    return (-solveOperation(str2));
                return (solveOperation(str1) - solveOperation(str2));
            }

            ContinueMinus:

            if (str.Contains('*'))
            {
                String str1 = str.Substring(0, str.IndexOf('*'));
                String str2 = str.Substring(str.IndexOf('*') + 1, str.Length - str.IndexOf('*') - 1);
                if (str1.Length == 0 || str2.Length == 0)
                    throw new Exception("Input corrupted!");
                return (solveOperation(str1) * solveOperation(str2));
            }

            if (str.Contains('/'))
            {
                String str1 = str.Substring(0, str.IndexOf('/'));
                String str2 = str.Substring(str.IndexOf('/') + 1, str.Length - str.IndexOf('/') - 1);
                if (str1.Length == 0 || str2.Length == 0)
                    throw new Exception("Input corrupted!");
                return (solveOperation(str1) / solveOperation(str2));
            }

            return 0.0;
        }
    }
}
