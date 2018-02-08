// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    /// <summary>
    /// Represents an HTTP response exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HttpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        public HttpException()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public HttpException(string message, Exception inner)
            : base(message, inner)
        {
            if (inner != null)
            {
                this.HResult = inner.HResult;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="method">The request method.</param>
        /// <param name="uri">The request URI.</param>
        /// <param name="inner">The inner exception.</param>
        public HttpException(HttpMethod method, Uri uri, Exception inner)
            : this($"An error occurred when attempting to call {method} {uri}.", inner)
        {
            this.Method = method;
            this.Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="method">The request method.</param>
        /// <param name="uri">The request URI.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reasonPhrase">The reason phrase.</param>
        /// <param name="contentMessage">The content message.</param>
        public HttpException(HttpMethod method, Uri uri, HttpStatusCode statusCode, string reasonPhrase, string contentMessage)
            : this(GetResponseMessage(method, uri, statusCode, reasonPhrase, contentMessage), null)
        {
            this.Method = method;
            this.Uri = uri;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.ContentMessage = contentMessage;
        }

        /// <summary>
        /// Gets the request URI.
        /// </summary>
        /// <value>The request URI.</value>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the request method.
        /// </summary>
        /// <value>The request method.</value>
        public HttpMethod Method { get; }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the reason phrase.
        /// </summary>
        /// <value>The reason phrase.</value>
        public string ReasonPhrase { get; }

        /// <summary>
        /// Gets the message returned from the body of the response.
        /// </summary>
        /// <value>The message returned from the body of the response.</value>
        public string ContentMessage { get; }

        private static string GetResponseMessage(HttpMethod method, Uri uri, HttpStatusCode statusCode, string reasonPhrase, string contentMessage)
        {
            var builder = new StringBuilder();
            builder.Append($"Unexpected response when attempting to call {method} {uri} ({(int)statusCode} {reasonPhrase})");

            if (contentMessage == null)
            {
                builder.Append(".");
            }
            else
            {
                builder.AppendLine(":");
                builder.Append(contentMessage);
            }

            return builder.ToString();
        }
    }
}
