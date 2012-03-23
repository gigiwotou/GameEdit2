using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Collections;
using System.Drawing;

namespace GameData.Helper
{
    public class PointFConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            string[] numArray = str.Split(' ');
            return new PointF(float.Parse(numArray[0]), float.Parse(numArray[1]));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            PointF p = (PointF)value;
            return String.Format("{0} {1}", p.X, p.Y);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            object xvalue = propertyValues["X"];
            object yvalue = propertyValues["Y"];
            return new PointF((float)xvalue, (float)yvalue);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(PointF), attributes).Sort(new string[] { "X", "Y" });
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

}
