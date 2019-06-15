using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


/// <summary>
/// HandSerializer: This class contains a method to serialize and deserialize any kind of object. It is to be used
///                 for debugging purposes for feeding data from a websocket and feeding it into the leapserviceprovider.
/// </summary>
public static class HandSerializer {

    /// <summary>
    /// WriteToBinaryFile: Method to save an object to file.
    ///
    /// <param name="filePath"> The filepath to save the serialized hand to. </param>
    /// <param name="objectToWrite"> The hand object to save. </param>
    /// <param name="append"> An optional boolean indicating whether this should create or append to an existing file. </param>
    /// </summary>
    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
    {
        using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    /// <summary>
    /// WriteToBinaryFile: Method to read an object to file.
    ///
    /// <param name="filePath"> The filepath to read the serialized hand from. </param>
    /// </summary>
    public static T ReadFromBinaryFile<T>(string filePath)
    {
        using (Stream stream = File.Open(filePath, FileMode.Open))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

}
