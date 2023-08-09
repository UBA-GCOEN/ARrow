using System;
using System.ComponentModel;

namespace UnityEditor.XR.ARKit
{
    static class ACTool
    {
        public class ACToolException : Exception
        {
            public ACToolException() { }

            public ACToolException(string msg)
                : base(msg) { }
        }

        public class ExecutionFailedException : ACToolException
        {
            public int exitCode { get; }

            public string stderr { get; }

            public ExecutionFailedException(int exitCode, string stderr)
                : base($"Execution of actool failed with exit code {exitCode}. stderr:\n{stderr}")
            {
                this.exitCode = exitCode;
                this.stderr = stderr;
            }
        }

        public class CompilationFailedException : ACToolException
        {
            public CompilationFailedException() { }

            public CompilationFailedException(string msg)
                : base(msg) { }
        }

        public class XCRunNotFoundException : ACToolException
        {
            public Exception innerException { get; }

            public XCRunNotFoundException() { }

            public XCRunNotFoundException(Exception innerException)
                : base(innerException.ToString())
            {
                this.innerException = innerException;
            }
        }

        public static string Compile(string assetCatalogPath, string outputDirectory, Version minimumDeploymentTarget)
        {
            try
            {
                var (stdout, stderr, exitCode) = Cli.Execute($"xcrun", new[]
                {
                    "actool",
                    $"\"{assetCatalogPath}\"",
                    $"--compile \"{outputDirectory}\"",
                    "--platform iphoneos",
                    $"--minimum-deployment-target {minimumDeploymentTarget}",
                    "--warnings",
                    "--errors"
                });

                if (exitCode != 0)
                    throw new ExecutionFailedException(exitCode, stderr);

                // Parse the plist
                var plist = Plist.ReadFromString(stdout);
                var outputFiles = plist.root?["com.apple.actool.compilation-results"]?["output-files"]?.AsArray();
                if (outputFiles?.Length < 1)
                    throw new CompilationFailedException();

                return outputFiles[0].AsString();
            }
            catch (Win32Exception e)
            {
                throw new XCRunNotFoundException(e);
            }
        }
    }
}
