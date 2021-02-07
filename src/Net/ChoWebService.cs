//using Cinchoo.Core.Xml;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Xml;

//namespace Cinchoo.Core.Net
//{
//    public enum ChoWebServiceRequestType { SOAP_1_1, SOAP_1_2, HTTP_POST, HTTP_GET }

//    public class ChoWebService : ChoDisposableObject
//    {
//        #region Constants

//        private const string SOAP_ENVELOPE_XML = @"<?xml version=""1.0"" encoding=""utf-8""?>
//            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
//                <soap:Body>{0}</soap:Body>
//            </soap:Envelope>";
//        private const string SOAP12_ENVELOPE_XML = @"<?xml version=""1.0"" encoding=""utf-8""?>
//            <soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
//                <soap12:Body>{0}</soap:Body>
//            </soap:Envelope>";

//        #endregion Constants

//        #region Instance Properties

//        public string Url
//        {
//            get;
//            private set;
//        }

//        public string WSNamespace
//        {
//            get;
//            private set;
//        }

//        #endregion Instance Properties

//        #region Constructor

//        public ChoWebService(string url, string wsNamespace)
//        {
//            ChoGuard.ArgumentNotNullOrEmpty(url, "Url");
//            ChoGuard.ArgumentNotNullOrEmpty(wsNamespace, "WSNamespace");

//            Url = url;
//            WSNamespace = wsNamespace;
//        }

//        #endregion Constructor

//        #region Instance Members (Public)

//        /// <summary>
//        /// Execute a Soap WebService call
//        /// </summary>
//        public string Execute(string webMethodName, Diction ChoWebServiceRequestType requestType = ChoWebServiceRequestType.SOAP_1_1)
//        {
//            ChoGuard.ArgumentNotNullOrEmpty(webMethodName, "WebMethodName");

//            HttpWebRequest request = CreateWebRequest(Url);
//            XmlDocument soapEnvelopeXml = new XmlDocument();

//            if (requestType == ChoWebServiceRequestType.SOAP_1_1)
//                soapEnvelopeXml.LoadXml(SOAP_ENVELOPE_XML.FormatString(GetSoap11Body(webMethodName)));
//            else if (requestType == ChoWebServiceRequestType.SOAP_1_2)
//                soapEnvelopeXml.LoadXml(SOAP12_ENVELOPE_XML.FormatString(soapBodyXml));

//            using (Stream stream = request.GetRequestStream())
//            {
//                soapEnvelopeXml.Save(stream);
//            }

//            using (WebResponse response = request.GetResponse())
//            {
//                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
//                {
//                    return rd.ReadToEnd();
//                }
//            }
//        }

//        public T Execute<T>(string soapBodyXml, ChoWebServiceRequestType requestType = ChoWebServiceRequestType.SOAP_1_1)
//        {
//            T retValue = default(T);

//            string ret = Execute(soapBodyXml);

//            return retValue;
//        }

//        #endregion Instance Members (Public)

//        #region Instance Members (Private)

//        /// <summary>
//        /// Create a soap webrequest to [Url]
//        /// </summary>
//        /// <returns></returns>
//        public static HttpWebRequest CreateWebRequest(string url)
//        {
//            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
//            webRequest.Headers.Add(@"SOAP:Action");
//            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
//            webRequest.Accept = "text/xml";
//            webRequest.Method = "POST";
//            return webRequest;
//        }

//        #endregion Instance Members (Private)

//        #region ChoDisposableObject Overrides

//        protected override void Dispose(bool finalize)
//        {
//        }

//        #endregion ChoDisposableObject Overrides
//    }
//}
