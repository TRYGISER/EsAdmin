﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;


namespace EsAdmin.ElasticSearch
{
    public enum ConnectionErrorType
    {
        Uncaught,
        Client,
        Server,
        UnAuthorizedAccess
    }

    public class ConnectionError
    {
        public ConnectionErrorType Type { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ExceptionMessage { get; set; }
        public Exception OriginalException { get; set; }

        public ConnectionError(Exception e)
        {
            this.OriginalException = e;
            this.ExceptionMessage = e.Message;
            this.Type = ConnectionErrorType.Uncaught;

            var webException = e as WebException;
            if (webException != null)
            {
                this.Type = ConnectionErrorType.Server;
                var response = ((HttpWebResponse)webException.Response);
                if (response == null)
                {
                    this.Type = ConnectionErrorType.Client;
                }
                else
                {
                    this.HttpStatusCode = response.StatusCode;
                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, true))
                    {
                        var responseString = reader.ReadToEnd();
                        this.ExceptionMessage = responseString;

                    }
                }

            }


        }
    }

}
