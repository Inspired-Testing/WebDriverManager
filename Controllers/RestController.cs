using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using WebDriverManager.Models;

namespace WebDriverManager.Controllers
{
    internal class RestController
    {
         /// <summary>
              /// The base url of the Server which hosts the Rest API
              /// </summary>
              protected string BaseUrl;
              private RestClient httpClient;
              private CookieContainer cookieJar;
              private TimeSpan _sla;
              private Dictionary<string, string> headers = new Dictionary<string, string>();

              /// <summary>
              /// RestController for executing Rest Requests against a Server
              /// </summary>
              /// <param name="url">REST server URL</param>
              /// <param name="SLA">Service Level Agreement - agreed time to fail a test</param>
              public RestController(string url, int SLA = 5000)
              {
                     this.BaseUrl = url;
                     this.httpClient = new RestClient(url);
                     this.cookieJar = new CookieContainer();
                     this.httpClient.CookieContainer = cookieJar;
                     this._sla = TimeSpan.FromMilliseconds(SLA);
              }

              #region Posts        
              /// <summary>
              /// Runs a Post Request against a given endpoint
              /// Requires Headers 
              /// Requires Auth
              /// Includes all cookies
              /// </summary>
              /// <param name="endpoint">This will be appended to the BaseURl</param>
              /// <param name="body">JSON data to be posted</param>
              /// <param name="headers">Key Value pairs to include on the Header, defaults to null</param>
              /// <param name="auth">Authentication object to be used on endpoint, defaults to null</param>
              /// <returns></returns>
              public WebServiceResponse Post(string endpoint, string body, IAuthenticator auth = null, Dictionary<string, string> headers = null)
              {
                     httpClient = new RestClient(this.BaseUrl + endpoint);
                     if (auth != null)
                     {
                            httpClient.Authenticator = auth;
                     }
                     var request = new RestRequest(Method.POST);

                     if (headers != null)
                     {
                            foreach (var header in headers)
                            {
                                   request.AddHeader(header.Key, header.Value);
                            }

                     }

                     request.AddParameter("text/plain", body, ParameterType.RequestBody);
                     DateTime startTime = DateTime.Now;
                     var response = httpClient.Post(request);
                     DateTime endTime = DateTime.Now;

                    

                     WebServiceResponse wsResponse = new WebServiceResponse()
                     {
                            ResponseData = response.Content,
                            Status = response.StatusCode,
                            StartTime = startTime,
                            EndTime = endTime,
                            Duration = endTime - startTime,
                            SLA = _sla,
                            EndPoint = this.BaseUrl + endpoint
                     };

                     return wsResponse;
              }

              #endregion

              #region Gets        
              /// <summary>
              /// Runs a Get Request against a given endpoint
              /// Requires Headers
              /// Requires Authentication
              /// </summary>
              /// <param name="endpoint">This will be appended to the BaseURl</param>
              /// <param name="headers">Key Value pairs to include on the Header, defaults to null</param>
              /// <param name="auth">Authentication object to be used on endpoint, defaults to null</param>
              /// <returns>WebServiceResponse with relevant data</returns>
              public WebServiceResponse Get(string endpoint, Dictionary<string, string> headers = null, IAuthenticator auth = null)
              {
                     httpClient = new RestClient(this.BaseUrl + endpoint);
                     if (auth != null)
                     {
                            httpClient.Authenticator = auth;
                     }

                     var request = new RestRequest(Method.GET);

                     if (headers != null)
                     {
                            foreach (var header in headers)
                            {
                                   request.AddHeader(header.Key, header.Value);
                            }

                     }

                     DateTime startTime = DateTime.Now;
                     var response = httpClient.Get(request);
                     DateTime endTime = DateTime.Now;

                   


                     WebServiceResponse wsResponse = new WebServiceResponse()
                     {
                            ResponseData = response.Content,
                            Status = response.StatusCode,
                            StartTime = startTime,
                            EndTime = endTime,
                            Duration = endTime - startTime,
                            SLA = _sla,
                            EndPoint = this.BaseUrl + endpoint
                     };

                     return wsResponse;
              }

              #endregion

              #region Put

              /// <summary>
              /// Runs a Put Request against a given endpoint
              /// Requires Headers 
              /// Requires Auth
              /// Includes all cookies
              /// </summary>
              /// <param name="endpoint">This will be appended to the BaseURl</param>
              /// <param name="body">JSON data to be posted</param>
              /// <param name="headers">Key Value pairs to include on the Header, defaults to null</param>
              /// <param name="auth">Authentication object to be used on endpoint, defaults to null</param>
              /// <returns></returns>
              public WebServiceResponse Put(string endpoint, string body, Dictionary<string, string> headers = null, IAuthenticator auth = null)
              {
                     httpClient = new RestClient(this.BaseUrl + endpoint);
                     if (auth != null)
                     {
                            httpClient.Authenticator = auth;
                     }
                     var request = new RestRequest(Method.PUT);

                     if (headers != null)
                     {
                            foreach (var header in headers)
                            {
                                   request.AddHeader(header.Key, header.Value);
                            }

                     }

                     request.AddParameter("text/plain", body, ParameterType.RequestBody);
                     DateTime startTime = DateTime.Now;
                     var response = httpClient.Put(request);
                     DateTime endTime = DateTime.Now;

                   


                     WebServiceResponse wsResponse = new WebServiceResponse()
                     {
                            ResponseData = response.Content,
                            Status = response.StatusCode,
                            StartTime = startTime,
                            EndTime = endTime,
                            Duration = endTime - startTime,
                            SLA = _sla,
                            EndPoint = this.BaseUrl + endpoint
                     };
                     return wsResponse;
              }

              #endregion

              #region Delete        
              /// <summary>
              /// Runs a Delete Request against a given endpoint
              /// Requires Headers
              /// Requires Authentication
              /// </summary>
              /// <param name="endpoint">This will be appended to the BaseURl</param>
              /// <param name="headers">Key Value pairs to include on the Header, defaults to null</param>
              /// <param name="auth">Authentication object to be used on endpoint, defaults to null</param>
              /// <returns>WebServiceResponse with relevant data</returns>
              public WebServiceResponse Delete(string endpoint, Dictionary<string, string> headers = null, IAuthenticator auth = null)
              {
                     httpClient = new RestClient(this.BaseUrl + endpoint);
                     if (auth != null)
                     {
                            httpClient.Authenticator = auth;
                     }

                     var request = new RestRequest(Method.DELETE);

                     if (headers != null)
                     {
                            foreach (var header in headers)
                            {
                                   request.AddHeader(header.Key, header.Value);
                            }

                     }

                     DateTime startTime = DateTime.Now;
                     var response = httpClient.Delete(request);
                     DateTime endTime = DateTime.Now;

                   

                     WebServiceResponse wsResponse = new WebServiceResponse()
                     {
                            ResponseData = response.Content,
                            Status = response.StatusCode,
                            StartTime = startTime,
                            EndTime = endTime,
                            Duration = endTime - startTime,
                            SLA = _sla,
                            EndPoint = this.BaseUrl + endpoint
                     };
                     return wsResponse;
              }

            
              #endregion
    }
}