using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolutionMerger.Models;
using SolutionMerger.Parsers;

namespace SolutionMerger.Utils
{
    public static class ProjectReferenceFixer
    {
        public static void FixAllSolutions(SolutionInfo[] allSolutions, out string errors)
        {
            var dupes = SolutionDiagnostics.GetProjectGuidDuplicates(allSolutions);
            if (dupes.Length == 0)
            {
                errors = null;
                return;
            }

            errors = SolutionDiagnostics.DiagnoseDupeGuidsInTheSameSolution(dupes);
            if (!string.IsNullOrEmpty(errors))
                return;

            var dupedProjects = dupes.SelectMany(g => g.Skip(1)).ToArray();//it's sufficient to rename all but one dupe - it'll be left with uniquer GUID

            //Find all solutions with dupes
            var slns = allSolutions.Where(s => s.Projects.Any(p => dupedProjects.Contains(p, BaseProject.ProjectGuidLocationComparer))).ToDictionary(s => s, s => s.Text);
            //Find all projects of these solutions - check for read/write access??
            var prjs = slns.Keys.SelectMany(s => s.Projects).OfType<Project>().Distinct<Project>(BaseProject.ProjectGuidLocationComparer).Where(pp => File.Exists(pp.AbsolutePath)).ToDictionary(pp => pp, pp => File.ReadAllText(pp.AbsolutePath));

            //checks for Read/Write access :) - overwrites potentially modified files without changes
            errors = OverwriteFiles(slns, prjs);

            if (!string.IsNullOrEmpty(errors))
                return;

            foreach (var dupedProject in dupedProjects)
            {
                var dupe = dupedProject;
                var brokenGuid = dupe.Guid.Substring(1, dupe.Guid.Length - 2);
                var newGuid = Guid.NewGuid().ToString().ToUpper();

                var affectedSolutions = slns.Keys.Where(s => s.Projects.Contains(dupe, BaseProject.ProjectGuidLocationComparer)).ToArray();
                foreach (var s in affectedSolutions)
                {
                    slns[s] = slns[s].Replace(brokenGuid, newGuid);
                }

                if (dupe is ProjectDirectory)// directories are mentioned only in solution files
                    continue;

                var affectedProjects = prjs.Keys.Where(p => affectedSolutions.Contains(p.ProjectInfo.SolutionInfo)).ToArray();
                foreach (var p in affectedProjects)
                {
                    prjs[p] = prjs[p].Replace(brokenGuid, newGuid);
                }
            }

            errors = OverwriteFiles(slns, prjs);
        }

        private static string OverwriteFiles(Dictionary<SolutionInfo, string> slns, Dictionary<Project, string> projectsToCleanup)
        {
            var errorLog = new StringBuilder();
            foreach (var sln in slns.Keys)
            {
                var filename = Path.Combine(sln.BaseDir, sln.Name + ".sln");
                try
                {
                    File.WriteAllText(filename, slns[sln]);
                }
                catch
                {
                    errorLog.AppendLine("Cannot write to file: " + filename);
                }
            }

            foreach (var prj in projectsToCleanup.Keys)
            {
                var filename = prj.AbsolutePath;
                try
                {
                    File.WriteAllText(filename, projectsToCleanup[prj]);
                }
                catch
                {
                    errorLog.AppendLine("Cannot write to file: " + filename);
                }
            }

            return errorLog.ToString();
        }
    }
}