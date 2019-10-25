using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace OrchardCore.Localization
{
    /// <remarks>
    /// A LocalizedString is not encoded, so it can contain the formatted string
    /// including the argument values.
    /// </remarks>
    public class NullStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource) => NullStringLocalizer.Instance;

        public IStringLocalizer Create(string baseName, string location) => NullStringLocalizer.Instance;

        internal class NullStringLocalizer : IStringLocalizer
        {
            private static readonly PluralizationRuleDelegate _defaultPluralRule = n => (n == 1) ? 0 : 1;

            public static NullStringLocalizer Instance { get; } = new NullStringLocalizer();

            public LocalizedString this[string name] => new LocalizedString(name, name, false);

            public LocalizedString this[string name, params object[] arguments]
            {
                get
                {
                    var translation = name;

                    if (arguments.Length == 1 && arguments[0] is PluralizationArgument pluralArgument)
                    {
                        translation = pluralArgument.Forms[_defaultPluralRule(pluralArgument.Count)];

                        arguments = new object[pluralArgument.Arguments.Length + 1];
                        arguments[0] = pluralArgument.Count;
                        Array.Copy(pluralArgument.Arguments, 0, arguments, 1, pluralArgument.Arguments.Length);
                    }

                    translation = String.Format(translation, arguments);

                    return new LocalizedString(name, translation, false);
                }
            }

            public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
                => Enumerable.Empty<LocalizedString>();

            public IStringLocalizer WithCulture(CultureInfo culture) => Instance;
            
            public LocalizedString GetString(string name) => this[name];

            public LocalizedString GetString(string name, params object[] arguments) => this[name, arguments];
        }
    }
}