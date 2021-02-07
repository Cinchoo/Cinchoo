using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    public abstract class ChoPlugInBuilder : ChoNotifyPropertyChangedObject, ICloneable
    {
        private string _name;
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "Name");
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _description;
        [XmlAttribute("description")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        private bool _doPropertyResolve;
        [XmlAttribute("doPropertyResolve")]
        public bool DoPropertyResolve
        {
            get { return _doPropertyResolve; }
            set
            {
                if (_doPropertyResolve == value) return;
                _doPropertyResolve = value;
                RaisePropertyChanged("DoPropertyResolve");
            }
        }

        private bool _enabled = true;
        [XmlAttribute("enabled")]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                RaisePropertyChanged("Enabled");
            }
        }

        private Lazy<ChoPlugInBuilderProperty> _plugInBuilderProperty;

        public ChoPlugInBuilder()
        {
            _plugInBuilderProperty = new Lazy<ChoPlugInBuilderProperty>(() =>
            {
                ChoPlugInAttribute plugInAttribute = ChoType.GetAttribute<ChoPlugInAttribute>(GetType());
                if (plugInAttribute == null)
                    throw new ChoPlugInException("Missing ChoPlugInAttribute decorated for '{0}' plugin builder.".FormatString(GetType().Name));

                Type type = plugInAttribute.PlugInBuilderPropertyType;
                if (type == null)
                    throw new ChoPlugInException("Can't find plugin property for '{0}' plugin builder.".FormatString(GetType().Name));

                ChoPlugInBuilderProperty p = ChoActivator.CreateInstance(type) as ChoPlugInBuilderProperty;
                if (p == null)
                    throw new ChoPlugInException("Can't find plugin property for '{0}' plugin builder.".FormatString(GetType().Name));

                try
                {
                    InitPlugInBuilderProperty(p);
                }
                catch { }

                p.PropertyChanged += ((s, e) =>
                    {
                        ApplyPropertyValues(p, e.PropertyName);
                    });

                return p;
            }, true);
        }

        public virtual ChoPlugIn CreatePlugIn()
        {
            ChoPlugInAttribute plugInAttribute = ChoType.GetAttribute<ChoPlugInAttribute>(GetType());
            if (plugInAttribute == null)
                throw new ChoPlugInException("Missing ChoPlugInAttribute decorated for '{0}' plugin builder.".FormatString(GetType().Name));

            Type type = plugInAttribute.PlugInType;
            if (type == null)
                throw new ChoPlugInException("Can't find plugin for '{0}' plugin builder.".FormatString(GetType().Name));

            ChoPlugIn p = ChoActivator.CreateInstance(type) as ChoPlugIn;
            if (p == null)
                throw new ChoPlugInException("Can't find plugin for '{0}' plugin builder.".FormatString(GetType().Name));

            InitPlugIn(p);
            return p;
        }

        internal static Type GetPlugInType(Type plugInBuilderType)
        {
            ChoPlugInAttribute plugInAttribute = ChoType.GetAttribute<ChoPlugInAttribute>(plugInBuilderType);
            if (plugInAttribute == null)
                return null;

            Type type = plugInAttribute.PlugInType;
            if (type == null)
                throw new ChoPlugInException("Can't find plugin for '{0}' plugin builder.".FormatString(plugInBuilderType.Name));

            return type;
        }

        internal static string GetPlugInName(Type plugInBuilderType)
        {
            ChoPlugInAttribute plugInAttribute = ChoType.GetAttribute<ChoPlugInAttribute>(plugInBuilderType);
            if (plugInAttribute == null)
                throw new ChoPlugInException("Missing ChoPlugInAttribute decorated for '{0}' plugin builder.".FormatString(plugInBuilderType.Name));

            return plugInAttribute.Name;
        }

        protected virtual void InitPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;

            plugIn.Name = Name;
            plugIn.Description = Description;
            plugIn.DoPropertyResolve = DoPropertyResolve;
            plugIn.Enabled = Enabled;
        }

        public virtual ChoPlugInBuilderProperty PlugInBuilderProperty
        {
            get
            {
                return _plugInBuilderProperty.Value;
            }
        }

        protected virtual void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            plugInBuilderProperty.Name = Name;
            plugInBuilderProperty.Description = Description;
            plugInBuilderProperty.DoPropertyResolve = DoPropertyResolve;
            plugInBuilderProperty.Enabled = Enabled;
        }

        protected virtual bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;

            if (propertyName == "Name")
                Name = plugInBuilderProperty.Name;
            else if (propertyName == "Description")
                Description = plugInBuilderProperty.Description;
            else if (propertyName == "DoPropertyResolve")
                DoPropertyResolve = plugInBuilderProperty.DoPropertyResolve;
            else if (propertyName == "Enabled")
                Enabled = plugInBuilderProperty.Enabled;
            else
                return false;

            return true;
        }

        public virtual object GetPropertiesWindow()
        {
            ChoPlugInPropertyGrid obj = new ChoPlugInPropertyGrid();
            obj.SelectedObject = PlugInBuilderProperty;

            return obj;
        }

        public virtual void ShowPropertyWindow()
        {
            object wnd = GetPropertiesWindow();
            if (wnd is Form)
                ((Form)wnd).ShowDialog();
            else if (wnd is Window)
                ((Window)wnd).ShowDialog();
        }

        protected virtual void Clone(ChoPlugInBuilder o)
        {
            o.Name = Name;
            o.Description = Description;
            o.DoPropertyResolve = DoPropertyResolve;
            o.Enabled = Enabled;
        }

        public virtual object Clone()
        {
            object cloneObj = ChoActivator.CreateInstance(GetType());

            Clone(cloneObj as ChoPlugInBuilder);

            return cloneObj;
        }

        public static string ToXml(ChoPlugInBuilder[] builders)
        {
            StringBuilder xml = new StringBuilder();

            xml.AppendLine("<plugIns>");
            if (builders != null)
            {
                foreach (ChoPlugInBuilder pb in builders)
                {
                    xml.AppendLine(pb.ToXml());
                }
            }
            xml.AppendLine("</plugIns>");
            return xml.ToString().IndentXml();
        }

        public static void Save(string filePath, ChoPlugInBuilder[] builders)
        {
            File.WriteAllText(filePath, ToXml(builders));
        }

        public static TextReader CreateReader(ChoPlugInBuilder[] builders)
        {
            return new StringReader(ToXml(builders));
        }

        public static IEnumerable<ChoPlugInBuilder> LoadFrom(string plugInDefFilePath)
        {
            ChoGuard.ArgumentNotNullOrEmpty(plugInDefFilePath, "PlugInDefFilePath");
            FileIOPermission FilePermission = new FileIOPermission(FileIOPermissionAccess.AllAccess, plugInDefFilePath);
            FilePermission.Demand();

            return Load(File.OpenText(plugInDefFilePath));
        }

        public static IEnumerable<ChoPlugInBuilder> Parse(string plugInDefXml)
        {
            return Load(new StringReader(plugInDefXml));
        }

        public static IEnumerable<ChoPlugInBuilder> LoadFrom(Stream plugInDefStream)
        {
            ChoGuard.ArgumentNotNullOrEmpty(plugInDefStream, "PlugInDefStream");
            return Load(new StreamReader(plugInDefStream));
        }

        public static IEnumerable<ChoPlugInBuilder> LoadFrom(TextReader plugInDefStream)
        {
            ChoGuard.ArgumentNotNullOrEmpty(plugInDefStream, "PlugInDefStream");
            return Load(plugInDefStream);
        }

        private static IEnumerable<ChoPlugInBuilder> Load(TextReader plugInDefStream)
        {
            if (plugInDefStream == null) yield break;

            foreach (ChoPlugInBuilder plugInBuilder in ChoObject.FromXml(plugInDefStream))
            {
                if (plugInBuilder == null) continue;
                yield return plugInBuilder;
            }
        }
    }
}
