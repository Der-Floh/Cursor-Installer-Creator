using System;
using System.Linq;

namespace Cursor_Installer_Creator.Data;

public sealed class Version
{
    public int Major { get; set; } = -1;
    public int Minor { get; set; } = -1;
    public int Patch { get; set; } = -1;
    public bool IsPrerelease { get; set; }

    public Version() { }

    public Version(int major, int minor, int patch, bool isPrerelease = false)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        IsPrerelease = isPrerelease;
    }

    public Version(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return;

        version = version.ToLower();
        version = version.TrimStart('v');

        var parts = version.Split('-');
        IsPrerelease = parts.Length > 1;

        var versionParts = parts[0].Split('.').Select(int.Parse).ToArray();
        Major = versionParts[0];
        Minor = versionParts[1];
        Patch = versionParts[2];
    }

    public override string ToString() => IsPrerelease ? $"{Major}.{Minor}.{Patch}-alpha" : $"{Major}.{Minor}.{Patch}";

    public override bool Equals(object? obj)
    {
        if (obj is Version otherVersion)
        {
            return Major == otherVersion.Major
                && Minor == otherVersion.Minor
                && Patch == otherVersion.Patch
                && IsPrerelease == otherVersion.IsPrerelease;
        }
        return base.Equals(obj);
    }

    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, IsPrerelease);

    public static bool operator >(Version v1, Version v2)
    {
        if (v1.Major != v2.Major)
            return v1.Major > v2.Major;
        if (v1.Minor != v2.Minor)
            return v1.Minor > v2.Minor;
        if (v1.Patch != v2.Patch)
            return v1.Patch > v2.Patch;
        return !v1.IsPrerelease && v2.IsPrerelease;
    }

    public static bool operator <(Version v1, Version v2)
    {
        return !(v1 > v2) && !v1.Equals(v2);
    }

    public static bool operator >=(Version v1, Version v2)
    {
        return v1 > v2 || v1.Equals(v2);
    }

    public static bool operator <=(Version v1, Version v2)
    {
        return v1 < v2 || v1.Equals(v2);
    }

    public static Version operator +(Version v1, Version v2)
    {
        return new Version(
            v1.Major + v2.Major,
            v1.Minor + v2.Minor,
            v1.Patch + v2.Patch,
            v1.IsPrerelease || v2.IsPrerelease
        );
    }

    public static Version operator -(Version v1, Version v2)
    {
        return new Version(
            Math.Max(v1.Major - v2.Major, 0),
            Math.Max(v1.Minor - v2.Minor, 0),
            Math.Max(v1.Patch - v2.Patch, 0),
            v1.IsPrerelease
        );
    }
}
