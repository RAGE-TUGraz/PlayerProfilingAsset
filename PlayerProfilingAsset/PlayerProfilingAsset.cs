/*
  Copyright 2016 TUGraz, http://www.tugraz.at/
  
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  This project has received funding from the European Union�s Horizon
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

namespace PlayerProfilingAssetNameSpace
{
    using System;
    using System.Collections.Generic;
    using AssetPackage;

    /// <summary>
    /// A singelton asset.
    /// </summary>
    public class PlayerProfilingAsset : BaseAsset
    {
        #region Fields

        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private PlayerProfilingAssetSettings settings = null;

        /// <summary>
        /// Instance of the class PlayerProfilerHandler - Singelton pattern
        /// </summary>
        static readonly PlayerProfilingAsset instance = new PlayerProfilingAsset();

        /// <summary>
        /// Instance of the PlayerProfilingHandler
        /// </summary>
        static internal PlayerProfilerHandler playerProfilingHandler = new PlayerProfilerHandler();

        #endregion Fields
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the PlayerProfiler.Asset class.
        /// </summary>
        private PlayerProfilingAsset()
            : base()
        {
            //! Create Settings and let it's BaseSettings class assign Defaultvalues where it can.
            settings = new PlayerProfilingAssetSettings();
        }

        #endregion Constructors
        #region Properties

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        ///
        /// <remarks>   Besides the toXml() and fromXml() methods, we never use this property but use
        ///                it's correctly typed backing field 'settings' instead. </remarks>
        /// <remarks> This property should go into each asset having Settings of its own. </remarks>
        /// <remarks>   The actual class used should be derived from BaseAsset (and not directly from
        ///             ISetting). </remarks>
        ///
        /// <value>
        /// The settings.
        /// </value>
        public override ISettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = (value as PlayerProfilingAssetSettings);
            }
        }

        /// <summary>
        /// Getter for Instance of the PlayerProfilingAsset - Singelton pattern
        /// </summary>
        public static PlayerProfilingAsset Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Getter for Instance of the PlayerProfilerHandler
        /// </summary>
        internal static PlayerProfilerHandler Handler
        {
            get
            {
                return playerProfilingHandler;
            }
        }

        #endregion Properties
        #region Methods

        /// <summary>
        /// Method for requesting the questionares fileId - the fileId under which the HTML-Questionnaire can be accessed.
        /// </summary>
        /// <returns></returns>
        public string getQuestionnaireFileId()
        {
            return playerProfilingHandler.getHTMLQuestionnaire();
        } 

        /// <summary>
        /// Method for requesting questionnaire results.
        /// </summary>
        /// 
        /// <returns> Value for the questionnaire per group.</returns>
        public Dictionary<String, Double> getResults()
        {
            if(playerProfilingHandler.questionnaireResults != null)
                return playerProfilingHandler.questionnaireResults;
            
            return playerProfilingHandler.getQuestionnaireResultFromGameStorage();
        }

        /// <summary>
        /// Method for requestiong the questionnaire data.
        /// </summary>
        /// <returns> A string containing the questionnaire xml</returns>
        public String getQuestionnaireXML()
        {
            return playerProfilingHandler.getQuestionnaireData().toXmlString();
        }

        /// <summary>
        /// Method for returning the chosen answers of the questionnaire to the asset.
        /// </summary>
        /// 
        /// <param name="answers">Dictionary containing the questionnaire questions as keys and the chosen answer as value.</param>
        /// <returns> True if the supplied data is accurate, false otherwise.</returns>
        public Boolean setQuestionnaireAnswers(Dictionary<String, int> answers)
        {
            return playerProfilingHandler.getQuestionnaireData().checkAnswerData(answers);
        }

        #endregion Methods
        #region internal Methods

        /// <summary>
        /// Wrapper method for getting the getInterface method of the base Asset
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>Corresponding Interface</returns>
        internal T getInterfaceFromAsset<T>()
        {
            return this.getInterface<T>();
        }

        #endregion internal Methods
    }
}