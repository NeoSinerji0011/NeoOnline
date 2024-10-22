using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    // Language dosyasının (bu project için) görevini yapar
    public class ResourceHelper
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        public static ResourceManager ResourceManager
        {
            get
            {
                try
                {
                    if (object.ReferenceEquals(resourceMan, null))
                    {
                        Type babonline = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => p.Name == "babonline").FirstOrDefault();
                        if (babonline != null)
                        {
                            ResourceManager temp = new ResourceManager(babonline.FullName, babonline.Assembly);
                            resourceMan = temp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ILogService _log = System.Web.Mvc.DependencyResolver.Current.GetService<ILogService>();
                    _log.Error(ex);

                    if (ex is System.Reflection.ReflectionTypeLoadException)
                    {
                        System.Reflection.ReflectionTypeLoadException refx = ex as System.Reflection.ReflectionTypeLoadException;
                        if (refx.LoaderExceptions != null && refx.LoaderExceptions.Length > 0)
                        {
                            System.Text.StringBuilder sb = new StringBuilder();
                            foreach (var item in refx.LoaderExceptions)
                            {
                                sb.AppendLine(item.Message);
                            }

                            _log.Error(sb.ToString());
                        }
                    }
                }
                return resourceMan;
            }
        }

        public static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        public static string GetString(string name)
        {
            string result = String.Empty;

            if (ResourceManager != null)
                result = ResourceManager.GetString(name);

            return result;
        }
    }
}
