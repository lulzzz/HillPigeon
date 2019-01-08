
using System;
using System.Collections.Generic;

namespace HillPigeon.MvcAttributes
{
    /// <summary>
    /// Identifies an action that supports the HTTP OPTIONS method.
    /// </summary>
    public class HttpOptionsAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new [] { "OPTIONS" };

        /// <summary>
        /// Creates a new <see cref="HttpOptionsAttribute"/>.
        /// </summary>
        public HttpOptionsAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HttpOptionsAttribute"/> with the given route template.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpOptionsAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}