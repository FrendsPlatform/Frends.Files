using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Frends.Files.Move.Definitions;
using SimpleImpersonation;

namespace Frends.Files.Move;

/// <summary>
/// Handles logic of running actions under impersonated user context.
/// </summary>
internal static class ImpersonatedAction
{
    internal static TResult Execute<TResult>(
        Func<TResult> action,
        Connection connection,
        ImpersonatedPart part)
    {
        return ExecuteInternal(action, connection, part);
    }

    internal static void Execute(
        Action action,
        Connection connection,
        ImpersonatedPart part)
    {
        ExecuteInternal(() =>
        {
            action();
            return true;
        }, connection, part);
    }

    private static TResult ExecuteInternal<TResult>(
        Func<TResult> action,
        Connection connection,
        ImpersonatedPart part
    )
    {
        var (username, password) = part switch
        {
            ImpersonatedPart.Source when connection.SourceIsRemote => (connection.SourceUserName,
                connection.SourcePassword),
            ImpersonatedPart.Target when connection.TargetIsRemote => (connection.TargetUserName,
                connection.TargetPassword),
            _ => (string.Empty, string.Empty)
        };

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            return action();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var (domain, user) = Helpers.GetDomainAndUsername(username);
        var credentials = new UserCredentials(domain, user, password);
        using var userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return WindowsIdentity.RunImpersonated(userHandle, action);
    }
}