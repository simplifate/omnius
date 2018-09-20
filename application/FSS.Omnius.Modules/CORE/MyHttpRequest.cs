using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Routing;

namespace FSS.Omnius.Modules.CORE
{
    public class MyHttpRequest
    {
        public MyHttpRequest(HttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }
        public MyHttpRequest(HttpRequestBase httpRequestBase)
        {
            _httpRequestBase = httpRequestBase;
        }

        private HttpRequest _httpRequest;
        private HttpRequestBase _httpRequestBase;

        //
        // Summary:
        //     Gets the specified object from the System.Web.HttpRequest.QueryString, System.Web.HttpRequest.Form,
        //     System.Web.HttpRequest.Cookies, or System.Web.HttpRequest.ServerVariables collections.
        //
        // Parameters:
        //   key:
        //     The name of the collection member to get.
        //
        // Returns:
        //     The System.Web.HttpRequest.QueryString, System.Web.HttpRequest.Form, System.Web.HttpRequest.Cookies,
        //     or System.Web.HttpRequest.ServerVariables collection member specified in the
        //     key parameter. If the specified key is not found, then null is returned.
        public string this[string key] => _httpRequest != null ? _httpRequest[key] : _httpRequestBase[key];

        //
        // Summary:
        //     Gets the System.Security.Principal.WindowsIdentity type for the current user.
        //
        // Returns:
        //     A System.Security.Principal.WindowsIdentity object for the current Microsoft
        //     Internet Information Services (IIS) authentication settings.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The Web application is running in IIS 7 integrated mode and the System.Web.HttpApplication.PostAuthenticateRequest
        //     event has not yet been raised.
        public WindowsIdentity LogonUserIdentity => _httpRequest != null ? _httpRequest.LogonUserIdentity : _httpRequestBase.LogonUserIdentity;
        //
        // Summary:
        //     Gets the physical file system path corresponding to the requested URL.
        //
        // Returns:
        //     The file system path of the current request.
        public string PhysicalPath => _httpRequest != null ? _httpRequest.PhysicalPath : _httpRequestBase.PhysicalPath;
        //
        // Summary:
        //     Gets the ASP.NET application's virtual application root path on the server.
        //
        // Returns:
        //     The virtual path of the current application.
        public string ApplicationPath => _httpRequest != null ? _httpRequest.ApplicationPath : _httpRequestBase.ApplicationPath;
        //
        // Summary:
        //     Gets the physical file system path of the currently executing server application's
        //     root directory.
        //
        // Returns:
        //     The file system path of the current application's root directory.
        public string PhysicalApplicationPath => _httpRequest != null ? _httpRequest.PhysicalApplicationPath : _httpRequestBase.PhysicalApplicationPath;
        //
        // Summary:
        //     Gets the raw user agent string of the client browser.
        //
        // Returns:
        //     The raw user agent string of the client browser.
        public string UserAgent => _httpRequest != null ? _httpRequest.UserAgent : _httpRequestBase.UserAgent;
        //
        // Summary:
        //     Gets a sorted string array of client language preferences.
        //
        // Returns:
        //     A sorted string array of client language preferences, or null if empty.
        public string[] UserLanguages => _httpRequest != null ? _httpRequest.UserLanguages : _httpRequestBase.UserLanguages;
        //
        // Summary:
        //     Gets the DNS name of the remote client.
        //
        // Returns:
        //     The DNS name of the remote client.
        public string UserHostName => _httpRequest != null ? _httpRequest.UserHostName : _httpRequestBase.UserHostName;
        //
        // Summary:
        //     Gets the IP host address of the remote client.
        //
        // Returns:
        //     The IP address of the remote client.
        public string UserHostAddress => _httpRequest != null ? _httpRequest.UserHostAddress : _httpRequestBase.UserHostAddress;
        //
        // Summary:
        //     Gets the raw URL of the current request.
        //
        // Returns:
        //     The raw URL of the current request.
        public string RawUrl => _httpRequest != null ? _httpRequest.RawUrl : _httpRequestBase.RawUrl;
        //
        // Summary:
        //     Gets information about the URL of the current request.
        //
        // Returns:
        //     A System.Uri object that contains the URL of the current request.
        public Uri Url => _httpRequest != null ? _httpRequest.Url : _httpRequestBase.Url;
        //
        // Summary:
        //     Gets information about the URL of the client's previous request that linked to
        //     the current URL.
        //
        // Returns:
        //     A System.Uri object.
        //
        // Exceptions:
        //   T:System.UriFormatException:
        //     The HTTP Referer request header is malformed and cannot be converted to a System.Uri
        //     object.
        public Uri UrlReferrer => _httpRequest != null ? _httpRequest.UrlReferrer : _httpRequestBase.UrlReferrer;
        //
        // Summary:
        //     Gets the System.Security.Authentication.ExtendedProtection.ChannelBinding object
        //     of the current System.Web.HttpWorkerRequest instance.
        //
        // Returns:
        //     The System.Security.Authentication.ExtendedProtection.ChannelBinding object of
        //     the current System.Web.HttpWorkerRequest instance.
        //
        // Exceptions:
        //   T:System.PlatformNotSupportedException:
        //     The current System.Web.HttpWorkerRequest object is not a System.Web.Hosting.IIS7WorkerRequest
        //     object or a System.Web.Hosting.ISAPIWorkerRequestInProc object.
        public ChannelBinding HttpChannelBinding => _httpRequest != null ? _httpRequest.HttpChannelBinding : _httpRequestBase.HttpChannelBinding;
        //
        // Summary:
        //     Gets a combined collection of System.Web.HttpRequest.QueryString, System.Web.HttpRequest.Form,
        //     System.Web.HttpRequest.Cookies, and System.Web.HttpRequest.ServerVariables items.
        //
        // Returns:
        //     A System.Collections.Specialized.NameValueCollection object.
        public NameValueCollection Params => _httpRequest != null ? _httpRequest.Params : _httpRequestBase.Params;
        //
        // Summary:
        //     Gets the collection of HTTP query string variables.
        //
        // Returns:
        //     Query string variables sent by the client. Keys and values are URL-decoded.
        public NameValueCollection QueryString => _httpRequest != null ? _httpRequest.QueryString : _httpRequestBase.QueryString;
        //
        // Summary:
        //     Gets a collection of form variables.
        //
        // Returns:
        //     A System.Collections.Specialized.NameValueCollection representing a collection
        //     of form variables.
        public NameValueCollection Form => _httpRequest != null ? _httpRequest.Form : _httpRequestBase.Form;
        //
        // Summary:
        //     Gets a collection of HTTP headers.
        //
        // Returns:
        //     A System.Collections.Specialized.NameValueCollection of headers.
        public NameValueCollection Headers => _httpRequest != null ? _httpRequest.Headers : _httpRequestBase.Headers;
        //
        // Summary:
        //     Gets a collection of Web server variables.
        //
        // Returns:
        //     A System.Collections.Specialized.NameValueCollection of server variables.
        public NameValueCollection ServerVariables => _httpRequest != null ? _httpRequest.ServerVariables : _httpRequestBase.ServerVariables;
        //
        // Summary:
        //     Gets a collection of cookies sent by the client.
        //
        // Returns:
        //     An System.Web.HttpCookieCollection object representing the client's cookie variables.
        public HttpCookieCollection Cookies => _httpRequest != null ? _httpRequest.Cookies : _httpRequestBase.Cookies;
        //
        // Summary:
        //     Gets the contents of the incoming HTTP entity body.
        //
        // Returns:
        //     A System.IO.Stream object representing the contents of the incoming HTTP content
        //     body.
        public Stream InputStream => _httpRequest != null ? _httpRequest.InputStream : _httpRequestBase.InputStream;
        //
        // Summary:
        //     Gets the number of bytes in the current input stream.
        //
        // Returns:
        //     The number of bytes in the input stream.
        public int TotalBytes => _httpRequest != null ? _httpRequest.TotalBytes : _httpRequestBase.TotalBytes;
        //
        // Summary:
        //     Gets or sets the filter to use when reading the current input stream.
        //
        // Returns:
        //     A System.IO.Stream object to be used as the filter.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     The specified System.IO.Stream is invalid.
        public Stream Filter
        {
            get
            {
                return _httpRequest != null ? _httpRequest.Filter : _httpRequestBase.Filter;
            }
            set
            {
                if (_httpRequest != null)
                    _httpRequest.Filter = value;
                else
                    _httpRequestBase.Filter = value;
            }
        }
        //
        // Summary:
        //     Gets the current request's client security certificate.
        //
        // Returns:
        //     An System.Web.HttpClientCertificate object containing information about the client's
        //     security certificate settings.
        public HttpClientCertificate ClientCertificate => _httpRequest != null ? _httpRequest.ClientCertificate : _httpRequestBase.ClientCertificate;
        //
        // Summary:
        //     Gets additional path information for a resource with a URL extension.
        //
        // Returns:
        //     Additional path information for a resource.
        public string PathInfo => _httpRequest != null ? _httpRequest.PathInfo : _httpRequestBase.PathInfo;
        //
        // Summary:
        //     Gets a value that indicates whether the request entity body has been read, and
        //     if so, how it was read.
        //
        // Returns:
        //     The value that indicates how the request entity body was read, or that it has
        //     not been read.
        public ReadEntityBodyMode ReadEntityBodyMode => _httpRequest != null ? _httpRequest.ReadEntityBodyMode : _httpRequestBase.ReadEntityBodyMode;
        //
        // Summary:
        //     Gets the virtual path of the application root and makes it relative by using
        //     the tilde (~) notation for the application root (as in "~/page.aspx").
        //
        // Returns:
        //     The virtual path of the application root for the current request.
        public string AppRelativeCurrentExecutionFilePath => _httpRequest != null ? _httpRequest.AppRelativeCurrentExecutionFilePath : _httpRequestBase.AppRelativeCurrentExecutionFilePath;
        //
        // Summary:
        //     Gets the virtual path of the current request.
        //
        // Returns:
        //     The virtual path of the current request.
        public string CurrentExecutionFilePath => _httpRequest != null ? _httpRequest.CurrentExecutionFilePath : _httpRequestBase.CurrentExecutionFilePath;
        //
        // Summary:
        //     Gets the extension of the file name that is specified in the System.Web.HttpRequest.CurrentExecutionFilePath
        //     property.
        //
        // Returns:
        //     The extension of the file name that is specified in the System.Web.HttpRequest.CurrentExecutionFilePath
        //     property.
        public string CurrentExecutionFilePathExtension => _httpRequest != null ? _httpRequest.CurrentExecutionFilePathExtension : _httpRequestBase.CurrentExecutionFilePathExtension;
        //
        // Summary:
        //     Gets the System.Web.Routing.RequestContext instance of the current request.
        //
        // Returns:
        //     The System.Web.Routing.RequestContext instance of the current request. For non-routed
        //     requests, the System.Web.Routing.RequestContext object that is returned is empty.
        public RequestContext RequestContext
        {
            get
            {
                return _httpRequest != null ? _httpRequest.RequestContext : _httpRequestBase.RequestContext;
            }
            set
            {
                if (_httpRequest != null)
                    _httpRequest.RequestContext = value;
                else
                    _httpRequestBase.RequestContext = value;
            }
        }
        //
        // Summary:
        //     Gets a value indicating whether the request is from the local computer.
        //
        // Returns:
        //     true if the request is from the local computer; otherwise, false.
        public bool IsLocal => _httpRequest != null ? _httpRequest.IsLocal : _httpRequestBase.IsLocal;
        //
        // Summary:
        //     Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.
        //
        // Returns:
        //     The HTTP data transfer method used by the client.
        public string HttpMethod => _httpRequest != null ? _httpRequest.HttpMethod : _httpRequestBase.HttpMethod;
        //
        // Summary:
        //     Gets or sets the HTTP data transfer method (GET or POST) used by the client.
        //
        // Returns:
        //     A string representing the HTTP invocation type sent by the client.
        public string RequestType { get; set; }
        //
        // Summary:
        //     Gets or sets the MIME content type of the incoming request.
        //
        // Returns:
        //     A string representing the MIME content type of the incoming request, for example,
        //     "text/html". Additional common MIME types include "audio.wav", "image/gif", and
        //     "application/pdf".
        public string ContentType { get; set; }
        //
        // Summary:
        //     Specifies the length, in bytes, of content sent by the client.
        //
        // Returns:
        //     The length, in bytes, of content sent by the client.
        public int ContentLength => _httpRequest != null ? _httpRequest.TotalBytes : _httpRequestBase.TotalBytes;
        //
        // Summary:
        //     Gets a System.Threading.CancellationToken object that is tripped when a request
        //     times out.
        //
        // Returns:
        //     The cancellation token.
        public CancellationToken TimedOutToken => _httpRequest != null ? _httpRequest.TimedOutToken : _httpRequestBase.TimedOutToken;
        //
        // Summary:
        //     Gets a string array of client-supported MIME accept types.
        //
        // Returns:
        //     A string array of client-supported MIME accept types.
        public string[] AcceptTypes => _httpRequest != null ? _httpRequest.AcceptTypes : _httpRequestBase.AcceptTypes;
        //
        // Summary:
        //     Gets a value indicating whether the request has been authenticated.
        //
        // Returns:
        //     true if the request is authenticated; otherwise, false.
        public bool IsAuthenticated => _httpRequest != null ? _httpRequest.IsAuthenticated : _httpRequestBase.IsAuthenticated;
        //
        // Summary:
        //     Gets a value indicating whether the HTTP connection uses secure sockets (that
        //     is, HTTPS).
        //
        // Returns:
        //     true if the connection is an SSL connection; otherwise, false.
        public bool IsSecureConnection => _httpRequest != null ? _httpRequest.IsSecureConnection : _httpRequestBase.IsSecureConnection;
        //
        // Summary:
        //     Gets the virtual path of the current request.
        //
        // Returns:
        //     The virtual path of the current request.
        public string Path => _httpRequest != null ? _httpRequest.Path : _httpRequestBase.Path;
        //
        // Summary:
        //     Gets the anonymous identifier for the user, if present.
        //
        // Returns:
        //     A string representing the current anonymous user identifier.
        public string AnonymousID => _httpRequest != null ? _httpRequest.AnonymousID : _httpRequestBase.AnonymousID;
        //
        // Summary:
        //     Gets the virtual path of the current request.
        //
        // Returns:
        //     The virtual path of the current request.
        public string FilePath => _httpRequest != null ? _httpRequest.FilePath : _httpRequestBase.FilePath;
        //
        // Summary:
        //     Gets or sets the character set of the entity-body.
        //
        // Returns:
        //     An System.Text.Encoding object representing the client's character set.
        public Encoding ContentEncoding
        {
            get
            {
                return _httpRequest != null ? _httpRequest.ContentEncoding : _httpRequestBase.ContentEncoding;
            }
            set
            {
                if (_httpRequest != null)
                    _httpRequest.ContentEncoding = value;
                else
                    _httpRequestBase.ContentEncoding = value;
            }
        }

        //
        // Summary:
        //     Forcibly terminates the underlying TCP connection, causing any outstanding I/O
        //     to fail. You might use this method in response to an attack by a malicious HTTP
        //     client.
        public void Abort()
        {
            if (_httpRequest != null)
                _httpRequest.Abort();
            else
                _httpRequestBase.Abort();
        }
        //
        // Summary:
        //     Performs a binary read of a specified number of bytes from the current input
        //     stream.
        //
        // Parameters:
        //   count:
        //     The number of bytes to read.
        //
        // Returns:
        //     A byte array.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     count is 0.- or -count is greater than the number of bytes available.
        public byte[] BinaryRead(int count)
        {
            if (_httpRequest != null)
                return _httpRequest.BinaryRead(count);
            else
                return _httpRequestBase.BinaryRead(count);
        }
        //
        // Summary:
        //     Gets a System.IO.Stream object that can be used to read the incoming HTTP entity
        //     body.
        //
        // Returns:
        //     A System.IO.Stream object that can be used to read the incoming HTTP entity body.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     The request's entity body has already been loaded and parsed. Examples of properties
        //     that cause the entity body to be loaded and parsed include the following: The
        //     System.Web.HttpRequest.Form property.The System.Web.HttpRequest.Files property.The
        //     System.Web.HttpRequest.InputStream property.The System.Web.HttpRequest.GetBufferlessInputStream
        //     method.To avoid this exception, call the System.Web.HttpRequest.ReadEntityBodyMode
        //     method first. This exception is also thrown if the client disconnects while the
        //     entity body is being read.
        public Stream GetBufferedInputStream()
        {
            if (_httpRequest != null)
                return _httpRequest.GetBufferedInputStream();
            else
                return _httpRequestBase.GetBufferedInputStream();
        }
        //
        // Summary:
        //     Gets a System.IO.Stream object that can be used to read the incoming HTTP entity
        //     body.
        //
        // Returns:
        //     A System.IO.Stream object that can be used to read the incoming HTTP entity body.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     The request's entity body has already been loaded and parsed. Examples of properties
        //     that cause the entity body to be loaded and parsed include the following:System.Web.HttpRequest.FormSystem.Web.HttpRequest.InputStreamSystem.Web.HttpRequest.FilesSystem.Web.HttpRequest.GetBufferedInputStreamTo
        //     avoid this exception, call the System.Web.HttpRequest.ReadEntityBodyMode method
        //     first. This exception is also thrown if the client disconnects while the entity
        //     body is being read.
        public Stream GetBufferlessInputStream()
        {
            if (_httpRequest != null)
                return _httpRequest.GetBufferlessInputStream();
            else
                return _httpRequestBase.GetBufferlessInputStream();
        }
        //
        // Summary:
        //     Gets a System.IO.Stream object that can be used to read the incoming HTTP entity
        //     body, optionally disabling the request-length limit that is set in the System.Web.Configuration.HttpRuntimeSection.MaxRequestLength
        //     property.
        //
        // Parameters:
        //   disableMaxRequestLength:
        //     true to disable the request-length limit; otherwise, false.
        //
        // Returns:
        //     A System.IO.Stream object that can be used to read the incoming HTTP entity body.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     The request's entity body has already been loaded and parsed. Examples of properties
        //     that cause the entity body to be loaded and parsed include the following: The
        //     System.Web.HttpRequest.Form property.The System.Web.HttpRequest.Files property.The
        //     System.Web.HttpRequest.InputStream property.The System.Web.HttpRequest.GetBufferedInputStream
        //     method.To avoid this exception, call the System.Web.HttpRequest.ReadEntityBodyMode
        //     method first. This exception is also thrown if the client disconnects while the
        //     entity body is being read.
        public Stream GetBufferlessInputStream(bool disableMaxRequestLength)
        {
            if (_httpRequest != null)
                return _httpRequest.GetBufferlessInputStream(disableMaxRequestLength);
            else
                return _httpRequestBase.GetBufferlessInputStream(disableMaxRequestLength);
        }
        //
        // Summary:
        //     Provides IIS with a copy of the HTTP request entity body and with information
        //     about the request entity object.
        //
        // Parameters:
        //   buffer:
        //     An array that contains the request entity data.
        //
        //   offset:
        //     The zero-based position in buffer at which to begin storing the request entity
        //     data.
        //
        //   count:
        //     The number of bytes to read into the buffer array.
        //
        // Exceptions:
        //   T:System.PlatformNotSupportedException:
        //     The method was invoked on a version of IIS earlier than IIS 7.0.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     count is a negative value.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     offset is a negative value.
        //
        //   T:System.ArgumentException:
        //     The number of items in count is larger than the available space in buffer, given
        //     the offset value.
        public void InsertEntityBody(byte[] buffer, int offset, int count)
        {
            if (_httpRequest != null)
                _httpRequest.InsertEntityBody(buffer, offset, count);
            else
                _httpRequestBase.InsertEntityBody(buffer, offset, count);
        }
        //
        // Summary:
        //     Provides IIS with a copy of the HTTP request entity body.
        //
        // Exceptions:
        //   T:System.PlatformNotSupportedException:
        //     The method was invoked on a version of IIS earlier than IIS 7.0.
        public void InsertEntityBody()
        {
            if (_httpRequest != null)
                _httpRequest.InsertEntityBody();
            else
                _httpRequestBase.InsertEntityBody();
        }
        //
        // Summary:
        //     Maps an incoming image-field form parameter to appropriate x-coordinate and y-coordinate
        //     values.
        //
        // Parameters:
        //   imageFieldName:
        //     The name of the form image map.
        //
        // Returns:
        //     A two-dimensional array of integers.
        public int[] MapImageCoordinates(string imageFieldName)
        {
            if (_httpRequest != null)
                return _httpRequest.MapImageCoordinates(imageFieldName);
            else
                return _httpRequestBase.MapImageCoordinates(imageFieldName);
        }
        //
        // Summary:
        //     Maps the specified virtual path to a physical path.
        //
        // Parameters:
        //   virtualPath:
        //     The virtual path (absolute or relative) for the current request.
        //
        //   baseVirtualDir:
        //     The virtual base directory path used for relative resolution.
        //
        //   allowCrossAppMapping:
        //     true to indicate that virtualPath may belong to another application; otherwise,
        //     false.
        //
        // Returns:
        //     The physical path on the server.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     allowCrossMapping is false and virtualPath belongs to another application.
        //
        //   T:System.Web.HttpException:
        //     No System.Web.HttpContext object is defined for the request.
        public string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            if (_httpRequest != null)
                return _httpRequest.MapPath(virtualPath, baseVirtualDir, allowCrossAppMapping);
            else
                return _httpRequestBase.MapPath(virtualPath, baseVirtualDir, allowCrossAppMapping);
        }
        //
        // Summary:
        //     Maps the specified virtual path to a physical path.
        //
        // Parameters:
        //   virtualPath:
        //     The virtual path (absolute or relative) for the current request.
        //
        // Returns:
        //     The physical path on the server specified by virtualPath.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     No System.Web.HttpContext object is defined for the request.
        public string MapPath(string virtualPath)
        {
            if (_httpRequest != null)
                return _httpRequest.MapPath(virtualPath);
            else
                return _httpRequestBase.MapPath(virtualPath);
        }
        //
        // Summary:
        //     Maps an incoming image field form parameter into appropriate x and y coordinate
        //     values.
        //
        // Parameters:
        //   imageFieldName:
        //     The name of the image field.
        //
        // Returns:
        //     The x and y coordinate values.
        public double[] MapRawImageCoordinates(string imageFieldName)
        {
            if (_httpRequest != null)
                return _httpRequest.MapRawImageCoordinates(imageFieldName);
            else
                return _httpRequestBase.MapRawImageCoordinates(imageFieldName);
        }
        //
        // Summary:
        //     Saves an HTTP request to disk.
        //
        // Parameters:
        //   filename:
        //     The physical drive path.
        //
        //   includeHeaders:
        //     A Boolean value specifying whether an HTTP header should be saved to disk.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     The System.Web.Configuration.HttpRuntimeSection.RequireRootedSaveAsPath property
        //     of the System.Web.Configuration.HttpRuntimeSection is set to true but filename
        //     is not an absolute path.
        public void SaveAs(string filename, bool includeHeaders)
        {
            if (_httpRequest != null)
                _httpRequest.SaveAs(filename, includeHeaders);
            else
                _httpRequestBase.SaveAs(filename, includeHeaders);
        }
        //
        // Summary:
        //     Causes validation to occur for the collections accessed through the System.Web.HttpRequest.Cookies,
        //     System.Web.HttpRequest.Form, and System.Web.HttpRequest.QueryString properties.
        //
        // Exceptions:
        //   T:System.Web.HttpRequestValidationException:
        //     Potentially dangerous data was received from the client.
        public void ValidateInput()
        {
            if (_httpRequest != null)
                _httpRequest.ValidateInput();
            else
                _httpRequestBase.ValidateInput();
        }

        public static implicit operator MyHttpRequest(HttpRequest request)
        {
            return new MyHttpRequest(request);
        }
        public static implicit operator MyHttpRequest(HttpRequestBase request)
        {
            return new MyHttpRequest(request);
        }
    }
}