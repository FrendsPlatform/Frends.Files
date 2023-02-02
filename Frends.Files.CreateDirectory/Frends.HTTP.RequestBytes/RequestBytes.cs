using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Frends.HTTP.RequestBytes.Definitions;

[assembly: InternalsVisibleTo("Frends.HTTP.RequestBytes.Tests")]
namespace Frends.Files.CreateDirectory;

/// <summary>
/// Task class.
/// </summary>
public class HTTP
{
    /// <summary>
    /// HTTP request with byte return type
    /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.HTTP.RequestBytes)
    /// </summary>
    /// <param name="input"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"/>
    /// <returns>Object { dynamic Body, double BodySizeInMegaBytes, MediaTypeHeaderValue ContentType, Dictionary(string, string) Headers, int StatusCode }</returns>
    public static async Task<object> CreateDirectory([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        
    }
}

