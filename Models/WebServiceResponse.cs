using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace WebDriverManager.Models
{
    internal class WebServiceResponse
    {
        /// <summary>
        /// The full URl
        /// </summary>
        internal string EndPoint { get; set; }

        /// <summary>
        /// The response from the WebService in string format
        /// </summary>
        internal string ResponseData { get; set; }

        /// <summary>
        /// The HTTP Status of the given Request
        /// </summary>
        internal HttpStatusCode Status { get; set; }

        /// <summary>
        /// DateTime object from before the Request was sent
        /// </summary>
        internal DateTime StartTime { get; set; }

        /// <summary>
        /// DateTime object from after the response was received
        /// </summary>
        internal DateTime EndTime { get; set; }

        /// <summary>
        /// Difference between EndTime and StartTime
        /// </summary>
        internal TimeSpan Duration { get; set; }

        /// <summary>
        /// Service Level Agreement
        /// This is the acceptable timeframe for the Request to respond
        /// </summary>
        internal TimeSpan SLA { get; set; }

        internal enum RemoteProtocolType {REST , SOAP}

        /// <summary>
        /// RPC type
        /// SOAP or REST
        /// </summary>
        internal RemoteProtocolType ResponseType { get; set; } = RemoteProtocolType.REST;
 

        /// <summary>
        /// Deserialises XML or JSON to an Object of specified Type
        /// </summary>
        /// <typeparam name="T">Type of expected class object</typeparam>
        /// <returns>Object which must be cast to the expected type</returns>
        internal object DeserialiseResponseTo<T>()
        {
                if (ResponseType == RemoteProtocolType.SOAP)
                {
                    var serialiser = new XmlSerializer(typeof(T));
                    object result;

                    using (TextReader reader = new StringReader(ResponseData))
                    {
                            result = (T)serialiser.Deserialize(reader);
                    }

                    return result;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(ResponseData);
                }

        }

        internal T DeserialiseAsXML<T>() 
        {
            var serialiser = new XmlSerializer(typeof(T));
            T result;

            using (TextReader reader = new StringReader(ResponseData))
            {
                result = (T)serialiser.Deserialize(reader);
            }

            return result;
        }
    }
}