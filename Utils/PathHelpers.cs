using System;
using System.IO;

namespace SolutionMerger.Utils
{
    public static class PathHelpers
    {
        /// <summary>
        /// Gets name of the directory (excluding path)
        /// </summary>
        public static string GetDirName(string directoryPath)
        {
            var separators = new[] {Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar};
            var path = directoryPath.TrimEnd(separators);
            return path.Substring(path.LastIndexOfAny(separators) + 1);
        }

        public static string ResolveRelativePath(string baseDir, string absolutePath)
        {
            return absolutePath.IsWebSiteUrl() || absolutePath.IsNonPath()
                ? absolutePath
                : EvaluateRelativePath(baseDir, absolutePath);
        }

        public static string ResolveAbsolutePath(string baseDir, string relativePath)
        {
            return relativePath.IsWebSiteUrl() || (relativePath.IsNonPath() && !File.Exists(baseDir + "/" + relativePath))
                ? relativePath
                : Path.GetFullPath(Path.Combine(baseDir, relativePath));
        }

        public static bool IsWebSiteUrl(this string path)
        {
            return path.Contains(@"://") || path.Contains(@":\\");
        }

        private static bool IsNonPath(this string path)
        {
            return !(path.Contains(":") || path.Contains(@"\") || path.Contains("/"));
        }

        private static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
        {
            var firstPathParts = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            var secondPathParts = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            var sameCounter = 0;
            for (var i = 0; i < Math.Min(firstPathParts.Length, secondPathParts.Length); i++)
            {
                if (!firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
                    break;

                sameCounter++;
            }

            if (sameCounter == 0)
                return absoluteFilePath;

            var newPath = "";
            for (var i = sameCounter; i < firstPathParts.Length; i++)
            {
                if (i > sameCounter)
                    newPath += Path.DirectorySeparatorChar;

                newPath += "..";
            }

            if (newPath.Length == 0)
                newPath = ".";

            for (var i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath += Path.DirectorySeparatorChar;
                newPath += secondPathParts[i];
            }

            return newPath;
        }
    }
}