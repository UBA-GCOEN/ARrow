using System;
using System.Collections.Generic;
using System.IO;
using Gpm.Common.Util;
using UnityEngine;

namespace Gpm.Manager.Constant
{
    [Serializable]
    internal class InstallInfo
    {   
        [Serializable]
        public class Service
        {
            public string name;
            public string version;

            public string[] dependency;
        }

        public List<Service> installs;
        
        public string GetInstallVersion(string serviceName)
        {
            if (installs != null && installs.Count > 0)
            {
                var service = installs.Find(data => { return data.name.Equals(serviceName, StringComparison.OrdinalIgnoreCase); });
                if (service != null)
                {
                    return service.version;
                }
            }

            return string.Empty;
        }

        public Service GetInstallInfo(string serviceName)
        {
            if (installs != null && installs.Count > 0)
            {
                var service = installs.Find(data => { return data.name.Equals(serviceName, StringComparison.OrdinalIgnoreCase); });
                if (service != null)
                {
                    return service;
                }
            }

            return null;
        }

        public List<Service> GetUsingServiceList(string serviceName)
        {
            if (installs != null && installs.Count > 0)
            {
                var serviceList = installs.FindAll(data => 
                {
                    if(data.dependency != null)
                    {
                        for(int i=0;i< data.dependency.Length;i++)
                        {
                            if (data.dependency[i].Equals(serviceName, StringComparison.OrdinalIgnoreCase) == true)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                });

                if (serviceList != null)
                {
                    return serviceList;
                }
            }

            return null;
        }

        public void AddService(string name, string version, string[] dependency)
        {
            if (installs == null)
            {
                installs = new List<Service>();
            }

            var installed = installs.Find(data => data.name.Equals(name));
            if (installed != null)
            {
                installed.version = version;
            }
            else
            {
                installs.Add(new Service()
                {
                    name = name,
                    version = version,
                    dependency = dependency
                });
            }

            SaveMetafile();
        }

        public void RemoveService(ServiceInfo info)
        {
            var installed = installs.Find(data => data.name.Equals(info.title));
            if (installed == null)
            {
                return;
            }

            if (installs.Remove(installed) == true)
            {
                SaveMetafile();
            }
        }

        private void SaveMetafile()
        {
            var jsonData = JsonUtility.ToJson(this, true);
            Directory.CreateDirectory(ManagerPaths.PROJECT_DOWNLOAD_PATH);
            File.WriteAllText(GpmPathUtil.Combine(ManagerPaths.PROJECT_DOWNLOAD_PATH, ManagerPaths.INSTALL_INFO_FILE_NAME), jsonData);
        }
    }
}