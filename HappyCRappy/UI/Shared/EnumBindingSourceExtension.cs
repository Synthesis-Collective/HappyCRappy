using System;
using System.Windows.Markup;

namespace HappyCRappy;

public class EnumBindingSourceExtension : MarkupExtension // https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/
{
    private Type? _enumType;
    public Type EnumType
    {
#pragma warning disable CS8603 // Possible null reference return.
        get { return this._enumType; }
#pragma warning restore CS8603 // Possible null reference return.
        set
        {
            if (value != this._enumType)
            {
                if (null != value)
                {
                    Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                    if (!enumType.IsEnum)
                        throw new ArgumentException("Type must be for an Enum.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }    

                this._enumType = value;
            }
        }
    }

    public EnumBindingSourceExtension() { }

    public EnumBindingSourceExtension(Type enumType)
    {
        this.EnumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (null == this._enumType)
            throw new InvalidOperationException("The EnumType must be specified.");

        Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
        Array enumValues = Enum.GetValues(actualEnumType);

        if (actualEnumType == this._enumType)
            return enumValues;

        Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
        enumValues.CopyTo(tempArray, 1);
        return tempArray;
    }
}