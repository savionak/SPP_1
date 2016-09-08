﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TracerLib
{
    class TraceTree
    {
        const string XML_METHOD_START = "<method name=\"{0}\" time=\"{1}\" package=\"{2}\"{3}";
        const string XML_PARAMS_COUNT = " paramsCount=";
        const string XML_METHOD_END = "</method>";
        const string XML_TAG_END = ">";
        const string XML_SELFCLOSE_TAG_END = "/>";

        const string TO_STRING_FORMAT = "{0}.{1}(paramsCount: {2}; time: {3})";


        private Stopwatch sw;

        public List<TraceTree> Children { get; set; }
        public MethodInfo Info { get; set; }

        public TraceTree(MethodInfo info)
        {
            sw = new Stopwatch();
            this.Info = info;
            this.Children = new List<TraceTree>();
        }

        public void startTimer()
        {
            sw.Reset();
            sw.Start();
        }

        public void stopTimer()
        {
            sw.Stop();
            //Console.WriteLine(sw.Elapsed.ToString());
            Info.Time = sw.ElapsedMilliseconds;
        }

        public string ToString(int indent)
        {
            string result = "";
            for (int i = 0; i < indent; ++i)
            {
                result += " ";
            }

            object[] args = new object[] {
                Info.Method.ReflectedType.Name,
                Info.Method.Name,
                Info.Method.GetParameters().Count(),
                Info.Time.ToString()
            };

            result += String.Format(TO_STRING_FORMAT, args);

            foreach (var child in Children)
            {
                result += "\n" + child.ToString(indent + 1);
            }

            return result;
        }

        public string ToXMLString(int indent)
        {
            string tab = "";
            for (int i = 0; i < indent; ++i)
            {
                tab += " ";
            }

            string paramsCountString = "";
            int paramsCount = Info.Method.GetParameters().Count();
            if (paramsCount > 0)
                 paramsCountString = XML_PARAMS_COUNT + "\"" + Info.Method.GetParameters().Count() + "\"";

            object[] args = new object[] { Info.Method.Name, Info.Time.ToString(), Info.Method.ReflectedType.Name, paramsCountString };
            string result = tab + String.Format(XML_METHOD_START, args);

            if (Children.Count > 0)
            {
                result += XML_TAG_END;
                foreach (var child in Children)
                {
                    result += "\n" + child.ToXMLString(indent + 1);
                }
                result += "\n" + tab + XML_METHOD_END;
            }
            else
            {
                result += XML_SELFCLOSE_TAG_END;
            }

            return result;
        }
    }
}
