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
using System.Linq;
using System.Text;

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
            //performTest1();
            loggingPPA("Tests PlayerProfilingAsset - done!");
            loggingPPA("*****************************************************************");
        }

        #endregion TestMethods
    }
}
