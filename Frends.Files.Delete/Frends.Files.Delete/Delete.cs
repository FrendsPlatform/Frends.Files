﻿using Frends.Files.Delete.Definitions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Text.RegularExpressions;

namespace Frends.Files.Delete;

///<summary>
/// Files task.
/// </summary>
public class Files
{
    static Files()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
        if (currentContext != null)
            currentContext.Unloading += OnPluginUnloadingRequested;
    }

    /// <summary>
    /// Delete files.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.Delete)
    /// </summary>
    /// <frendsdocs>
    /// Pattern matching
    ///
    /// Delete Task uses pattern matching for deleting files.
    ///
    /// The search starts from the root directory defined in the input parameters.
    ///
    /// `* to match one or more characters in a path segment`
    ///
    /// `** to match any number of path segments, including none`
    ///
    /// Examples:
    ///
    /// `**\output\*\temp\*.txt` matches:
    ///
    /// * `test\subfolder\output\2015\temp\file.txt`
    /// * `production\output\2016\temp\example.txt`
    ///
    ///
    /// `**\temp*` matches
    ///
    /// * `prod\test\temp123.xml`
    /// * `test\temp234.xml`
    ///
    /// `subfolder\**\temp\*.xml` matches
    ///
    /// * `subfolder\temp\test.xml`
    /// * `subfolder\foo\bar\is\here\temp\test.xml`
    /// </frendsdocs>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <param name="cancellationToken">Token to stop task. This is generated by Frends.</param>
    /// <returns>Result object { List&lt;FileItem&gt; }</returns>
    public static Result Delete([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var files = ExecuteAction(() => ExecuteDelete(input, cancellationToken), options.UseGivenUserCredentialsForRemoteConnections, options.UserName, options.Password);

        return new Result(files);
    }

    private static TResult ExecuteAction<TResult>(Func<TResult> action, bool useGivenCredentials, string username, string password)
    {
        if (!useGivenCredentials)
            return action();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var (domain, user) = GetDomainAndUsername(username);

        UserCredentials credentials = new UserCredentials(domain, user, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return WindowsIdentity.RunImpersonated(userHandle, action);
    }

    private static List<FileItem> ExecuteDelete(Input input, CancellationToken cancellationToken)
    {
        var results = FindMatchingFiles(input.Directory, input.Pattern);

        var fileResults = new List<FileItem>();
        try
        {
            foreach (var path in results.Files.Select(match => Path.Combine(input.Directory, match.Path)))
            {
                cancellationToken.ThrowIfCancellationRequested();

                fileResults.Add(new FileItem(new FileInfo(path)));
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            var deletedfilesMsg = fileResults.Any() ? ": " + string.Join(",", fileResults.Select(f => f.Path)) : string.Empty;
            throw new Exception($"Could not delete all files. Error: {ex.Message}. Deleted {fileResults.Count} files{deletedfilesMsg}", ex);
        }

        return fileResults;
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");

        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    internal static PatternMatchingResult FindMatchingFiles(string directoryPath, string pattern)
    {
        // Check the user can access the folder.
        // This will return false if the path does not exist or you do not have read permissions.
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory does not exist or you do not have read access. Tried to access directory '{directoryPath}'");

        if (pattern.StartsWith("<regex>"))
        {
            string regexPattern = pattern.Substring(7);

            var matchingFiles = Directory.GetFiles(directoryPath)
                .Where(file => Regex.IsMatch(Path.GetFileName(file), regexPattern))
                .Select(file => new FilePatternMatch(Path.GetFileName(file), Path.GetFileName(file)))
                .ToList();

            return new PatternMatchingResult(matchingFiles);
        }
        else
        {
            var matcher = new Matcher();
            matcher.AddInclude(pattern);
            var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));
            return results;
        }
    }

    private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
    {
        obj.Unloading -= OnPluginUnloadingRequested;
    }
}