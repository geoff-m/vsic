﻿using System.IO;

namespace SICXE
{
    internal interface ISerialize
    {
        /// <summary>
        /// Writes the data that may be used later to restore the state of this instance (or some subset thereof).
        /// </summary>
        void Serialize(Stream stream);

        ///// <summary>
        ///// This method should change the state of this object so that it matches the one described by the data read from the stream.
        ///// </summary>
        ///// <param name="readFromStream">The read method of the stream to be used for deserialization.</param>
        //void Deserialize(Stream stream);
    }
}
