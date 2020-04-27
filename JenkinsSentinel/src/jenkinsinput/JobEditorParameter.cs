using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src.jenkinsinput
{
    public abstract class JobEditorParameter
    {
        public JobEditorParameter(string Name, ParamType ParameterType)
        {
            this.Name = Name;
            this.paramType = ParameterType;
        }

        public string Name;
        public ParamType paramType;

        public abstract object GetValue();
    }

    public class TextParameter : JobEditorParameter
    {
        public TextParameter(string Name, ParamType ParameterType, string DefaultValue)
            : base(Name, ParameterType)
        {
            this.Value = DefaultValue;
        }

        public string Value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class BooleanParameter : JobEditorParameter
    {
        public BooleanParameter(string Name, ParamType ParameterType, bool DefaultValue)
            : base(Name, ParameterType)
        {
            this.Value = DefaultValue;
        }

        public bool Value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class ListParameter : JobEditorParameter
    {
        public ListParameter(string Name, ParamType ParameterType, List<string> DefaultValue)
            : base(Name, ParameterType)
        {
            this.Value = DefaultValue;
        }

        public List<string> Value;

        public override object GetValue()
        {
            return Value;
        }
    }
}
