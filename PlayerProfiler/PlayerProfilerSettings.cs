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
    public class PlayerProfilerSettings : BaseSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the PlayerProfiler.AssetSettings class.
        /// </summary>
        public PlayerProfilerSettings()
            : base()
        {
            // Set Default values here.
            TestProperty = "Hello Default World";
            TestList = new String[] { "Red", "Green", "Blue" };
            TestPrivate = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the test property.
        /// </summary>
        ///
        /// <value>
        /// The test property.
        /// </value>
        [XmlElement()]
        public String TestProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the string[].
        /// </summary>
        ///
        /// <value>
        /// .
        /// </value>
        [XmlArray()]
        [XmlArrayItem("ListItem")]
        public String[] TestList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the test read only.
        /// </summary>
        ///
        /// <value>
        /// true if test read only, false if not.
        /// </value>
        public Boolean TestReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the test private.
        /// </summary>
        ///
        /// <value>
        /// true if test private only, false if not.
        /// </value>
        public Boolean TestPrivate
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
