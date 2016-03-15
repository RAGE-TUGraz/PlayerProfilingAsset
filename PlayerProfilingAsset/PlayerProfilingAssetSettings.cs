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

namespace AssetPackage
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// An asset settings.
    /// 
    /// BaseSettings contains the (de-)serialization methods.
    /// </summary>
    public class PlayerProfilingAssetSettings : BaseSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the PlayerProfiler.AssetSettings class.
        /// </summary>
        public PlayerProfilingAssetSettings()
            : base()
        {
            // Set Default values here.
            QuestionnaireDataXMLFileId = "QuestionnaireData.xml";
            HTMLQuestionnaireFileId = "QuestionnaireData.html";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// File Id under which the questionareData is loaded.
        /// </summary>
        [XmlElement()]
        public String QuestionnaireDataXMLFileId
        {
            get;
            set;
        }

        /// <summary>
        /// File Id under which the questionareData-HTML representation is stored.
        /// </summary>
        [XmlElement()]
        public String HTMLQuestionnaireFileId
        {
            get;
            set;
        }


        #endregion Properties
    }
}
