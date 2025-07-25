using System.Net;
using System.Net.Sockets;

namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when a network error occurs during an API request.
/// </summary>
public class PlanningCenterApiNetworkException : PlanningCenterApiException
{
    /// <summary>
    /// The type of network error that occurred.
    /// </summary>
    public NetworkErrorType NetworkErrorType { get; }
    
    /// <summary>
    /// The socket error code, if applicable.
    /// </summary>
    public SocketError? SocketError { get; }
    
    /// <summary>
    /// Initializes a new instance of the PlanningCenterApiNetworkException class.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="networkErrorType">The type of network error</param>
    /// <param name="socketError">The socket error code</param>
    /// <param name="requestId">The request ID from the API response</param>
    /// <param name="requestUrl">The URL that was requested</param>
    /// <param name="requestMethod">The HTTP method used</param>
    /// <param name="innerException">The inner exception</param>
    public PlanningCenterApiNetworkException(
        string message,
        NetworkErrorType networkErrorType,
        SocketError? socketError = null,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        Exception? innerException = null)
        : base(message, HttpStatusCode.ServiceUnavailable, requestId, null, requestUrl, requestMethod, innerException)
    {
        NetworkErrorType = networkErrorType;
        SocketError = socketError;
    }
    
    /// <summary>
    /// Gets a detailed message including network error information.
    /// </summary>
    public override string DetailedMessage
    {
        get
        {
            var details = new List<string> { base.DetailedMessage };
            
            details.Add($"Network error type: {NetworkErrorType}");
            
            if (SocketError.HasValue)
                details.Add($"Socket error: {SocketError.Value}");
            
            return string.Join(Environment.NewLine, details);
        }
    }
    
    /// <summary>
    /// Creates a PlanningCenterApiNetworkException from a network-related exception.
    /// </summary>
    /// <param name="exception">The network exception</param>
    /// <param name="requestUrl">The request URL</param>
    /// <param name="requestMethod">The request method</param>
    /// <returns>A new PlanningCenterApiNetworkException instance</returns>
    public static PlanningCenterApiNetworkException FromNetworkException(
        Exception exception,
        string? requestUrl = null,
        string? requestMethod = null)
    {
        var networkErrorType = NetworkErrorType.Unknown;
        SocketError? socketError = null;
        
        switch (exception)
        {
            case SocketException sockEx:
                socketError = sockEx.SocketErrorCode;
                networkErrorType = sockEx.SocketErrorCode switch
                {
                    System.Net.Sockets.SocketError.HostNotFound => NetworkErrorType.DnsResolution,
                    System.Net.Sockets.SocketError.ConnectionRefused => NetworkErrorType.ConnectionRefused,
                    System.Net.Sockets.SocketError.TimedOut => NetworkErrorType.Timeout,
                    System.Net.Sockets.SocketError.NetworkUnreachable => NetworkErrorType.NetworkUnreachable,
                    System.Net.Sockets.SocketError.HostUnreachable => NetworkErrorType.HostUnreachable,
                    _ => NetworkErrorType.Socket
                };
                break;
                
            case HttpRequestException httpEx when httpEx.Message.Contains("SSL", StringComparison.OrdinalIgnoreCase):
                networkErrorType = NetworkErrorType.SslHandshake;
                break;
                
            case HttpRequestException httpEx when httpEx.Message.Contains("DNS", StringComparison.OrdinalIgnoreCase):
                networkErrorType = NetworkErrorType.DnsResolution;
                break;
                
            case HttpRequestException:
                networkErrorType = NetworkErrorType.Http;
                break;
                
            case TaskCanceledException:
                networkErrorType = NetworkErrorType.Timeout;
                break;
        }
        
        return new PlanningCenterApiNetworkException(
            $"Network error occurred: {exception.Message}",
            networkErrorType,
            socketError,
            null,
            requestUrl,
            requestMethod,
            exception);
    }
}

/// <summary>
/// Represents the type of network error that occurred.
/// </summary>
public enum NetworkErrorType
{
    /// <summary>
    /// Unknown network error.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// DNS resolution failed.
    /// </summary>
    DnsResolution,
    
    /// <summary>
    /// Connection was refused by the server.
    /// </summary>
    ConnectionRefused,
    
    /// <summary>
    /// Network timeout occurred.
    /// </summary>
    Timeout,
    
    /// <summary>
    /// Network is unreachable.
    /// </summary>
    NetworkUnreachable,
    
    /// <summary>
    /// Host is unreachable.
    /// </summary>
    HostUnreachable,
    
    /// <summary>
    /// SSL/TLS handshake failed.
    /// </summary>
    SslHandshake,
    
    /// <summary>
    /// Socket-level error.
    /// </summary>
    Socket,
    
    /// <summary>
    /// HTTP-level error.
    /// </summary>
    Http
}